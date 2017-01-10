using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WPF_Currency_Converter
{

   
    class ExchangeRateDownloader
    {
        private readonly string _url;



    public ExchangeRateDownloader(string url)
    {
        _url = url;
    }

    public XDocument DownloadExchangeRates()
    {
        return XDocument.Load(_url);
    }

    }
    public class CurrencyBoxData
    {

        public string displayValue { get; private set; }
        public decimal currencyCourse { get; private set; }
        public int currencyConverter { get; private set; }
        public string currencyCode { get; private set; }
        public CurrencyBoxData(string ncurrencyName, string ncurrencyCode, decimal ncurrencyCourse, int ncurrencyConverter)
        {
            currencyCode = ncurrencyCode;
            displayValue = ncurrencyName + " (" + ncurrencyCode + ")";

            currencyCourse = ncurrencyCourse;

            currencyConverter = ncurrencyConverter;

        }
    }
    internal class FillComboBox
    {
        public List<CurrencyBoxData> LoadComboBox(XDocument xdoc)
        {
            List<CurrencyBoxData> checkBoxVal = new List<CurrencyBoxData>();
            var query = from position in xdoc.Root.Elements("pozycja")
                        select position;

            foreach (var item in query)
            {
                checkBoxVal.Add(new CurrencyBoxData(item.Element("nazwa_waluty").Value, item.Element("kod_waluty").Value,
                   Convert.StringToDecimal(item.Element("kurs_sredni").Value), Int32.Parse(item.Element("przelicznik").Value)));
            }
            return checkBoxVal;
        }

    }
    class Convert
    {
        static public decimal StringToDecimal(string value)
        {
            decimal decimalVal = 0;
            if (Decimal.TryParse(value, out decimalVal) == false)
                MessageBox.Show("Wrong amount, try again");
            return decimalVal;
        }
        static public decimal DecimalToRound(decimal value, int decimalPlace)
        {
            return Decimal.Round(value, decimalPlace, MidpointRounding.AwayFromZero);
        }
    }
    class XmlAdd
    {
       static public XDocument AddPLN(XDocument xdoc)
        {
            XElement root = new XElement("pozycja");
            root.Add(new XElement("nazwa_waluty", "polski zloty"));
            root.Add(new XElement("kod_waluty", "PLN"));
            root.Add(new XElement("przelicznik", "1"));
            root.Add(new XElement("kurs_sredni", "1"));
            xdoc.Element("tabela_kursow").Add(root);
            return xdoc;
        }
    }
    class CurrencyExchanger 
    {
       static public decimal Exchange(string actualCurrencyCode,decimal actualCurrencyCourse,int actualCurrencyConverter,string desirableCurrencyCode, decimal desirableCurrencyCourse,int desirableCurrencyConverter,
           decimal moneyToChange )
        {            
            decimal exchangedMoney = 0;

            if (actualCurrencyCode == "PLN")
            {
                exchangedMoney = moneyToChange *desirableCurrencyConverter / desirableCurrencyCourse;
            }

            else if (desirableCurrencyCode == "PLN")
            {
                exchangedMoney = moneyToChange * actualCurrencyCourse / actualCurrencyConverter;
            }
            // cross rate using PLN 
            else
            {
                exchangedMoney = actualCurrencyCourse / actualCurrencyConverter / desirableCurrencyCourse / desirableCurrencyConverter * moneyToChange;

            }
            return Convert.DecimalToRound(exchangedMoney, 2);
        }
    }
}
