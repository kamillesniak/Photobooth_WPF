using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WPF_Currency_Converter
{
   public class GetActualExchangeRate
    {
        private string txtURL { get; set; }
        private string[] exchangeTables { get; set; }
        public string xmlURL { get; set; }
        private string todayCode { get; set; }

      public  GetActualExchangeRate(string _urlTXT)
        {
            txtURL = _urlTXT;
            exchangeTables = NbpXmlURL();
            todayCode = ActualCode();
            xmlURL = ActualXmlURL();
        }
        private string[] NbpXmlURL()
        {
            var client = new WebClient();
            string nbpXmlURL = string.Empty;
            nbpXmlURL = client.DownloadString("http://www.nbp.pl/kursy/xml/dir.txt");
            string[] lines = nbpXmlURL.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return lines;
        }
        private string ActualCode()
        {

            var date1 = DateTime.Now;
            DateTime t2 = DateTime.Parse("2016/01/12 12:25:00.000");
            string todayUrlCode = string.Empty;

            if (TimeSpan.Compare(date1.TimeOfDay, t2.TimeOfDay) == 1)
            {

                todayUrlCode = date1.ToString("yy") + date1.ToString("MM") + date1.ToString("dd");
            }
            else
            {
                date1 = DateTime.Now.AddDays(-1);
                todayUrlCode = date1.ToString("yy") + date1.ToString("MM") + date1.ToString("dd");
            }
            return todayUrlCode;
        }
        private string ActualXmlURL()
        {
            foreach (string line in exchangeTables)
            {
                if (line.Contains(todayCode) && line.Contains("a"))
                {
                    xmlURL = "http://www.nbp.pl/kursy/xml/" + line + ".xml";
                }
            }
            return xmlURL;
        }
    }
}
