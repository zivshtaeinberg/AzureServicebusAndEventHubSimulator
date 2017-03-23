using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceBusSimulator
{
    public partial class MessageForm : Form
    {
        public string strMessage = "Test message";
        public string strMessageProperty = "MessageId";
        public string strMessageCount = "1";

        public MessageForm()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            strMessage = textBoxMessage.Text;
            strMessageProperty = textBoxProperty.Text;
            strMessageCount = textBoxMessageCount.Text;
            DialogResult = DialogResult.OK;

            Close();
        }

        private void textBoxMessageCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            
        }

        private void textBoxMessageCount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                hScrollBar.Value = Convert.ToInt32(textBoxMessageCount.Text);
            }
            catch(Exception ex)
            {
                int x = 0;
            }
            
        }

        private void hScrollBar_LocationChanged(object sender, EventArgs e)
        {

        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            textBoxMessageCount.Text = hScrollBar.Value.ToString();
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            textBoxMessage.Text = strMessage;
            textBoxProperty.Text = strMessageProperty;
            textBoxMessageCount.Text = strMessageCount;

            this.Size = new Size(334, 470);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void buttonImportaFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a file";

            op.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            /*
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            "Portable Network Graphic (*.png)|*.png" +
            "TXT files|*.txt";*/

            if (op.ShowDialog() == DialogResult.OK)
            {
                string strFileName = op.FileName;
                if((strFileName.Contains(".jpg")) ||
                  (strFileName.Contains(".jpeg")) ||
                  (strFileName.Contains(".png")))
                {


                    try
                    {
                        System.IO.Stream fileStream = System.IO.File.Open(strFileName, System.IO.FileMode.Open);
                        System.IO.StreamReader reader = new System.IO.StreamReader(fileStream);

                        string strBuffer = "";
                        string line = null;
                        do
                        {
                            // get the next line from the file
                            line = reader.ReadLine();
                            if (line == null)
                            {
                                // there are no more lines; break out of the loop
                                break;
                            }

                            // split the line on each semicolon character
                            string[] parts = line.Split(';');
                            // now the array contains values as such:
                            // "Name" in parts[0] 
                            // "Surname" in parts[1] 
                            // "Birthday" in parts[2] 
                            // "Address" in parts[3]
                            strBuffer += line;

                        } while (true);

                        textBoxMessage.AppendText(strBuffer);

                        reader.Dispose();
                        reader.Close();
                        fileStream.Dispose();
                        fileStream.Close();
                        
                        
                        pictureBoxPreview.Image = new Bitmap(strFileName); ;//Controls.Add(imageControl);                
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.StretchImage;
                        int iWidth = 334 + pictureBoxPreview.Size.Width + 30;

                        this.Size = new Size(iWidth, 470);
                        this.StartPosition = FormStartPosition.CenterScreen;
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                    
                    //textBoxMessage.Text = Convert.ToString(imgBytes);
                }
                else
                {
                    System.IO.Stream fileStream = System.IO.File.Open(strFileName, System.IO.FileMode.Open);
                    System.IO.StreamReader reader = new System.IO.StreamReader(fileStream);

                    string strBuffer = "";
                    string line = null;
                    do
                    {
                        // get the next line from the file
                        line = reader.ReadLine();
                        if (line == null)
                        {
                            // there are no more lines; break out of the loop
                            break;
                        }

                        // split the line on each semicolon character
                        string[] parts = line.Split(';');
                        // now the array contains values as such:
                        // "Name" in parts[0] 
                        // "Surname" in parts[1] 
                        // "Birthday" in parts[2] 
                        // "Address" in parts[3]
                        strBuffer += line;

                    } while (true);

                    this.Size = new Size(334, 470);
                    this.FormBorderStyle = FormBorderStyle.FixedDialog;
                    textBoxMessage.Text = strBuffer;// fileStream.ToString();                    
                }
            }
        }
    }
}
