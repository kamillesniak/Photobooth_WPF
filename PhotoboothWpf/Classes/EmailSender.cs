using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using EOSDigital.SDK;


namespace PhotoboothWpf.Classes
{
    class EmailSender
    {
        private string smtpServerName;
        private string smtpPortNumber;
        private string emailHostAddress;
        private string emailHostPassword;

        private XDocument settings = new XDocument();
        private string currentDirectory = Environment.CurrentDirectory;

        public async void SendEmail(int photoNumber, int numberOfPhotosToSendViaEmail, string emailClientAddress)
        {
            try
            {
                await Task.Run(() =>
                {

                    LoadValues();
                    MailMessage mail = new MailMessage();
                    //put your SMTP address and port here.
                    SmtpClient SmtpServer = new SmtpClient(smtpServerName);
                    //Put the email address
                    mail.From = new MailAddress(emailHostAddress);
                    //Put the email where you want to send.
                    //TODO: Regexp sprawdzajacy poprawny e-mail
                    mail.To.Add(emailClientAddress);

                    mail.Subject = "Test1234Topic";

                    StringBuilder sbBody = new StringBuilder();

                    sbBody.AppendLine("Hi, this is test mail");

                    mail.Body = sbBody.ToString();

                    var instance = new SavePhoto(photoNumber);
                    for (int i = 0; i < numberOfPhotosToSendViaEmail; i++)
                    {
                        photoNumber = (instance.PhotoNumberJustTaken() - i);
                        Debug.WriteLine("photo number is: " + photoNumber);
                        string photoName = instance.photoNaming(photoNumber);
                        string photoDirectoryPath = Path.Combine(Actual.FilePath(), photoName);
                        Debug.WriteLine(photoDirectoryPath);
                        Attachment attachment = new Attachment(photoDirectoryPath);
                        mail.Attachments.Add(attachment);
                    }

                    /// mail - photomadnesstest@hotmail.com
                    /// pass - Photomadness123
                    /// Server name: smtp-mail.outlook.com
                    //Port: 587
                    //TODO:dodac mozliwosc podmiany maili
                    SmtpServer.Credentials = new NetworkCredential(emailHostAddress, emailHostPassword);
                    SmtpServer.Port = int.Parse(smtpPortNumber);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                });
            }
            catch (FormatException) { Report.Error("Wrong e-mail format \nPlease enter your e-mail correctly\nexample@mail.com", true); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        public void LoadValues()
        {
            try
            {
                settings = XDocument.Load(Path.Combine(currentDirectory, "menusettings.xml"));
                settings.Root.Elements("setting");
                emailHostAddress = settings.Root.Element("EmailHostAddress").Value;
                emailHostPassword = settings.Root.Element("EmailHostPassword").Value;
                smtpServerName = settings.Root.Element("SmtpServerName").Value;
                smtpPortNumber = settings.Root.Element("SmtpPortNumber").Value;
             }
            catch (XmlException e)
            {
                Debug.WriteLine("LoadDefaultValues exception");
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("missing settings in menusettings.xml");
            }



        }
    }
}

