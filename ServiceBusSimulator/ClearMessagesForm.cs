using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceBusSimulator
{
    public partial class ClearMessagesForm : Form
    {
        public string strTopic;
        public string strSubscription;
        public string connectionString;
        public Microsoft.ServiceBus.NamespaceManager namespaceManager;
        public SubscriptionWrapper SW;
        public ServiceBusHelper serviceBusHelper;

        public TopicDescription TopicDesc;
        public SubscriptionDescription SubscriptionDesc;

        public int iMessageCount = 0;

        private SortableBindingList<BrokeredMessage> messageBindingList;
        //private SortableBindingList<BrokeredMessage> deadletterBindingList;
        //private SortableBindingList<MessageSession> sessionBindingList;

        public bool bFlagPlayPause = true;

        private Thread demoThread = null;
        delegate void StringArgReturningVoidDelegate(string text);
        delegate void StringArgReturningVoidDelegateProgress(int iStep);

        public ClearMessagesForm()
        {
            InitializeComponent();
        }
        
        private void ClearMessagesForm_Load(object sender, EventArgs e)
        {
            textBoxTopic.Text = strTopic;
            textBoxSubscription.Text = strSubscription;
            textBoxMessageCount.Text = iMessageCount.ToString();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        /*private static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            ReceiveMessages();

            Console.WriteLine("Press any key to stop receiving messages.");
            Console.ReadKey();

            // Close the client after the ReceiveMessages method has exited.
            await queueClient.CloseAsync();
        }*/

        private void GetMessages(/*bool peek, bool all, int count*/)
        {
            bool peek = false;
            bool all = true;
            int count = iMessageCount;

            SetLogText("Delete Process Starts");

            try
            {
                
                var brokeredMessages = new List<BrokeredMessage>();
                if (peek)
                {
                    var subscriptionClient = serviceBusHelper.MessagingFactory.CreateSubscriptionClient(strTopic,
                                                                                                        strSubscription,
                                                                                                        ReceiveMode.PeekLock);
                    var totalRetrieved = 0;
                    while (totalRetrieved < count)
                    {
                        var messageEnumerable = subscriptionClient.PeekBatch(count);
                        if (messageEnumerable == null)
                        {
                            break;
                        }
                        var messageArray = messageEnumerable as BrokeredMessage[] ?? messageEnumerable.ToArray();
                        var partialList = new List<BrokeredMessage>(messageArray);
                        brokeredMessages.AddRange(partialList);
                        totalRetrieved += partialList.Count;
                        if (partialList.Count == 0)
                        {
                            break;
                        }
                    }
                    //writeToLog(string.Format(MessagesPeekedFromTheSubscription, brokeredMessages.Count, subscriptionWrapper.SubscriptionDescription.Name));
                }
                else
                {
                    MessageReceiver messageReceiver;
                    /*if (subscriptionWrapper.SubscriptionDescription.RequiresSession)
                    {
                        var subscriptionClient = serviceBusHelper.MessagingFactory.CreateSubscriptionClient(strTopic,
                                                                                                            strSubscription,
                                                                                                            ReceiveMode.ReceiveAndDelete);

                        messageReceiver = subscriptionClient.AcceptMessageSession(TimeSpan.FromSeconds(MainForm.SingletonMainForm.ReceiveTimeout));
                    }
                    else*/
                    {
                        messageReceiver = serviceBusHelper.MessagingFactory.CreateMessageReceiver(SubscriptionClient.FormatSubscriptionPath(
                                                                                                  strTopic,
                                                                                                  strSubscription),
                                                                                                  ReceiveMode.ReceiveAndDelete);

                        SetLogText("Message Receiver Created");
                    }

                    SetLogText("Need to delete: " + count);
                    var totalRetrieved = 0;
                    int retrieved;
                    do
                    {                        
                        var messages = messageReceiver.ReceiveBatch(10, TimeSpan.FromSeconds(1));
                        var enumerable = messages as BrokeredMessage[] ?? messages.ToArray();
                        retrieved = enumerable.Count();
                        SetLogText("Retrived: " + retrieved);

                        if (retrieved == 0)
                        {
                            continue;
                        }
                        totalRetrieved += retrieved;
                        SetMessagesDeletedText(totalRetrieved.ToString());
                        brokeredMessages.AddRange(enumerable);

                        SetProgress(retrieved);
                        SetLogText("Total Retrived: " + totalRetrieved);

                    } while (retrieved > 0 && (all || count > totalRetrieved));

                    SetMessagesDeletedText(totalRetrieved.ToString());
                    SetProgress(retrieved);
                }
                messageBindingList = new SortableBindingList<BrokeredMessage>(brokeredMessages)
                {
                    AllowEdit = false,
                    AllowNew = false,
                    AllowRemove = false
                };
                
            }
            catch (TimeoutException ex)
            {
                SetLogText("TimeoutException: " + ex.Message);                
            }
            catch (NotSupportedException ex)
            {
                SetLogText("NotSupportedException: " + ex.Message);//ReadMessagesOneAtTheTime(peek, all, count, messageInspector);
            }
            catch (Exception ex)
            {
                SetLogText("Exception: " + ex.Message);//HandleException(ex);
            }
            finally
            {
                SetLogText("Process Complete !!!!!");
                /*mainTabControl.ResumeLayout();
                mainTabControl.ResumeDrawing();
                tabPageMessages.ResumeLayout();
                tabPageMessages.ResumeDrawing();
                Cursor.Current = Cursors.Default;*/
            }
        }
        
        private void buttonClearMessage_Click(object sender, EventArgs e)
        {
            textBoxMessageCount.Text = iMessageCount.ToString();
            toolStripProgressBar.Maximum = iMessageCount;
            toolStripProgressBar.Step = 1;

            this.demoThread = new Thread(new ThreadStart(this.GetMessages));

            this.demoThread.Start();

            /*
            GetMessages(/*receiveModeForm.Peek false, 
                        /*receiveModeForm.All true, 
                        /*receiveModeForm.Count iMessageCount); */           
        }
        
        private void SetMessagesDeletedText(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.textBoxDeletedMessages.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetMessagesDeletedText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxDeletedMessages.Text = text;
            }
        }

        private void SetLogText(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.richTextBoxMessagesLog.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetLogText);
                this.Invoke(d, new object[] { text });
            }
            else
            {                              
                if(text.Contains("Process Complete"))
                {
                    this.richTextBoxMessagesLog.SelectionColor = Color.Green;
                }
                else if (text.Contains("Exeception"))
                {
                    this.richTextBoxMessagesLog.SelectionColor = Color.Red;
                }
                else
                {
                    this.richTextBoxMessagesLog.SelectionColor = Color.Black;
                }

                this.richTextBoxMessagesLog.AppendText(text);

                this.richTextBoxMessagesLog.AppendText("\n");
                this.richTextBoxMessagesLog.ScrollToCaret();
            }
        }

        private void SetProgress(int iStep)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.toolStripProgressBar.Control.InvokeRequired)
            {
                StringArgReturningVoidDelegateProgress d = new StringArgReturningVoidDelegateProgress(SetProgress);
                this.Invoke(d, new object[] { iStep });
            }
            else
            {
                for(int iIndex = 0;iIndex < iStep;iIndex++)
                {
                    this.toolStripProgressBar.PerformStep();
                }
                
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (bFlagPlayPause)
            {
                buttonStop.Image = imageListStatus.Images[0];
                bFlagPlayPause = false;
            }
            else
            {
                buttonStop.Image = imageListStatus.Images[1];
                bFlagPlayPause = true;
            }
            
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void GetQMessageCount(/*TopicDescription TopicDesc, SubscriptionDescription SubscriptionDesc*/)
        {
            
            long lngCount = namespaceManager.GetSubscription(TopicDesc.Path, SubscriptionDesc.Name).MessageCount;

            string strlabelMessageCount = lngCount.ToString();

            MessageCountDetails MsgCD = namespaceManager.GetSubscription(TopicDesc.Path, SubscriptionDesc.Name).MessageCountDetails;

            string strLockDuration = SW.SubscriptionDescription.LockDuration.ToString();
            string strAvilableStatus = SW.SubscriptionDescription.AvailabilityStatus.ToString();
            string strAccessedAt = SW.SubscriptionDescription.AccessedAt.ToString();

            string strlabelTransferMessageCount = MsgCD.TransferMessageCount.ToString();
            string strlabelActiveMessageCount = MsgCD.ActiveMessageCount.ToString();
            string strlabelScheduledMessageCount = MsgCD.ScheduledMessageCount.ToString();
            string strlabelDeadLetterMessageCount = MsgCD.DeadLetterMessageCount.ToString();

            textBoxMessageCount.Text = strlabelMessageCount;
            /*
            Console.WriteLine("Topic: " + SW.TopicDescription.Path);
            Console.WriteLine("Subscription: " + SW.SubscriptionDescription.Name);
            Console.WriteLine("Messages Count: " + lngCount);            
            Console.WriteLine("TransferMessageCount:" + MsgCD.TransferMessageCount);
            Console.WriteLine("ActiveMessageCount:" + MsgCD.ActiveMessageCount);
            Console.WriteLine("ScheduledMessageCount:" + MsgCD.ScheduledMessageCount);
            Console.WriteLine("DeadLetterMessageCount:" + MsgCD.DeadLetterMessageCount);
            Console.WriteLine("Lock Duration:" + strLockDuration);
            Console.WriteLine("Avilable Status:" + strAvilableStatus);
            Console.WriteLine("Accessed At:" + strAccessedAt);

            Console.WriteLine("\n");*/

            //GetMessages(true, true, (int)lngCount, SW);
        }
    }
}
