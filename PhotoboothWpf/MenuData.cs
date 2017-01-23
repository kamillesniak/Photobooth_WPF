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
        public string actualPrinter { get; set; }
        public string timeBetweenPhotos { get; set; }
        public string printingTime { get; set; }
        public string maxNumberOfCopies { get; set; }

     
        public  void FillValues(string acttemp, string actprint, string timephotos, string printtime, string maxcopies)
        {
            actualTemplate = acttemp;
            actualPrinter = actprint;
            timeBetweenPhotos = timephotos;
            printingTime = printtime;
            maxNumberOfCopies = maxcopies;
            SaveToXml();
        }
        public void SaveToXml()
        {
            using (XmlWriter writer = XmlWriter.Create("menusettings.xml"))
            {
                writer.WriteStartElement("Setting");
                writer.WriteElementString("actualTemplate", actualTemplate);
                writer.WriteElementString("actualPrinter", actualPrinter);
                writer.WriteElementString("timeBetweenPhotos", timeBetweenPhotos);
                writer.WriteElementString("printingTime", printingTime);
                writer.WriteElementString("maxNumberOfCopies", maxNumberOfCopies);
                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
