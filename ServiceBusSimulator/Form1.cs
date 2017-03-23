using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.NotificationHubs;
using ConnectivityMode = Microsoft.ServiceBus.ConnectivityMode;
using System.Threading;

namespace ServiceBusSimulator
{
    public partial class Form1 : Form
    {
        enum SEND_METHOD { SEND = 1, ASYNC_SEND = 2};
        public string connectionString = "Endpoint=sb://zivservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Is/eu+TXflptC4bpxUs9JZrUHIL1jJLUwoun+NLvPFY=";
        public Microsoft.ServiceBus.NamespaceManager namespaceManager;
        //private Microsoft.Azure.NotificationHubs.NamespaceManager notificationHubNamespaceManager;

        TopicDescription m_TopicDescSRC;
        SubscriptionDescription m_SubscriptionDescSRC;
        TopicDescription m_TopicDescTARGET;
        SubscriptionDescription m_SubscriptionDescTARGET;

        delegate void StringArgReturningVoidDelegateTimer(string text);
        private Thread TimerThread = null;
        
        delegate void ArgReturningVoidDelegateProgress(int iStep);
        private Thread CreateMessagesThread = null;
        private Thread CreateMessagesDestThread = null;
        private Thread TransferMessagesThread = null;
        private Thread ReturnMessagesThread = null;
        //private Thread TranferSendMessagesThread = null;

        delegate void ArgReturningVoidDelegateMsgCreated(string text);
        delegate void ArgReturningVoidDelegateAvrTime(string text, double AvrTimeSec, string msgIndex, int iMsgIndex);
        delegate void ArgReturningVoidTransferSendAndAsuncSendAvrTime(string text, double AvrTimeSec, string msgIndex, int iMsgIndex);        
        delegate void ArgReturnVoidSendAndAsyncSendAvrTime(string text, double AvrTimeSec, string msgIndex, int iMsgIndex, SEND_METHOD SendMethod); 
        delegate void ArgReturningVoidDelegateCompleteTime(string text);
        delegate void ArgReturningVoidDelegatePredictToComplete(string text);
        delegate void ArgReturningVoidDelegateRuningTime(string text, Color clr);
        delegate void ArgReturningVoidDelegateTransferMessageDestComplete(string text);
        delegate void ArgReturningVoidDelegateTransferMessageDestProgress(string text);        
        delegate void ArgReturningVoidDelegateRuningThreads(string text);

        public event EventHandler OnCreateMessagesFinishes;
        public event EventHandler OnTransferMessagesFinishes;
        public event EventHandler OnCreateMessagesDestFinishes;
        public event EventHandler OnReturnMessagesFinishes;

        private static string m_strCreateMessagesSeriesChart = "";
        private static int i_TimeCreatingMessagesChart = 0;

        private static string m_strTransferMessagesSeriesChart = "";
        private static int i_TimeTransferMessagesChart = 0;

        private static string m_strReturnMessagesSeriesChart = "";
        private static int i_TimeReturnMessagesChart = 0;

        private static string m_strCreateMessagesDestSeriesChart = "";
        private static int i_TimeCreatingMessagesDestChart = 0;

        private static int i_TimeCreateMsgSendMethodAvrChart = 0;
        private static int i_TimeCreateMsgAsyncSendMethodAvrChart = 0;
        private static string m_strCreateMsgSendAndAsyncSendChart = "";

        private static int i_TimeCreateMsgDestSendMethodAvrChart = 0;
        private static int i_TimeCreateMsgDestAsyncSendMethodAvrChart = 0;
        private static string m_strCreateMsgDestSendAndAsyncSendChart = "";

        private static string m_strTransferMessagesSendAndAsyncSendChart;
        private static int i_TimeTransferMessagesSendAndAsyncSendChart = 0;

        private static string m_strReturnMessagesSendAndAsyncSendChart;
        private static int i_TimeReturnMessagesSendAndAsyncSendChart = 0;



        private const int SERVER_UP = 10;
        private const int SERVER_DOWN = 7;

        private const string AllEntities = "Entities";
        private const string QueueEntities = "Queues";
        private const string TopicEntities = "Topics";
        private const string SubscriptionEntities = "Subscriptions";
        private const string PartitionEntities = "Partitions";
        private const string ConsumerGroupEntities = "Consumer Groups";
        private const string FilteredQueueEntities = "Queues (Filtered)";
        private const string FilteredTopicEntities = "Topics (Filtered)";
        private const string FilteredSubscriptionEntities = "Subscriptions (Filtered)";

        public string m_strTopicSRC;
        public string m_strTopicTARGET;
        public string m_strSubscriptionSRC;
        public string m_strSubscriptionTRGET;
        public int m_iMessageCount = 0;

        int m_totalTransferSend = 0;
        int m_totalTransferMsgThreads = 0;
        DateTime m_TranferMsg_StartDT;// = DateTime.Now;
        TimeSpan m_TranferMsgTSMsgTotal;// = new TimeSpan();
        DateTime m_TranferMsgDTPredict = new DateTime();
        int m_iTransferProdit = 0;
        int m_iTranferMsgCount = 0;

        int m_totalReturnSend = 0;
        int m_totalReturnMsgThreads = 0;
        DateTime m_ReturnMsg_StartDT;// = DateTime.Now;
        TimeSpan m_ReturnMsgTSMsgTotal;// = new TimeSpan();
        DateTime m_ReturnMsgDTPredict = new DateTime();
        int m_iReturnProdit = 0;
        int m_iReturnMsgCount = 0;

        /*private string _Topic_A_Name;
        private string _Topic_B_Name;
        private string _Subscription_A_Name;
        private string _Subscription_B_Name;
        private int _ServiceBusMessage_Q_Count_UP;
        private int _ServiceBusMessage_Q_Time_UP;
        private int _ServiceBusMessage_Q_Count_DOWN;
        private int _ServiceBusMessage_Q_Time_Down;*/

        public delegate void TraceEventHandler(object sender, string message);
        //public event TraceEventHandler TraceEvent;

        private ServiceBusHelper serviceBusHelper;
        //public static long lngIndex = 0;

        //private TelemetryClient tc = new TelemetryClient();
        

        //***************************
        // Icons
        //***************************
        private const int QueueListIconIndex = 0;
        private const int TopicListIconIndex = 1;
        private const int QueueIconIndex = 2;
        private const int TopicIconIndex = 3;
        private const int SubscriptionListIconIndex = 4;
        private const int SubscriptionIconIndex = 5;
        private const int RuleListIconIndex = 4;
        private const int RuleIconIndex = 6;
        private const int AzureIconIndex = 7;
        private const int RelayListIconIndex = 8;
        //private const int RelayNonLeafIconIndex = 10;
        private const int RelayLeafIconIndex = 9;
        //private const int RelayUriIconIndex = 11;
        internal const int UrlSegmentIconIndex = 10;
        private const int GreyQueueIconIndex = 12;
        private const int GreyTopicIconIndex = 13;
        private const int GreySubscriptionIconIndex = 14;
        private const int NotificationHubListIconIndex = 15;
        private const int NotificationHubIconIndex = 16;
        private const int EventHubListIconIndex = 17;
        private const int EventHubIconIndex = 18;
        private const int GreyEventHubIconIndex = 19;
        private const int PartitionListIconIndex = 4;
        private const int PartitionIconIndex = 20;
        private const int ConsumerGroupListIconIndex = 4;
        private const int ConsumerGroupIconIndex = 21;

        private string m_strSource_Message;
        private string m_strSource_MessageProperty;
        private string m_strSource_MessageCount;
        private string m_strSource_Topic;
        private string m_strSource_Subscription;

        private string m_strSource_MessageDest;
        private string m_strSource_MessagePropertyDest;
        private string m_strSource_MessageCountDest;
        private string m_strSource_TopicDest;
        private string m_strSource_SubscriptionDest;

        TreeNode rootNode;
        //TreeNode subscriptionRootNode;

        public enum EncodingType
        {
            // ReSharper disable once InconsistentNaming
            ASCII,
            // ReSharper disable once InconsistentNaming
            UTF7,
            // ReSharper disable once InconsistentNaming
            UTF8,
            // ReSharper disable once InconsistentNaming
            UTF32,
            Unicode
        }
        public enum EntityType
        {
            All,
            Queue,
            Topic,
            Subscription,
            Rule,
            Relay,
            NotificationHub,
            EventHub,
            ConsumerGroup
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeSBView.Enabled = false;

            textBoxTopic.Enabled = false;
            textBoxSubscription.Enabled = false;
            textBoxMessageCount.Enabled = false;
            textBoxActiveMessageCount.Enabled = false;
            textBoxTransferMessageCount.Enabled = false;
            textBoxDeadLetterMessageCount.Enabled = false;
            textBoxAvilableStatus.Enabled = false;
            textBoxCompleteTime.Enabled = false;
            //textBoxRuningTime.Enabled = false;

            textBox_InProc_MsgCreating.Enabled = false;
            textBox_InProc_MsgCreated.Enabled = false;
            textBox_InProc_AvrTime.Enabled = false;
            textBox_InProc_PredictToComplete.Enabled = false;

            textBoxTopicDest.Enabled = false;
            textBoxSubscriptionDest.Enabled = false;
            textBoxMessageCountDest.Enabled = false;
            textBoxActiveMessageCountDest.Enabled = false;
            textBoxTransferMessageCountDest.Enabled = false;
            textBoxDeadLetterMessageCountDest.Enabled = false;
            textBoxAvilableStatusDest.Enabled = false;

            buttonClearMessages.Enabled = false;
            buttonClearMessagesDest.Enabled = false;
            buttonCreateMessage.Enabled = false;
            buttonCreateMessageDest.Enabled = false;
            buttonTransferMessages.Enabled = false;
            buttonReturnMessages.Enabled = false;

            textBoxConnectionString.Text = connectionString;

            treeSBView.TreeViewNodeSorter = new TreeViewHelper();

            //statusStripSB.TabIndex
            //statusStripSB.Text = "Waiting for connection";
            toolStripProgressBar.Visible = false;
            toolStripConnectionStatusLable.Text = "Waiting for connection";
            toolStripConnectionStatusImage.Image = imageListStatusStrip.Images[1];
            statusStripSB.Refresh();

            this.TimerThread = new Thread(new ThreadStart(this.StripTimerFunc));
            this.TimerThread.Start();

            OnCreateMessagesFinishes += new EventHandler(Form1_OnCreateMessagesFinishes);
            OnTransferMessagesFinishes += new EventHandler(Form1_OnTranferMessagesFinishes);
            OnCreateMessagesDestFinishes += new EventHandler(Form1_OnCreateMessagesDestFinishes);
            OnReturnMessagesFinishes += new EventHandler(Form1_OnReturnMessagesFinishes);

            if (connectionString.Contains("Endpoint=sb://"))
            {
                Connect();
            }

        }
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void Connect()
        { 
            treeSBView.Enabled = false;

            connectionString = "";
            connectionString = textBoxConnectionString.Text;
            if(connectionString == "")
            {
                MessageBox.Show("Please insert your servicebus connection string");                
                return;
            }
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 100;
            toolStripProgressBar.Step = 1;
            toolStripProgressBar.PerformStep();

            namespaceManager = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(connectionString);

            toolStripProgressBar.PerformStep();

            serviceBusHelper = new ServiceBusHelper();            

            var serviceBusNamespace = serviceBusHelper.GetServiceBusNamespace("Manual", connectionString);
            bool bFlag = serviceBusHelper.Connect(serviceBusNamespace);

            toolStripProgressBar.PerformStep();

            if (!bFlag)
            {
                toolStripConnectionStatusLable.Text = "Connection fail";
                toolStripConnectionStatusImage.Image = imageListStatusStrip.Images[4];
                statusStripSB.Refresh();

                return;
            }

            toolStripConnectionStatusLable.Text = "Connected";
            toolStripConnectionStatusImage.Image = imageListStatusStrip.Images[3];
            statusStripSB.Refresh();

            rootNode = treeSBView.Nodes.Add(serviceBusHelper.NamespaceUri.AbsoluteUri, serviceBusHelper.NamespaceUri.AbsoluteUri, AzureIconIndex, AzureIconIndex);

            toolStripProgressBar.PerformStep();

            GetEntities();

            for(int i = toolStripProgressBar.Value; i < toolStripProgressBar.Maximum;i++)
            {
                toolStripProgressBar.PerformStep();
            }

            toolStripProgressBar.Visible = false;
            toolStripConnectionStatusLable.Text = "Connected && Ready";
            toolStripConnectionStatusImage.Image = imageListStatusStrip.Images[2];
            statusStripSB.Refresh();

            treeSBView.Enabled = true;            
        }

        private void GetEntities()
        {
            var topicListNode = FindNode(TopicEntities, rootNode);
            var queueListNode = FindNode(QueueEntities, rootNode);

            try
            {
                if (serviceBusHelper != null)
                {
                    try
                    {
                        toolStripProgressBar.PerformStep();

                        var topics = serviceBusHelper.NamespaceManager.GetTopics(FilterExpressionHelper.TopicFilterExpression);
                        string strText = string.IsNullOrWhiteSpace(FilterExpressionHelper.TopicFilterExpression) ? TopicEntities : FilteredTopicEntities;

                        toolStripProgressBar.PerformStep();

                        if (topics != null)
                        {
                            topicListNode = rootNode.Nodes.Add(TopicEntities, "Topics", SubscriptionListIconIndex, SubscriptionListIconIndex);
                            foreach (var topic in topics)
                            {
                                toolStripProgressBar.PerformStep();

                                if (string.IsNullOrWhiteSpace(topic.Path))
                                {
                                    continue;
                                }
                                string strTopicPath = topic.Path;
                                TopicDescription TopicDesc = topic;

                                var TopicListNode = topicListNode.Nodes.Add(TopicEntities, strTopicPath, TopicListIconIndex, TopicListIconIndex);
                                
                                try
                                {
                                    toolStripProgressBar.PerformStep();
                                    var subscriptions = serviceBusHelper.GetSubscriptions(topic, FilterExpressionHelper.SubscriptionFilterExpression);
                                    var subscriptionDescriptions = subscriptions as IList<SubscriptionDescription> ?? subscriptions.ToList();
                                    if ((subscriptions != null && subscriptionDescriptions.Any()) || !string.IsNullOrWhiteSpace(FilterExpressionHelper.SubscriptionFilterExpression))
                                    {
                                        string strSubscriptionText = string.IsNullOrWhiteSpace(FilterExpressionHelper.SubscriptionFilterExpression) ? SubscriptionEntities : FilteredSubscriptionEntities;
                                        //queueListNode = TopicListNode.Nodes.Add("Subscriptions", "Subscriptions", SubscriptionListIconIndex, SubscriptionListIconIndex);

                                        foreach (var subscription in subscriptionDescriptions)
                                        {
                                            toolStripProgressBar.PerformStep();
                                            string strSubscriptionName = subscription.Name;

                                            TopicListNode.Nodes.Add(SubscriptionEntities, strSubscriptionName, SubscriptionIconIndex, SubscriptionIconIndex);

                                            //if (strSubscriptionName.Equals(_Subscription_A_Name) || strSubscriptionName.Equals(_Subscription_B_Name))
                                            {
                                                //GetQMessageCount(topic, subscription);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception: " + ex.Message);
                                }
                            }                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            //return 0;
        }

        private void GetEntitiesUniq(string strTopic,string strSubscription)
        {
            var topicListNode = FindNode(TopicEntities, rootNode);
            var queueListNode = FindNode(QueueEntities, rootNode);

            try
            {
                if (serviceBusHelper != null)
                {
                    try
                    {
                        var topics = serviceBusHelper.NamespaceManager.GetTopics(FilterExpressionHelper.TopicFilterExpression);
                        string strText = string.IsNullOrWhiteSpace(FilterExpressionHelper.TopicFilterExpression) ? TopicEntities : FilteredTopicEntities;

                        if (topics != null)
                        {                            
                            foreach (var topic in topics)
                            {
                                if (string.IsNullOrWhiteSpace(topic.Path))
                                {
                                    continue;
                                }
                                string strTopicPath = topic.Path;
                                TopicDescription TopicDesc = topic;
                                                         
                                try
                                {
                                    var subscriptions = serviceBusHelper.GetSubscriptions(topic, FilterExpressionHelper.SubscriptionFilterExpression);
                                    var subscriptionDescriptions = subscriptions as IList<SubscriptionDescription> ?? subscriptions.ToList();
                                    if ((subscriptions != null && subscriptionDescriptions.Any()) || !string.IsNullOrWhiteSpace(FilterExpressionHelper.SubscriptionFilterExpression))
                                    {
                                        string strSubscriptionText = string.IsNullOrWhiteSpace(FilterExpressionHelper.SubscriptionFilterExpression) ? SubscriptionEntities : FilteredSubscriptionEntities;                                

                                        foreach (var subscription in subscriptionDescriptions)
                                        {
                                            string strSubscriptionName = subscription.Name;
                                            
                                            if ((strSubscriptionName.Equals(strSubscription)) && (strTopicPath.Equals(strTopic)))
                                            {
                                                GetQMessageCount(topic, subscription);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception: " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            
        }

        public void GetQMessageCount(TopicDescription TopicDesc, SubscriptionDescription SubscriptionDesc)
        {
            SubscriptionWrapper SW = new SubscriptionWrapper(SubscriptionDesc, TopicDesc);
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

            string strlabelMaxDeliveryCount = namespaceManager.GetSubscription(TopicDesc.Path, SubscriptionDesc.Name).MaxDeliveryCount.ToString();

            String strStatus = namespaceManager.GetSubscription(TopicDesc.Path, SubscriptionDesc.Name).Status.ToString();
            string strlabelStatus = strStatus;
                        
            if (!checkBoxSelectAndLock.Checked)
            {
                textBoxMessageCount.Text = strlabelMessageCount;
                textBoxActiveMessageCount.Text = MsgCD.ActiveMessageCount.ToString();
                textBoxTopic.Text = SW.TopicDescription.Path;
                textBoxSubscription.Text = SW.SubscriptionDescription.Name;
                textBoxTransferMessageCount.Text = MsgCD.TransferMessageCount.ToString();
                textBoxAvilableStatus.Text = strAvilableStatus;
                textBoxDeadLetterMessageCount.Text = strlabelDeadLetterMessageCount;

                buttonClearMessages.Enabled = true;                
                buttonCreateMessage.Enabled = true;

                m_TopicDescSRC = TopicDesc;
                m_SubscriptionDescSRC = SubscriptionDesc;

            }
            else if (!checkBoxSelectAndLockDest.Checked)
            {
                textBoxMessageCountDest.Text = strlabelMessageCount;
                textBoxActiveMessageCountDest.Text = MsgCD.ActiveMessageCount.ToString();
                textBoxTopicDest.Text = SW.TopicDescription.Path;
                textBoxSubscriptionDest.Text = SW.SubscriptionDescription.Name;
                textBoxTransferMessageCountDest.Text = MsgCD.TransferMessageCount.ToString();
                textBoxAvilableStatusDest.Text = strAvilableStatus;
                textBoxDeadLetterMessageCountDest.Text = strlabelDeadLetterMessageCount;
                
                buttonClearMessagesDest.Enabled = true;
                buttonCreateMessageDest.Enabled = true;

                m_TopicDescTARGET = TopicDesc;
                m_SubscriptionDescTARGET = SubscriptionDesc;

            }
        }

        public void UpdateAllSrcControls(TopicDescription TopicDesC, SubscriptionDescription SubscriptionDesC,bool bFlagEnableControl = false)
        {
            SubscriptionWrapper SW = new SubscriptionWrapper(SubscriptionDesC, TopicDesC);
            long lngCount = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MessageCount;

            string strlabelMessageCount = lngCount.ToString();

            MessageCountDetails MsgCD = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MessageCountDetails;

            string strLockDuration = SW.SubscriptionDescription.LockDuration.ToString();
            string strAvilableStatus = SW.SubscriptionDescription.AvailabilityStatus.ToString();
            string strAccessedAt = SW.SubscriptionDescription.AccessedAt.ToString();

            string strlabelTransferMessageCount = MsgCD.TransferMessageCount.ToString();
            string strlabelActiveMessageCount = MsgCD.ActiveMessageCount.ToString();
            string strlabelScheduledMessageCount = MsgCD.ScheduledMessageCount.ToString();
            string strlabelDeadLetterMessageCount = MsgCD.DeadLetterMessageCount.ToString();

            string strlabelMaxDeliveryCount = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MaxDeliveryCount.ToString();

            String strStatus = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).Status.ToString();
            string strlabelStatus = strStatus;

            textBoxMessageCount.Text = strlabelMessageCount;
            textBoxActiveMessageCount.Text = MsgCD.ActiveMessageCount.ToString();
            textBoxTopic.Text = SW.TopicDescription.Path;
            textBoxSubscription.Text = SW.SubscriptionDescription.Name;
            textBoxTransferMessageCount.Text = MsgCD.TransferMessageCount.ToString();
            textBoxAvilableStatus.Text = strAvilableStatus;
            textBoxDeadLetterMessageCount.Text = strlabelDeadLetterMessageCount;

            if (bFlagEnableControl)
            {
                buttonClearMessages.Enabled = true;
                buttonCreateMessage.Enabled = true;
            }

            m_TopicDescSRC = TopicDesC;
            m_SubscriptionDescSRC = SubscriptionDesC;
        }

        public void UpdateAllDestControls(TopicDescription TopicDesC, SubscriptionDescription SubscriptionDesC, bool bFlagEnableControl = false)
        {
            SubscriptionWrapper SW = new SubscriptionWrapper(SubscriptionDesC, TopicDesC);
            long lngCount = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MessageCount;

            string strlabelMessageCount = lngCount.ToString();

            MessageCountDetails MsgCD = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MessageCountDetails;

            string strLockDuration = SW.SubscriptionDescription.LockDuration.ToString();
            string strAvilableStatus = SW.SubscriptionDescription.AvailabilityStatus.ToString();
            string strAccessedAt = SW.SubscriptionDescription.AccessedAt.ToString();

            string strlabelTransferMessageCount = MsgCD.TransferMessageCount.ToString();
            string strlabelActiveMessageCount = MsgCD.ActiveMessageCount.ToString();
            string strlabelScheduledMessageCount = MsgCD.ScheduledMessageCount.ToString();
            string strlabelDeadLetterMessageCount = MsgCD.DeadLetterMessageCount.ToString();

            string strlabelMaxDeliveryCount = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).MaxDeliveryCount.ToString();

            String strStatus = namespaceManager.GetSubscription(TopicDesC.Path, SubscriptionDesC.Name).Status.ToString();
            string strlabelStatus = strStatus;

            textBoxMessageCountDest.Text = strlabelMessageCount;
            textBoxActiveMessageCountDest.Text = MsgCD.ActiveMessageCount.ToString();
            textBoxTopicDest.Text = SW.TopicDescription.Path;
            textBoxSubscriptionDest.Text = SW.SubscriptionDescription.Name;
            textBoxTransferMessageCountDest.Text = MsgCD.TransferMessageCount.ToString();
            textBoxAvilableStatusDest.Text = strAvilableStatus;
            textBoxDeadLetterMessageCountDest.Text = strlabelDeadLetterMessageCount;

            if (bFlagEnableControl)
            {
                buttonClearMessagesDest.Enabled = true;
                buttonCreateMessageDest.Enabled = true;
            }

            m_TopicDescTARGET = TopicDesC;
            m_SubscriptionDescTARGET = SubscriptionDesC;
        }
        public void UpdateAllControl(TopicDescription TopicSrc, 
                                     SubscriptionDescription SubscriptionSrc,
                                     TopicDescription TopicDest, 
                                     SubscriptionDescription SubscriptionDest)
        {
            UpdateAllSrcControls(TopicSrc, SubscriptionSrc,true);
            UpdateAllDestControls(TopicDest, SubscriptionDest,true);
        }
        private void buttonCreateMessage_Click(object sender, EventArgs e)
        {
            textBox_InProc_MsgCreating.Text = "";
            textBox_InProc_MsgCreated.Text = "";
            textBox_InProc_AvrTime.Text = "";
            textBox_InProc_PredictToComplete.Text = "";
            textBoxRuningTime.Text = "";
            textBoxCompleteTime.Text = "";
            progressBarCreatingMessage.Value = 0;


            MessageForm MsgForm = new MessageForm();
            if(MsgForm.ShowDialog() == DialogResult.OK)
            {
                m_strSource_Message = MsgForm.strMessage;
                m_strSource_MessageProperty = MsgForm.strMessageProperty;
                m_strSource_MessageCount = MsgForm.strMessageCount;
                m_strSource_Topic = textBoxTopic.Text;
                m_strSource_Subscription = textBoxSubscription.Text;

                buttonCreateMessage.Enabled = false;
                buttonClearMessages.Enabled = false;
                buttonTransferMessages.Enabled = false;
                buttonReturnMessages.Enabled = false;
                buttonCreateMessageDest.Enabled = false;
                buttonClearMessagesDest.Enabled = false;
                treeSBView.Enabled = false;
                checkBoxSelectAndLock.Enabled = false;
                checkBoxSelectAndLockDest.Enabled = false;

                textBox_InProc_MsgCreating.Text = m_strSource_MessageCount;
                textBox_InProc_MsgCreated.Text = "0";

                int iCount = Convert.ToInt32(m_strSource_MessageCount);
                progressBarCreatingMessage.Step = 1;
                progressBarCreatingMessage.Maximum = iCount;

                i_TimeCreatingMessagesChart++;
                m_strCreateMessagesSeriesChart = "Run: " + i_TimeCreatingMessagesChart;
                chartSrcDelete.Series.Add(m_strCreateMessagesSeriesChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;  //SeriesChartType.;chartTransferAvrTime

                if (radioButtonSend.Checked)
                {
                    i_TimeCreateMsgSendMethodAvrChart++;
                    m_strCreateMsgSendAndAsyncSendChart = "Send: " + i_TimeCreateMsgSendMethodAvrChart;

                    chartSendAndAsyncSendAvrTime.Series.Add(m_strCreateMsgSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    chartSendAndAsyncSendAvrTime.Series.FindByName(m_strCreateMsgSendAndAsyncSendChart).Color = Color.Green;

                    //System.Windows.Forms.DataVisualization.Charting.DataPoint dt = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                    //dt.XValue = 100;
                    //dt.YValues = [ 100.4, 101.7];

                    //chartSendAndAsyncSendAvrTime.Series.FindByName(m_strCreateMsgSendAndAsyncSendChart).Points.Add(dt);
                }
                else if (radioButtonSendAsync.Checked)
                {
                    i_TimeCreateMsgAsyncSendMethodAvrChart++;
                    m_strCreateMsgSendAndAsyncSendChart = "AsyncSend: " + i_TimeCreateMsgAsyncSendMethodAvrChart;

                    chartSendAndAsyncSendAvrTime.Series.Add(m_strCreateMsgSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    chartSendAndAsyncSendAvrTime.Series.FindByName(m_strCreateMsgSendAndAsyncSendChart).Color = Color.Red;
                }

                this.CreateMessagesThread = new Thread(new ThreadStart(this.CreateMessages));
                this.CreateMessagesThread.Start();
                //CreateMessages();

                UpdateAllSrcControls(m_TopicDescSRC, m_SubscriptionDescSRC); // GetQMessageCount(m_TopicDescSRC ,m_SubscriptionDescSRC);
            }           

        }
        
        void Form1_OnCreateMessagesFinishes(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(Form1_OnCreateMessagesFinishesSafe), sender, e);
        }
        
        void Form1_OnCreateMessagesFinishesSafe(object sender, EventArgs e)
        {
            
            int iMsgCount = Convert.ToInt32(textBox_InProc_MsgCreated.Text);
            double iAvrTime = Convert.ToDouble(textBox_InProc_AvrTime.Text);
            double iRunCompleteTime = Convert.ToDouble(textBoxCompleteTime.Text);
            String strY = "Run: " + i_TimeCreatingMessagesChart.ToString();

            //chartSrcCreate.Series["Messages"].Points.AddXY(strY, iMsgCount);
            chartSrcCreate.Series["Time"].Points.AddXY(strY, iAvrTime);

            //chartMessagesAndCompleteTime.Series["Messages"].Points.AddXY(strY, iMsgCount);
            chartMessagesAndCompleteTime.Series["Time"].Points.AddXY(strY, iRunCompleteTime);
            

            UpdateAllSrcControls(m_TopicDescSRC, m_SubscriptionDescSRC);

            buttonCreateMessage.Enabled = true;
            buttonClearMessages.Enabled = true;             
            treeSBView.Enabled = true;
            checkBoxSelectAndLock.Enabled = true;
            checkBoxSelectAndLockDest.Enabled = true;

            if (textBoxTopicDest.Text != "")
            {
                buttonCreateMessageDest.Enabled = true;
                buttonClearMessagesDest.Enabled = true;
            }

            if ((checkBoxSelectAndLock.Checked) && (checkBoxSelectAndLockDest.Checked))
            {
                string strCount = textBoxActiveMessageCount.Text;
                int iCount = Convert.ToInt16(strCount);

                if(iCount > 0)
                {
                    buttonTransferMessages.Enabled = true;
                    buttonReturnMessages.Enabled = true;

                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.Text = "Ready To Transfer";
                }
                else
                {
                    richTextTransferBoxMessage.Text = "Cannot transfer messages to destenation becuase there are now waiting messages in the recource Subscription";
                }

            }

        }

        void Form1_OnTranferMessagesFinishes(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(Form1_OnTransferMessagesFinishesSafe), sender, e);
        }

        void Form1_OnTransferMessagesFinishesSafe(object sender, EventArgs e)
        {
            string strMsgCreated = textBox_InProc_MsgCreated.Text;
            if (strMsgCreated == "")
                return;

            int iMsgCount = Convert.ToInt32(textBox_InProc_MsgCreated.Text);
            double iCompleteTime = Convert.ToDouble(textBox_InProc_AvrTime.Text);
            String strY = "Transfer: " + i_TimeTransferMessagesChart.ToString();

            chartSrcTransfer.Series["Messages"].Points.AddXY(strY, iMsgCount);
            chartSrcTransfer.Series["Time"].Points.AddXY(strY, iCompleteTime);

            //UpdateAllDestControls(TopicDescSrc, SubscriptionDescSrc);
            UpdateAllControl(m_TopicDescSRC, m_SubscriptionDescSRC, m_TopicDescTARGET, m_SubscriptionDescTARGET);

            buttonCreateMessage.Enabled = true;
            buttonClearMessages.Enabled = true;
            //buttonTransferMessages.Enabled = true;
            //buttonReturnMessages.Enabled = true;
            buttonCreateMessageDest.Enabled = true;
            buttonClearMessagesDest.Enabled = true;
            treeSBView.Enabled = true;
            checkBoxSelectAndLock.Enabled = true;
            checkBoxSelectAndLockDest.Enabled = true;

            if ((checkBoxSelectAndLock.Checked) && (checkBoxSelectAndLockDest.Checked))
            {
                string strCount = textBoxActiveMessageCount.Text;
                int iCount = Convert.ToInt16(strCount);

                string strCountReturn = textBoxActiveMessageCountDest.Text;
                int iCountReturn = Convert.ToInt16(strCountReturn);

                if (iCount > 0)
                {
                    buttonTransferMessages.Enabled = true;
                }
                else
                {
                    buttonTransferMessages.Enabled = false;

                    richTextTransferBoxMessage.Clear();
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.AppendText("Transfer of messages process was succesful");
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.Invalidate();
                    this.Show();

                    //richTextTransferBoxMessage.ForeColor = Color.Red;
                    richTextTransferBoxMessage.AppendText("Please Notice:\nCannot transfer messages to destenation becuase there are no waiting messages in the recource Subscription.\nPlease create more messages in the source");
                    richTextTransferBoxMessage.ForeColor = Color.Red;
                }

                if(iCountReturn > 0)
                { 
                    buttonReturnMessages.Enabled = true;
                }
                else
                {
                    buttonReturnMessages.Enabled = false;

                    richTextTransferBoxMessage.Clear();
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.AppendText("Transfer of messages process was succesful");
                    richTextTransferBoxMessage.Invalidate();
                    this.Show();

                    richTextTransferBoxMessage.ForeColor = Color.Red;
                    richTextTransferBoxMessage.AppendText("Please Notice:\nCannot transfer messages to destenation becuase there are no waiting messages in the recource Subscription.\nPlease create more messages in the source");
                }
            }
        }

        public void CreateMessages()
        {
                        
            if (!namespaceManager.TopicExists(m_strSource_Topic))
            {
                namespaceManager.CreateTopic(m_strSource_Topic);
            }

            if (!namespaceManager.SubscriptionExists(m_strSource_Topic, m_strSource_Subscription))
            {                
                namespaceManager.CreateSubscription(m_strSource_Topic, m_strSource_Subscription);
            }

            if (!namespaceManager.SubscriptionExists("topic_a", "Subscription_A2"))
            {
                SqlFilter highMessagesFilter = new SqlFilter("MessageId <= 1000");
                namespaceManager.CreateSubscription(m_strSource_Topic, "Subscription_A2", highMessagesFilter);
            }
             

            TopicClient ClientHigh = TopicClient.CreateFromConnectionString(connectionString, m_strSource_Topic);

            int iCount = Convert.ToInt32(m_strSource_MessageCount);
            
            DateTime StartDT = DateTime.Now;
            TimeSpan TSMsgTotal = new TimeSpan();
            DateTime tPredict = new DateTime();// = null;

            int iProdit = -1;
            if (iCount > 500)
            {
                iProdit = iCount / 10;
            }
            
            for (int i = 0; i < iCount; i++)
            {
                DateTime bDT = DateTime.Now;

                // Create message, passing a string message for the body.
                BrokeredMessage message = new BrokeredMessage(m_strSource_Message + " " + i);

                // Set additional custom app-specific property.
                message.Properties[m_strSource_MessageProperty] = i;

                SEND_METHOD eSend = SEND_METHOD.SEND;
                // Send message to the topic.
                if (radioButtonSend.Checked)
                {
                    ClientHigh.Send(message);
                    eSend = SEND_METHOD.SEND;
                }
                else
                {
                    //Microsoft.ServiceBus.Messaging.BrokeredMessage msg = message;
                    //await SendAsync(msg, ClientHigh);
                    Task send2 = ClientHigh.SendAsync(message).ContinueWith((t) =>
                    {
                        Console.WriteLine("Sent message #2");
                        eSend = SEND_METHOD.ASYNC_SEND;
                    });
                    Task.WaitAll(send2);
                }
                
                DateTime eDT = DateTime.Now;
                TimeSpan TSMsg = eDT - bDT;
                                
                TimeSpan TSUntilNow = eDT - StartDT;
                TimeSpan MissedPredict;

                if (iProdit >= i)
                {
                    UpdateRuningTime(TSUntilNow.ToString(), Color.Green);
                }
                else
                {
                    MissedPredict = tPredict - eDT;
                    if (MissedPredict.TotalMilliseconds < 0)//(eDT.Millisecond < tPredict.Millisecond) //|| (tPredict != null))
                    {
                        UpdateRuningTime(TSUntilNow.ToString(), Color.Red);
                    }
                    else
                    {
                        UpdateRuningTime(TSUntilNow.ToString(), Color.Green);
                    }
                }

                TSMsgTotal += TSMsg;

                UpdateMsgCreated((i + 1).ToString());
                UpdateAvrTime(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));
                UpdateAvrChart(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));

                UpdateSendAndAsyncSendMethodAvrChart(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1), eSend);
                //UpdateSendMethodChart(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));
                //UpdateAsyncSendMethodChart(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));

                UpdateCreateMessagesProgressBar(1);

                if(iProdit == i)
                {
                    double ProdictTime = TSMsgTotal.TotalMilliseconds / iProdit;
                    double ProdictTimePerc = ProdictTime * 0.20;
                    ProdictTime += ProdictTimePerc;
                    double prodictTotalTime = (ProdictTime * iCount) - TSMsgTotal.TotalMilliseconds;

                    DateTime eProdict = StartDT;
                    tPredict = eProdict.AddMilliseconds(prodictTotalTime);
                    String strTime = BuildTime(tPredict);
                    UpdatePredictToComplete(strTime);
                                            
                }
            }

            DateTime EndDT = DateTime.Now;
            TimeSpan TotalTime = EndDT - StartDT;
            UpdateCompleteTime((TotalTime.TotalMilliseconds / 1000).ToString());

            double AvrTime = TSMsgTotal.TotalMilliseconds / iCount;
            double AvrTimeSec = AvrTime / 1000;
            UpdateAvrTime(AvrTimeSec.ToString(), AvrTimeSec, iCount.ToString(), iCount);

            if (OnCreateMessagesFinishes != null)
            {
                OnCreateMessagesFinishes(this, null);
            }
                
        }

        /*
        public async Task SendAsync<BrokeredMessage>(Microsoft.ServiceBus.Messaging.BrokeredMessage message, TopicClient ClientHigh)
        {            
            await ClientHigh.SendAsync(message);            
        }*/

        public string BuildTime(DateTime tPredict)
        {
            /*tPredict.Hour.ToString() + ":" +
            tPredict.Minute.ToString() + ":" +
            tPredict.Second.ToString() + ":" +
            tPredict.Millisecond.ToString());*/

            return (tPredict.ToString() + ":" + tPredict.Millisecond.ToString());
        }
        private void UpdateCreateMessagesProgressBar(int iStep)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.progressBarCreatingMessage.InvokeRequired)
            {
                ArgReturningVoidDelegateProgress d = new ArgReturningVoidDelegateProgress(UpdateCreateMessagesProgressBar);
                this.Invoke(d, new object[] { iStep });
            }
            else
            {
                for (int iIndex = 0; iIndex < iStep; iIndex++)
                {
                    this.progressBarCreatingMessage.PerformStep();
                }

            }
        }

        private void UpdateCreateMessagesProgressBarDest(int iStep)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.progressBarCreatingMessageDest.InvokeRequired)
            {
                ArgReturningVoidDelegateProgress d = new ArgReturningVoidDelegateProgress(UpdateCreateMessagesProgressBarDest);
                this.Invoke(d, new object[] { iStep });
            }
            else
            {
                for (int iIndex = 0; iIndex < iStep; iIndex++)
                {
                    this.progressBarCreatingMessageDest.PerformStep();
                }

            }
        }

        private void UpdateMsgCreated(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_MsgCreated.InvokeRequired)
            {
                ArgReturningVoidDelegateMsgCreated d = new ArgReturningVoidDelegateMsgCreated(UpdateMsgCreated);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_MsgCreated.Text = text;                
            }
        }
                
        private void UpdateMsgCreatedDest(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_MsgCreatedDest.InvokeRequired)
            {
                ArgReturningVoidDelegateMsgCreated d = new ArgReturningVoidDelegateMsgCreated(UpdateMsgCreatedDest);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_MsgCreatedDest.Text = text;
            }
        }

        private void UpdateAvrTime(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_AvrTime.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateAvrTime);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                this.textBox_InProc_AvrTime.Text = text;               
            }
        }

        private void UpdateAvrTimeDest(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_AvrTimeDest.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateAvrTimeDest);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                this.textBox_InProc_AvrTimeDest.Text = text;
            }
        }

        private void UpdateAvrChart(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.chartSrcDelete.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateAvrChart);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                double iAvrTime = AvrTimeSec;// Convert.ToInt32(text);
                int imsgIndex = iMsgIndex;// Convert.ToInt32(msgIndex);                

                chartSrcDelete.Series[m_strCreateMessagesSeriesChart].Points.AddXY(msgIndex, iAvrTime);                
            }
        }

        private void UpdateSendAndAsyncSendMethodAvrChart(string text, double AvrTimeSec, string msgIndex, int iMsgIndex, SEND_METHOD SendMethod)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.chartSendAndAsyncSendAvrTime.InvokeRequired)
            {
                ArgReturnVoidSendAndAsyncSendAvrTime d = new ArgReturnVoidSendAndAsyncSendAvrTime(UpdateSendAndAsyncSendMethodAvrChart);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex, SendMethod });
            }
            else
            {
                double iAvrTime = AvrTimeSec;// Convert.ToInt32(text);
                int imsgIndex = iMsgIndex;// Convert.ToInt32(msgIndex);                

                chartSendAndAsyncSendAvrTime.Series[m_strCreateMsgSendAndAsyncSendChart].Points.AddXY(msgIndex, iAvrTime);
            }
        }

        private void UpdateAvrChartDest(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.chartSrcDeleteDest.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateAvrChartDest);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                double iAvrTime = AvrTimeSec;// Convert.ToInt32(text);
                int imsgIndex = iMsgIndex;// Convert.ToInt32(msgIndex);

                //String strY = "Run: " + msgIndex.ToString();

                chartSrcDeleteDest.Series[m_strCreateMessagesDestSeriesChart].Points.AddXY(msgIndex, iAvrTime);
                //chartSrcDelete.Series["Time"].Points.AddXY(strY, iAvrTime);
            }
        }

        private void UpdateTransferAvrTime(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBoxTransferMessageAvrTime.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateTransferAvrTime);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                this.textBoxTransferMessageAvrTime.Text = text;                
            }
        }        

        private void UpdateTransferAvrChart(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.chartTransferAvrTime.InvokeRequired)
            {
                ArgReturningVoidDelegateAvrTime d = new ArgReturningVoidDelegateAvrTime(UpdateTransferAvrChart);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                double iAvrTime = AvrTimeSec;// Convert.ToInt32(text);
                int imsgIndex = iMsgIndex;// Convert.ToInt32(msgIndex);

                //String strY = "Transfer: " + msgIndex.ToString();
                
                chartTransferAvrTime.Series[m_strTransferMessagesSeriesChart].Points.AddXY(msgIndex, iAvrTime);
                //chartSrcDelete.Series["Time"].Points.AddXY(strY, iAvrTime);
            }
        }

        private void UpdateTransferSendAndAsyncSendAvrChart(string text, double AvrTimeSec, string msgIndex, int iMsgIndex)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.chartTransferSendAndAsyncSendAvrTime.InvokeRequired)
            {
                ArgReturningVoidTransferSendAndAsuncSendAvrTime d = new ArgReturningVoidTransferSendAndAsuncSendAvrTime(UpdateTransferSendAndAsyncSendAvrChart);
                this.Invoke(d, new object[] { text, AvrTimeSec, msgIndex, iMsgIndex });
            }
            else
            {
                double iAvrTime = AvrTimeSec;// Convert.ToInt32(text);
                int imsgIndex = iMsgIndex;// Convert.ToInt32(msgIndex);                
                
                chartTransferSendAndAsyncSendAvrTime.Series[m_strTransferMessagesSendAndAsyncSendChart].Points.AddXY(msgIndex, iAvrTime);
                //chartSrcDelete.Series["Time"].Points.AddXY(strY, iAvrTime);
            }
        }
        

        private void UpdateRuningTime(string text,Color clr)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBoxRuningTime.InvokeRequired)
            {
                ArgReturningVoidDelegateRuningTime d = new ArgReturningVoidDelegateRuningTime(UpdateRuningTime);
                this.Invoke(d, new object[] { text,clr });
            }
            else
            {
                this.textBoxRuningTime.ForeColor = clr;
                this.textBoxRuningTime.Text = text;
                //this.textBoxRuningTime.ForeColor = Color.Red;
                //this.textBoxRuningTime.
            }
        }

        private void UpdateRuningTimeDest(string text, Color clr)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBoxRuningTimeDest.InvokeRequired)
            {
                ArgReturningVoidDelegateRuningTime d = new ArgReturningVoidDelegateRuningTime(UpdateRuningTimeDest);
                this.Invoke(d, new object[] { text, clr });
            }
            else
            {
                this.textBoxRuningTimeDest.ForeColor = clr;
                this.textBoxRuningTimeDest.Text = text;
                //this.textBoxRuningTime.ForeColor = Color.Red;
                //this.textBoxRuningTime.
            }
        }

        private void UpdatePredictToComplete(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_PredictToComplete.InvokeRequired)
            {
                ArgReturningVoidDelegatePredictToComplete d = new ArgReturningVoidDelegatePredictToComplete(UpdatePredictToComplete);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_PredictToComplete.Text = text;
            }
        }

        private void UpdatePredictToCompleteDest(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_PredictToCompleteDest.InvokeRequired)
            {
                ArgReturningVoidDelegatePredictToComplete d = new ArgReturningVoidDelegatePredictToComplete(UpdatePredictToCompleteDest);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_PredictToCompleteDest.Text = text;
            }
        }

        private void UpdatePredictTransferMsgToComplete(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_PredictTransMsgToComplete.InvokeRequired)
            {
                ArgReturningVoidDelegatePredictToComplete d = new ArgReturningVoidDelegatePredictToComplete(UpdatePredictTransferMsgToComplete);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_PredictTransMsgToComplete.Text = text;
            }
        }

        private void UpdatePredictReturnMsgToComplete(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_PredictTransMsgToComplete.InvokeRequired)
            {
                ArgReturningVoidDelegatePredictToComplete d = new ArgReturningVoidDelegatePredictToComplete(UpdatePredictReturnMsgToComplete);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_PredictTransMsgToComplete.Text = text;
            }
        }

        private void UpdateCompleteTime(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_MsgCreated.InvokeRequired)
            {
                ArgReturningVoidDelegateCompleteTime d = new ArgReturningVoidDelegateCompleteTime(UpdateCompleteTime);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxCompleteTime.Text = text;
            }
        }

        private void UpdateCompleteTimeDest(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBox_InProc_MsgCreatedDest.InvokeRequired)
            {
                ArgReturningVoidDelegateCompleteTime d = new ArgReturningVoidDelegateCompleteTime(UpdateCompleteTimeDest);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxCompleteTimeDest.Text = text;
            }
        }        

        private void buttonClearMessages_Click(object sender, EventArgs e)
        {
            ClearMessagesForm CMsgForm = new ClearMessagesForm();
            CMsgForm.strTopic = textBoxTopic.Text;
            CMsgForm.strSubscription = textBoxSubscription.Text;
            CMsgForm.connectionString = connectionString;
            CMsgForm.TopicDesc = m_TopicDescSRC;
            CMsgForm.SubscriptionDesc = m_SubscriptionDescSRC;
            CMsgForm.serviceBusHelper = serviceBusHelper;
            CMsgForm.iMessageCount = Convert.ToInt32(textBoxActiveMessageCount.Text);


            if (CMsgForm.ShowDialog() == DialogResult.OK)
            {
                
            }

            UpdateAllSrcControls(m_TopicDescSRC, m_SubscriptionDescSRC); // GetQMessageCount(m_TopicDescSRC, m_SubscriptionDescSRC);

        }

        private void buttonTransferMessages_Click(object sender, EventArgs e)
        {
            m_strTopicSRC = textBoxTopic.Text;
            m_strTopicTARGET = textBoxTopicDest.Text;
            m_strSubscriptionSRC = textBoxSubscription.Text;
            m_strSubscriptionTRGET = textBoxSubscriptionDest.Text;
            m_iMessageCount = Convert.ToInt16(textBoxActiveMessageCount.Text);

            textBox_InProc_MsgTransferCreatingInDest.Text = textBoxActiveMessageCount.Text;
            //textBox_InProc_MsgCreatingDest

            progressBarTransfer.Maximum = m_iMessageCount;
            progressBarTransfer.Value = 0;
            progressBarTransfer.Step = 1;

            i_TimeTransferMessagesChart++;
            m_strTransferMessagesSeriesChart = "Transfer: " + i_TimeTransferMessagesChart;
            chartTransferAvrTime.Series.Add(m_strTransferMessagesSeriesChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            

            if(radioButtonSend.Checked)
            {
                i_TimeTransferMessagesSendAndAsyncSendChart++;
                m_strTransferMessagesSendAndAsyncSendChart = "Send Transfer: " + i_TimeTransferMessagesSendAndAsyncSendChart;

                chartTransferSendAndAsyncSendAvrTime.Series.Add(m_strTransferMessagesSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
                chartTransferSendAndAsyncSendAvrTime.Series.FindByName(m_strTransferMessagesSendAndAsyncSendChart).Color = Color.Green;
            }
            else if(radioButtonSendAsync.Checked)
            {
                i_TimeTransferMessagesSendAndAsyncSendChart++;
                m_strTransferMessagesSendAndAsyncSendChart = "AsyncSend Transfer: " + i_TimeTransferMessagesSendAndAsyncSendChart;

                chartTransferSendAndAsyncSendAvrTime.Series.Add(m_strTransferMessagesSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
                chartTransferSendAndAsyncSendAvrTime.Series.FindByName(m_strTransferMessagesSendAndAsyncSendChart).Color = Color.Red;
            }


            this.TransferMessagesThread = new Thread(new ThreadStart(this.TranferMessages));
            this.TransferMessagesThread.Start();
        }

        public class MyTransferMsg
        {
            public TopicClient m_TopicClient;
            public List<BrokeredMessage> m_BrokeredMessage = new List<BrokeredMessage>();
            public DateTime m_StartDateTime;
            public DateTime m_EndDateTime;
            public TimeSpan m_TS;

            public MyTransferMsg()
            {
                m_StartDateTime = DateTime.Now;
            }

            public TimeSpan GetEndMessageTime()
            {
                m_EndDateTime = DateTime.Now;
                m_TS = m_EndDateTime - m_StartDateTime;

                return m_TS;
            }
        }        

        public void TranferMessages()
        {            
            try
            {
                bool peek = false;
                bool all = true;
                
                int count;
                m_iTranferMsgCount = count = m_iMessageCount;                

                MessageReceiver messageReceiver;
                messageReceiver = serviceBusHelper.MessagingFactory.CreateMessageReceiver(SubscriptionClient.FormatSubscriptionPath(
                                                                                        m_strTopicSRC,
                                                                                        m_strSubscriptionSRC),
                                                                                        ReceiveMode.ReceiveAndDelete);
                                
                TopicClient ClientHigh = TopicClient.CreateFromConnectionString(connectionString, m_strTopicTARGET);
                
                var totalRetrieved = 0;

                m_TranferMsgTSMsgTotal = new TimeSpan();
                m_TranferMsg_StartDT = DateTime.Now;

                m_iTransferProdit = -1;
                if (m_iTranferMsgCount > 100)
                {
                    m_iTransferProdit = m_iTranferMsgCount / 10;
                }

                int retrieved;

                do
                {
                    var brokeredMessages = new List<BrokeredMessage>();
                    MyTransferMsg MTMsg = new MyTransferMsg();
                    var messages = messageReceiver.ReceiveBatch(10, TimeSpan.FromSeconds(1));
                    var enumerable = messages as BrokeredMessage[] ?? messages.ToArray();
                    retrieved = enumerable.Count();
                    
                    if (retrieved == 0)
                    {
                        continue;
                    }
                    totalRetrieved += retrieved;
                    
                    brokeredMessages.AddRange(enumerable);                                       

                    MTMsg.m_TopicClient = ClientHigh;
                    MTMsg.m_BrokeredMessage = brokeredMessages;
                    

                    Thread t = new Thread(new ParameterizedThreadStart(TranferSendMessages));
                    t.Start(MTMsg);                   

                } while (retrieved > 0 && (all || count > totalRetrieved));
                               
            }
            catch (TimeoutException ex)
            {
                int x = 0;//SetLogText("TimeoutException: " + ex.Message);
            }
            catch (NotSupportedException ex)
            {
                int x = 0;//SetLogText("NotSupportedException: " + ex.Message);//ReadMessagesOneAtTheTime(peek, all, count, messageInspector);
            }
            catch (Exception ex)
            {
                int x = 0;//SetLogText("Exception: " + ex.Message);//HandleException(ex);
            }
            finally
            {
                int x = 0;                
            }

            if (OnTransferMessagesFinishes != null)
            {
                OnTransferMessagesFinishes(this, null);
            }
        }

        public void TranferSendMessages(Object OSend)
        {
            m_totalTransferMsgThreads++;
            UpdateTransferMessagesRuningThreads(m_totalTransferMsgThreads.ToString());

            var MTMsg = OSend as MyTransferMsg;
            foreach (BrokeredMessage BMsg in MTMsg.m_BrokeredMessage) //for (int iIndex = 0;iIndex < retrieved; iIndex++)
            {
                MTMsg.m_TopicClient.Send(BMsg);
                m_totalTransferSend++;

                TimeSpan TS = MTMsg.GetEndMessageTime();
                m_TranferMsgTSMsgTotal += TS;
                UpdateTransferAvrTime(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalTransferSend.ToString(), m_totalTransferSend);
                UpdateTransferAvrChart(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalTransferSend.ToString(), m_totalTransferSend);
                UpdateTransferSendAndAsyncSendAvrChart(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalTransferSend.ToString(), m_totalTransferSend);
                UpdateTransferMessagesCreatedDestComplete(m_totalTransferSend.ToString());
                UpdateTransferMessagesProgress(m_totalTransferSend.ToString());

                
            }

            if (m_iTransferProdit == m_totalTransferSend)
            {
                double ProdictTime = m_TranferMsgTSMsgTotal.TotalMilliseconds / m_iTransferProdit;
                double ProdictTimePerc = ProdictTime / 10;
                ProdictTime += ProdictTimePerc;
                double prodictTotalTime = (ProdictTime * m_iTranferMsgCount) - m_TranferMsgTSMsgTotal.TotalMilliseconds;

                DateTime eProdict = m_TranferMsg_StartDT;
                m_TranferMsgDTPredict = eProdict.AddMilliseconds(prodictTotalTime);
                String strTime = BuildTime(m_TranferMsgDTPredict);
                UpdatePredictTransferMsgToComplete(strTime);
            }

            m_totalTransferMsgThreads--;
            UpdateTransferMessagesRuningThreads(m_totalTransferMsgThreads.ToString());
        }

        //textBox_InProc_MsgTransferCreatingInDest
        private void UpdateTransferMessagesCreatedDestComplete(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 
                     
            if (this.textBox_InProc_MsgTransherCreatedInDest.InvokeRequired)
            {
                ArgReturningVoidDelegateTransferMessageDestComplete d = new ArgReturningVoidDelegateTransferMessageDestComplete(UpdateTransferMessagesCreatedDestComplete);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox_InProc_MsgTransherCreatedInDest.Text = text;                
            }
        }

        private void UpdateTransferMessagesProgress(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.progressBarTransfer.InvokeRequired)
            {
                ArgReturningVoidDelegateTransferMessageDestProgress d = new ArgReturningVoidDelegateTransferMessageDestProgress(UpdateTransferMessagesProgress);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.progressBarTransfer.PerformStep();
            }
        }

        

        private void UpdateTransferMessagesRuningThreads(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBoxRuningThreads.InvokeRequired)
            {
                ArgReturningVoidDelegateRuningThreads d = new ArgReturningVoidDelegateRuningThreads(UpdateTransferMessagesRuningThreads);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxRuningThreads.Text = text;
            }
        }

        private void UpdateReturnMessagesRuningThreads(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true. 

            if (this.textBoxRuningThreads.InvokeRequired)
            {
                ArgReturningVoidDelegateRuningThreads d = new ArgReturningVoidDelegateRuningThreads(UpdateReturnMessagesRuningThreads);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBoxRuningThreads.Text = text;
            }
        }


        private void buttonReturnMessages_Click(object sender, EventArgs e)
        {
            m_strTopicSRC = textBoxTopicDest.Text;
            m_strTopicTARGET = textBoxTopic.Text;
            m_strSubscriptionSRC = textBoxSubscriptionDest.Text;
            m_strSubscriptionTRGET = textBoxSubscription.Text;
            m_iMessageCount = Convert.ToInt16(textBoxActiveMessageCountDest.Text);

            textBox_InProc_MsgTransferCreatingInDest.Text = textBoxActiveMessageCountDest.Text;
            //textBox_InProc_MsgCreatingDest

            progressBarTransfer.Maximum = m_iMessageCount;
            progressBarTransfer.Value = 0;
            progressBarTransfer.Step = 1;

            i_TimeReturnMessagesChart++;
            m_strReturnMessagesSeriesChart = "Return: " + i_TimeReturnMessagesChart;
            chartTransferAvrTime.Series.Add(m_strReturnMessagesSeriesChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;


            if (radioButtonSend.Checked)
            {
                i_TimeReturnMessagesSendAndAsyncSendChart++;
                m_strReturnMessagesSendAndAsyncSendChart = "Send Return: " + i_TimeReturnMessagesSendAndAsyncSendChart;

                chartTransferSendAndAsyncSendAvrTime.Series.Add(m_strReturnMessagesSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
                chartTransferSendAndAsyncSendAvrTime.Series.FindByName(m_strReturnMessagesSendAndAsyncSendChart).Color = Color.Green;
            }
            else if (radioButtonSendAsync.Checked)
            {
                i_TimeReturnMessagesSendAndAsyncSendChart++;
                m_strReturnMessagesSendAndAsyncSendChart = "AsyncSend Return: " + i_TimeReturnMessagesSendAndAsyncSendChart;

                chartTransferSendAndAsyncSendAvrTime.Series.Add(m_strReturnMessagesSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
                chartTransferSendAndAsyncSendAvrTime.Series.Add(m_strReturnMessagesSendAndAsyncSendChart).Color = Color.Red;
            }


            //this.ReturnMessagesThread = new Thread(new ThreadStart(this.ReturnMessages));
            //this.ReturnMessagesThread.Start();

            this.TransferMessagesThread = new Thread(new ThreadStart(this.TranferMessages));
            this.TransferMessagesThread.Start();
        }

        public void ReturnMessages()
        {
            try
            {
                bool peek = false;
                bool all = true;

                int count;
                m_iReturnMsgCount = count = m_iMessageCount;

                MessageReceiver messageReceiver;
                messageReceiver = serviceBusHelper.MessagingFactory.CreateMessageReceiver(SubscriptionClient.FormatSubscriptionPath(
                                                                                        m_strTopicSRC,
                                                                                        m_strSubscriptionSRC),
                                                                                        ReceiveMode.ReceiveAndDelete);

                TopicClient ClientHigh = TopicClient.CreateFromConnectionString(connectionString, m_strTopicTARGET);

                var totalRetrieved = 0;

                m_ReturnMsgTSMsgTotal = new TimeSpan();
                m_ReturnMsg_StartDT = DateTime.Now;

                m_iReturnProdit = -1;
                if (m_iReturnMsgCount > 100)
                {
                    m_iReturnProdit = m_iReturnMsgCount / 10;
                }

                int retrieved;

                do
                {
                    var brokeredMessages = new List<BrokeredMessage>();
                    MyTransferMsg MTMsg = new MyTransferMsg();
                    var messages = messageReceiver.ReceiveBatch(10, TimeSpan.FromSeconds(1));
                    var enumerable = messages as BrokeredMessage[] ?? messages.ToArray();
                    retrieved = enumerable.Count();

                    if (retrieved == 0)
                    {
                        continue;
                    }
                    totalRetrieved += retrieved;

                    brokeredMessages.AddRange(enumerable);

                    MTMsg.m_TopicClient = ClientHigh;
                    MTMsg.m_BrokeredMessage = brokeredMessages;

                    Thread t = new Thread(new ParameterizedThreadStart(ReturnSendMessages));
                    t.Start(MTMsg);

                } while (retrieved > 0 && (all || count > totalRetrieved));

            }
            catch (TimeoutException ex)
            {
                int x = 0;//SetLogText("TimeoutException: " + ex.Message);
            }
            catch (NotSupportedException ex)
            {
                int x = 0;//SetLogText("NotSupportedException: " + ex.Message);//ReadMessagesOneAtTheTime(peek, all, count, messageInspector);
            }
            catch (Exception ex)
            {
                int x = 0;//SetLogText("Exception: " + ex.Message);//HandleException(ex);
            }
            finally
            {
                int x = 0;
            }

            if (OnReturnMessagesFinishes != null)
            {
                OnReturnMessagesFinishes(this, null);
            }
        }

        public void ReturnSendMessages(Object OSend)
        {
            m_totalReturnMsgThreads++;
            UpdateReturnMessagesRuningThreads(m_totalReturnMsgThreads.ToString());

            var MTMsg = OSend as MyTransferMsg;
            foreach (BrokeredMessage BMsg in MTMsg.m_BrokeredMessage) //for (int iIndex = 0;iIndex < retrieved; iIndex++)
            {
                /*BrokeredMessage message = new BrokeredMessage(m_strSource_Message + " " + 1);

                // Set additional custom app-specific property.
                message.Properties[m_strSource_MessageProperty] = 1;

                // Send message to the topic.
                MTMsg.m_TopicClient.Send(message);*/
                //BrokeredMessage bMess = new BrokeredMessage();
                //bMess = BMsg;

                //var ct = bMessage.ContentType;
                //Type bodyType = Type.GetType(ct, true);

                BrokeredMessage message = new BrokeredMessage(BMsg.GetBody<string>());
                message.Properties["MessageId"] = 1;

                MTMsg.m_TopicClient.Send(message);// BMsg);
                m_totalReturnSend++;

                TimeSpan TS = MTMsg.GetEndMessageTime();
                m_ReturnMsgTSMsgTotal += TS;
                UpdateTransferAvrTime(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalReturnSend.ToString(), m_totalReturnSend);
                UpdateTransferAvrChart(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalReturnSend.ToString(), m_totalReturnSend);
                UpdateTransferSendAndAsyncSendAvrChart(TS.TotalMilliseconds.ToString(), TS.TotalMilliseconds, m_totalReturnSend.ToString(), m_totalReturnSend);
                UpdateTransferMessagesCreatedDestComplete(m_totalReturnSend.ToString());
                UpdateTransferMessagesProgress(m_totalReturnSend.ToString());


            }

            if (m_iReturnProdit == m_totalReturnSend)
            {
                double ProdictTime = m_ReturnMsgTSMsgTotal.TotalMilliseconds / m_iReturnProdit;
                double ProdictTimePerc = ProdictTime / 10;
                ProdictTime += ProdictTimePerc;
                double prodictTotalTime = (ProdictTime * m_iReturnMsgCount) - m_ReturnMsgTSMsgTotal.TotalMilliseconds;

                DateTime eProdict = m_ReturnMsg_StartDT;
                m_ReturnMsgDTPredict = eProdict.AddMilliseconds(prodictTotalTime);
                String strTime = BuildTime(m_ReturnMsgDTPredict);
                UpdatePredictReturnMsgToComplete(strTime);
            }

            m_totalReturnMsgThreads--;
            UpdateReturnMessagesRuningThreads(m_totalReturnMsgThreads.ToString());
        }

        void Form1_OnReturnMessagesFinishes(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(Form1_OnReturnMessagesFinishesSafe), sender, e);
        }

        void Form1_OnReturnMessagesFinishesSafe(object sender, EventArgs e)
        {
            return;

            int iMsgCount = Convert.ToInt32(textBox_InProc_MsgCreated.Text);
            double iCompleteTime = Convert.ToDouble(textBox_InProc_AvrTime.Text);
            String strY = "Transfer: " + i_TimeTransferMessagesChart.ToString();

            chartSrcTransfer.Series["Messages"].Points.AddXY(strY, iMsgCount);
            chartSrcTransfer.Series["Time"].Points.AddXY(strY, iCompleteTime);

            //UpdateAllDestControls(TopicDescSrc, SubscriptionDescSrc);
            UpdateAllControl(m_TopicDescSRC, m_SubscriptionDescSRC, m_TopicDescTARGET, m_SubscriptionDescTARGET);

            buttonCreateMessage.Enabled = true;
            buttonClearMessages.Enabled = true;
            //buttonTransferMessages.Enabled = true;
            //buttonReturnMessages.Enabled = true;
            buttonCreateMessageDest.Enabled = true;
            buttonClearMessagesDest.Enabled = true;
            treeSBView.Enabled = true;
            checkBoxSelectAndLock.Enabled = true;
            checkBoxSelectAndLockDest.Enabled = true;

            if ((checkBoxSelectAndLock.Checked) && (checkBoxSelectAndLockDest.Checked))
            {
                string strCount = textBoxActiveMessageCount.Text;
                int iCount = Convert.ToInt16(strCount);

                string strCountReturn = textBoxActiveMessageCountDest.Text;
                int iCountReturn = Convert.ToInt16(strCountReturn);

                if (iCount > 0)
                {
                    buttonTransferMessages.Enabled = true;
                }
                else
                {
                    buttonTransferMessages.Enabled = false;

                    richTextTransferBoxMessage.Clear();
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.AppendText("Transfer of messages process was succesful");
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.Invalidate();
                    this.Show();

                    //richTextTransferBoxMessage.ForeColor = Color.Red;
                    richTextTransferBoxMessage.AppendText("Please Notice:\nCannot transfer messages to destenation becuase there are no waiting messages in the recource Subscription.\nPlease create more messages in the source");
                    richTextTransferBoxMessage.ForeColor = Color.Red;
                }

                if (iCountReturn > 0)
                {
                    buttonReturnMessages.Enabled = true;
                }
                else
                {
                    buttonReturnMessages.Enabled = false;

                    richTextTransferBoxMessage.Clear();
                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.AppendText("Transfer of messages process was succesful");
                    richTextTransferBoxMessage.Invalidate();
                    this.Show();

                    richTextTransferBoxMessage.ForeColor = Color.Red;
                    richTextTransferBoxMessage.AppendText("Please Notice:\nCannot transfer messages to destenation becuase there are no waiting messages in the recource Subscription.\nPlease create more messages in the source");
                }
            }
        }

        private void buttonCreateMessageDest_Click(object sender, EventArgs e)
        {
            textBox_InProc_MsgCreatingDest.Text = "";
            textBox_InProc_MsgCreatedDest.Text = "";
            textBox_InProc_AvrTimeDest.Text = "";
            textBox_InProc_PredictToCompleteDest.Text = "";
            textBoxRuningTimeDest.Text = "";
            textBoxCompleteTimeDest.Text = "";
            progressBarCreatingMessageDest.Value = 0;


            MessageForm MsgForm = new MessageForm();
            if (MsgForm.ShowDialog() == DialogResult.OK)
            {
                m_strSource_MessageDest = MsgForm.strMessage;
                m_strSource_MessagePropertyDest = MsgForm.strMessageProperty;
                m_strSource_MessageCountDest = MsgForm.strMessageCount;
                m_strSource_TopicDest = textBoxTopicDest.Text;
                m_strSource_SubscriptionDest = textBoxSubscriptionDest.Text;

                buttonCreateMessage.Enabled = false;
                buttonClearMessages.Enabled = false;
                buttonTransferMessages.Enabled = false;
                buttonReturnMessages.Enabled = false;
                buttonCreateMessageDest.Enabled = false;
                buttonClearMessagesDest.Enabled = false;
                treeSBView.Enabled = false;
                checkBoxSelectAndLock.Enabled = false;
                checkBoxSelectAndLockDest.Enabled = false;

                textBox_InProc_MsgCreatingDest.Text = m_strSource_MessageCountDest;
                textBox_InProc_MsgCreatedDest.Text = "0";

                int iCount = Convert.ToInt32(m_strSource_MessageCountDest);
                progressBarCreatingMessageDest.Step = 1;
                progressBarCreatingMessageDest.Maximum = iCount;

                i_TimeCreatingMessagesDestChart++;
                m_strCreateMessagesDestSeriesChart = "Run: " + i_TimeCreatingMessagesDestChart;
                chartSrcDeleteDest.Series.Add(m_strCreateMessagesDestSeriesChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;  //SeriesChartType.;chartTransferAvrTime

                if(radioButtonSend.Checked)
                {
                    i_TimeCreateMsgSendMethodAvrChart++;
                    m_strCreateMsgSendAndAsyncSendChart = "Send: " + i_TimeCreateMsgSendMethodAvrChart;

                    chartSendAndAsyncSendAvrTime.Series.Add(m_strCreateMsgSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    chartSendAndAsyncSendAvrTime.Series.FindByName(m_strCreateMsgSendAndAsyncSendChart).Color = Color.Green;
                }
                else if(radioButtonSendAsync.Checked)
                {
                    i_TimeCreateMsgAsyncSendMethodAvrChart++;
                    m_strCreateMsgSendAndAsyncSendChart = "AsyncSend: " + i_TimeCreateMsgSendMethodAvrChart;

                    chartSendAndAsyncSendAvrTime.Series.Add(m_strCreateMsgSendAndAsyncSendChart).ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                    chartSendAndAsyncSendAvrTime.Series.FindByName(m_strCreateMsgSendAndAsyncSendChart).Color = Color.Red;
                }
                
                

                this.CreateMessagesDestThread = new Thread(new ThreadStart(this.CreateMessagesDest));
                this.CreateMessagesDestThread.Start();
                //CreateMessages();

                UpdateAllDestControls(m_TopicDescTARGET, m_SubscriptionDescTARGET); //GetQMessageCount(m_TopicDescSRC, m_SubscriptionDescSRC);
            }
        }

        public void CreateMessagesDest()
        {

            if (!namespaceManager.TopicExists(m_strSource_TopicDest))
            {
                namespaceManager.CreateTopic(m_strSource_TopicDest);
            }

            if (!namespaceManager.SubscriptionExists(m_strSource_TopicDest, m_strSource_SubscriptionDest))
            {
                namespaceManager.CreateSubscription(m_strSource_TopicDest, m_strSource_SubscriptionDest);
            }

            TopicClient ClientHigh = TopicClient.CreateFromConnectionString(connectionString, m_strSource_TopicDest);

            int iCount = Convert.ToInt32(m_strSource_MessageCountDest);

            DateTime StartDT = DateTime.Now;
            TimeSpan TSMsgTotal = new TimeSpan();
            DateTime tPredict = new DateTime();// = null;

            int iProdit = -1;
            if (iCount > 500)
            {
                iProdit = iCount / 10;
            }

            for (int i = 0; i < iCount; i++)
            {
                DateTime bDT = DateTime.Now;

                // Create message, passing a string message for the body.
                BrokeredMessage message = new BrokeredMessage(m_strSource_MessageDest + " " + i);

                // Set additional custom app-specific property.
                message.Properties[m_strSource_MessagePropertyDest] = i;

                // Send message to the topic.
                SEND_METHOD eSend = SEND_METHOD.SEND;
                // Send message to the topic.
                if (radioButtonSend.Checked)
                {
                    ClientHigh.Send(message);
                    eSend = SEND_METHOD.SEND;
                }
                else
                {
                    //Microsoft.ServiceBus.Messaging.BrokeredMessage msg = message;
                    //await SendAsync(msg, ClientHigh);
                    Task send2 = ClientHigh.SendAsync(message).ContinueWith((t) =>
                    {
                        Console.WriteLine("Sent message #2");
                        eSend = SEND_METHOD.ASYNC_SEND;
                    });
                    Task.WaitAll(send2);
                }

                DateTime eDT = DateTime.Now;
                TimeSpan TSMsg = eDT - bDT;

                TimeSpan TSUntilNow = eDT - StartDT;
                TimeSpan MissedPredict;

                if (iProdit >= i)
                {
                    UpdateRuningTimeDest(TSUntilNow.ToString(), Color.Green);
                }
                else
                {
                    MissedPredict = tPredict - eDT;
                    if (MissedPredict.TotalMilliseconds < 0)//(eDT.Millisecond < tPredict.Millisecond) //|| (tPredict != null))
                    {
                        UpdateRuningTimeDest(TSUntilNow.ToString(), Color.Red);
                    }
                    else
                    {
                        UpdateRuningTimeDest(TSUntilNow.ToString(), Color.Green);
                    }
                }

                TSMsgTotal += TSMsg;

                UpdateMsgCreatedDest((i + 1).ToString());
                UpdateAvrTimeDest(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));
                UpdateAvrChartDest(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1));
                UpdateSendAndAsyncSendMethodAvrChart(TSMsg.TotalMilliseconds.ToString(), TSMsg.TotalMilliseconds, (i + 1).ToString(), (i + 1), eSend);

                UpdateCreateMessagesProgressBarDest(1);

                if (iProdit == i)
                {
                    double ProdictTime = TSMsgTotal.TotalMilliseconds / iProdit;
                    double ProdictTimePerc = ProdictTime / 10;
                    ProdictTime += ProdictTimePerc;
                    double prodictTotalTime = (ProdictTime * iCount) - TSMsgTotal.TotalMilliseconds;

                    DateTime eProdict = StartDT;
                    tPredict = eProdict.AddMilliseconds(prodictTotalTime);
                    String strTime = BuildTime(tPredict);
                    UpdatePredictToCompleteDest(strTime);

                }
            }

            DateTime EndDT = DateTime.Now;
            TimeSpan TotalTime = EndDT - StartDT;
            UpdateCompleteTimeDest((TotalTime.TotalMilliseconds / 1000).ToString());

            double AvrTime = TSMsgTotal.TotalMilliseconds / iCount;
            double AvrTimeSec = AvrTime / 1000;
            UpdateAvrTimeDest(AvrTimeSec.ToString(), AvrTimeSec, iCount.ToString(), iCount);

            if (OnCreateMessagesDestFinishes != null)
            {
                OnCreateMessagesDestFinishes(this, null);
            }

        }

        void Form1_OnCreateMessagesDestFinishes(object sender, EventArgs e)
        {
            this.Invoke(new EventHandler(Form1_OnCreateMessagesDestFinishesSafe), sender, e);
        }

        void Form1_OnCreateMessagesDestFinishesSafe(object sender, EventArgs e)
        {

            int iMsgCount = Convert.ToInt32(textBox_InProc_MsgCreatedDest.Text);
            double iCompleteTime = Convert.ToDouble(textBox_InProc_AvrTimeDest.Text);
            String strY = "Run: " + i_TimeCreatingMessagesDestChart.ToString();

            chartSrcCreateDest.Series["Messages"].Points.AddXY(strY, iMsgCount);
            chartSrcCreateDest.Series["Time"].Points.AddXY(strY, iCompleteTime);

            UpdateAllDestControls(m_TopicDescTARGET, m_SubscriptionDescTARGET);

            buttonCreateMessage.Enabled = true;
            buttonClearMessages.Enabled = true;
            buttonCreateMessageDest.Enabled = true;
            buttonClearMessagesDest.Enabled = true;
            treeSBView.Enabled = true;
            checkBoxSelectAndLock.Enabled = true;
            checkBoxSelectAndLockDest.Enabled = true;

            if ((checkBoxSelectAndLock.Checked) && (checkBoxSelectAndLockDest.Checked))
            {
                string strCount = textBoxActiveMessageCount.Text;
                int iCount = Convert.ToInt16(strCount);

                if (iCount > 0)
                {
                    buttonTransferMessages.Enabled = true;
                    buttonReturnMessages.Enabled = true;

                    richTextTransferBoxMessage.ForeColor = Color.Green;
                    richTextTransferBoxMessage.Text = "Ready To Transfer";
                }
                else
                {
                    richTextTransferBoxMessage.Text = "Cannot transfer messages to destenation becuase there are now waiting messages in the recource Subscription";
                }

            }

        }

        private void buttonClearMessagesDest_Click(object sender, EventArgs e)
        {
            ClearMessagesForm CMsgForm = new ClearMessagesForm();
            CMsgForm.strTopic = textBoxTopicDest.Text;
            CMsgForm.strSubscription = textBoxSubscriptionDest.Text;
            CMsgForm.connectionString = connectionString;
            CMsgForm.TopicDesc = m_TopicDescTARGET;
            CMsgForm.SubscriptionDesc = m_SubscriptionDescTARGET;
            CMsgForm.serviceBusHelper = serviceBusHelper;
            CMsgForm.iMessageCount = Convert.ToInt32(textBoxActiveMessageCountDest.Text);


            if (CMsgForm.ShowDialog() == DialogResult.OK)
            {

            }

            UpdateAllDestControls(m_TopicDescTARGET, m_SubscriptionDescTARGET);
        }

        private void checkBoxSelectAndLock_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSelectAndLock.Checked)
            {
                /*textBoxTopic.Enabled = false;
                textBoxSubscription.Enabled = false;
                textBoxMessageCount.Enabled = false;
                textBoxActiveMessageCount.Enabled = false;
                textBoxTransferMessageCount.Enabled = false;
                textBoxDeadLetterMessageCount.Enabled = false;
                textBoxAvilableStatus.Enabled = false;*/

                if (checkBoxSelectAndLockDest.Checked)
                {
                    buttonTransferMessages.Enabled = true;
                    buttonReturnMessages.Enabled = true;
                }
            }
            else
            {
                /*
                textBoxTopic.Enabled = true;
                textBoxSubscription.Enabled = true;
                textBoxMessageCount.Enabled = true;
                textBoxActiveMessageCount.Enabled = true;
                textBoxTransferMessageCount.Enabled = true;
                textBoxDeadLetterMessageCount.Enabled = true;
                textBoxAvilableStatus.Enabled = true;*/

                buttonTransferMessages.Enabled = false;
                buttonReturnMessages.Enabled = false;
            }
        }

        private void checkBoxSelectAndLockDest_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSelectAndLockDest.Checked)
            {
                /*textBoxTopicDest.Enabled = false;
                textBoxSubscriptionDest.Enabled = false;
                textBoxMessageCountDest.Enabled = false;
                textBoxActiveMessageCountDest.Enabled = false;
                textBoxTransferMessageCountDest.Enabled = false;
                textBoxDeadLetterMessageCountDest.Enabled = false;
                textBoxAvilableStatusDest.Enabled = false;*/

                if (checkBoxSelectAndLock.Checked)
                {
                    buttonTransferMessages.Enabled = true;
                    buttonReturnMessages.Enabled = true;
                }
            }
            else
            {
                /*textBoxTopicDest.Enabled = true;
                textBoxSubscriptionDest.Enabled = true;
                textBoxMessageCountDest.Enabled = true;
                textBoxActiveMessageCountDest.Enabled = true;
                textBoxTransferMessageCountDest.Enabled = true;
                textBoxDeadLetterMessageCountDest.Enabled = true;
                textBoxAvilableStatusDest.Enabled = true;*/

                buttonTransferMessages.Enabled = false;
                buttonReturnMessages.Enabled = false;
            }
        }

        private TreeNode FindNode(string path, TreeNode node)
        {
            if (string.IsNullOrWhiteSpace(path) || node == null)
            {
                return null;
            }
            var segments = path.Split('/');
            if (segments.Length > 1)
            {
                var index = path.IndexOf('/');
                if (index >= 0 &&
                    !string.IsNullOrWhiteSpace(segments[0]) &&
                    node.Nodes.ContainsKey(segments[0]))
                {
                    var entityNode = node.Nodes[segments[0]];
                    return FindNode(path.Substring(index + 1), entityNode);
                }
            }
            else
            {
                if (node.Nodes.ContainsKey(path))
                {
                    return node.Nodes[path];
                }
            }
            return null;
        }

        private void treeSBView_MouseClick(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo info = treeSBView.HitTest(treeSBView.PointToClient(Cursor.Position));
            if (info != null)
            {
                TreeNode node = info.Node;

                if (node == null)
                {
                    return;
                }
                //currentNode = node;
                //treeViewControl.SuspendDrawing();
                //treeSBView.SuspendLayout();
                //treeSBView.BeginUpdate();
                var queueListNode = FindNode(QueueEntities, rootNode);
                var topicListNode = FindNode(TopicEntities, rootNode);
                //var eventHubListNode = FindNode(EventHubEntities, rootNode);
                //var notificationHubListNode = FindNode(NotificationHubEntities, rootNode);
                //var relayServiceListNode = FindNode(RelayEntities, rootNode);
                //var menuItem = createIoTHubListenerMenuItem;
                //actionsToolStripMenuItem.DropDownItems.Clear();
                //actionsToolStripMenuItem.DropDownItems.Add(menuItem);

                // Root Node
                if (node == rootNode)
                {
                    //var list = CloneItems(rootContextMenuStrip.Items);
                    //actionsToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
                    return;
                }
                // Queues Node
                if (node == queueListNode)
                {
                    //var list = CloneItems(queuesContextMenuStrip.Items);
                    //AddImportAndSeparatorMenuItems(list);
                    //actionsToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
                    return;
                }
                // Topics Node
                if (node == topicListNode)
                {
                    //var list = CloneItems(topicsContextMenuStrip.Items);
                    //AddImportAndSeparatorMenuItems(list);
                    //actionsToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
                    return;
                }

                // Subscription Node
                if (node.Text == SubscriptionEntities)
                {
                    //var list = CloneItems(topicsContextMenuStrip.Items);
                    //AddImportAndSeparatorMenuItems(list);
                    //actionsToolStripMenuItem.DropDownItems.AddRange(list.ToArray());
                    return;
                }
                

                if ((topicListNode != null) || (queueListNode != null))
                {
                    if(node.Name == TopicEntities)
                    {

                    }
                    else if(node.Name == SubscriptionEntities)
                    {
                        GetEntitiesUniq(node.Parent.Text, node.Text); 
                    }

                    /*TopicDescription tdc = topicListNode.Text;

                    GetQMessageCount(topicListNode.Text, queueListNode.Text);*/
                }
                

            }
            
        }

        private void StripTimerFunc()
        {
            while (true)
            {
                DateTime dt = DateTime.Now;
                SetStatusTimerThread("Time: " + dt.ToString());                

                Thread.Sleep(950);
            }
        }

        private void SetStatusTimerThread(string text)//,string strRuningTime)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.InvokeRequired) //(this.textBox1.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    SetStatusTimerThread(text);//, strRuningTime);
                });
            }
            else
            {
                this.toolStripStatusLabelTime.Text = text;// textBox1.Text = text;                
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.TimerThread.Interrupt();
            this.TimerThread.Abort();
            //Environment.Exit(Environment.ExitCode);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

    }
}
