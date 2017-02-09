using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using EOSDigital.SDK;


namespace PhotoboothWpf.Classes
{
    class EmailSender
    {
        public string clientEmailAdress;
        public static void SendEmail(int photoNumber, int numberOfPhotosToSendViaEmail)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //put your SMTP address and port here.
                SmtpClient SmtpServer = new SmtpClient("smtp-mail.outlook.com");
                //Put the email address
                mail.From = new MailAddress("photomadnesstest@hotmail.com");
                //Put the email where you want to send.
                mail.To.Add("0ptaq0@gmail.com");

                mail.Subject = "Test1234Topic";

                StringBuilder sbBody = new StringBuilder();

                sbBody.AppendLine("Hi, this is test mail");

                mail.Body = sbBody.ToString();
                 
                var instance = new SavePhoto(photoNumber);
                Debug.WriteLine("Jestem przed For`em");
                for (int i = 0; i < numberOfPhotosToSendViaEmail; i++)
                {
                    photoNumber = (instance.PhotoNumberJustTaken()-i);
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
                SmtpServer.Credentials = new NetworkCredential("photomadnesstest@hotmail.com", "Photomadness123");
                SmtpServer.Port = 587;
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("The e-mail has been sent! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

       
    }
}

