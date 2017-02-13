using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PhotoboothWpf
{
    class MenuData
    {
        public string actualTemplate { get;  set; }
        public string firstPrinter { get; set; }
        public string timeBetweenPhotos { get; set; }
        public string printingTime { get; set; }
        public string maxNumberOfCopies { get; set; }
        public string secondPrinter { get; set; }
        public string SmtpServerName { get; set; }
        public string SmtpPortNumber { get; set; }

        public string EmailHostAddress { get; set; }
        public string EmailHostPassword { get; set; }


        public  void FillValues(string acttemp, string actprint, string timephotos, string printtime, string maxcopies, string nsecondprinter, string emailHostAddress, string emailHostPassword, string emailSmtpServerName, string emailSmtpPortNumber)
        {
            actualTemplate = acttemp;
            firstPrinter = actprint;
            timeBetweenPhotos = timephotos;
            printingTime = printtime;
            maxNumberOfCopies = maxcopies;
            secondPrinter = nsecondprinter;
            EmailHostAddress = emailHostAddress;
            EmailHostPassword = emailHostPassword;
            SmtpServerName = emailSmtpServerName;
            SmtpPortNumber = emailSmtpPortNumber;   
            SaveToXml();
        }
        public void SaveToXml()
        {
            using (XmlWriter writer = XmlWriter.Create("menusettings.xml"))
            {
                writer.WriteStartElement("Setting");
                writer.WriteElementString("actualTemplate", actualTemplate);
                writer.WriteElementString("actualPrinter", firstPrinter);
                writer.WriteElementString("secondPrinter", secondPrinter);
                writer.WriteElementString("timeBetweenPhotos", timeBetweenPhotos);
                writer.WriteElementString("printingTime", printingTime);
                writer.WriteElementString("maxNumberOfCopies", maxNumberOfCopies);
                writer.WriteElementString("EmailHostAddress", EmailHostAddress);
                writer.WriteElementString("EmailHostPassword", EmailHostPassword);
                writer.WriteElementString("SmtpServerName", SmtpServerName);
                writer.WriteElementString("SmtpPortNumber", SmtpPortNumber);
                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
