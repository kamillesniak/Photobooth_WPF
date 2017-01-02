using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Currency_exchange_xml
{

    public  class Convert
    {
        public decimal StringToDecimal(string value)
        {

            decimal decimalVal = 0;

            if (Decimal.TryParse(value, out decimalVal) == false)
                Console.WriteLine("Unable to parse '{0}'.", value);


            return decimalVal;
        }
        public decimal DecimalToRound(decimal value, int decimalPlace)
            {
            return Decimal.Round(value, decimalPlace, MidpointRounding.AwayFromZero);       
            }

    }
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

    public class User_input: Convert
    {
        public static string actualCurrency { get; set; }
        public static string desirableCurrency { get; set; }
        public static decimal moneyToChange { get; set; }
        public User_input()
        {
            while (User_details() == false)
            {
                Console.WriteLine("Wrong Currency code , try again");

            }
        }
        public bool User_details()
        {
            Console.WriteLine(" Hello Which courrency do you own? Give me currency code");
            actualCurrency = Console.ReadLine();

            Console.WriteLine(" How much do you want to exchange?");
            desirableCurrency = Console.ReadLine();

            moneyToChange = StringToDecimal(desirableCurrency);

            Console.WriteLine(" Which currency do you want buy?");
            desirableCurrency = Console.ReadLine();

            actualCurrency = actualCurrency.ToUpper();
            desirableCurrency = desirableCurrency.ToUpper();

            if ((actualCurrency.Length != 3) || (desirableCurrency.Length != 3) || (actualCurrency == desirableCurrency) || (moneyToChange<=0))
            {
                return false;
            }
            return true;
        }

    }
   public class CurrencyConventer : Convert
    {
        ExchangeRate ratesnNbp = new ExchangeRate();
       

        public decimal Exchange(XDocument xml_rates)
        {

            decimal exchangedMoney = 0;

            if (User_input.actualCurrency == "PLN")
            {
                ratesnNbp.getDesirableCurrencyDetails(xml_rates);
                exchangedMoney =  User_input.moneyToChange * ratesnNbp.desirableConverterVal / ratesnNbp.desirableCurrencyCourse;
            }

            else if (User_input.desirableCurrency == "PLN")
            {
                ratesnNbp.getActualCurrencyDetails(xml_rates);
                exchangedMoney = User_input.moneyToChange * ratesnNbp.actualCurrencyCourse / ratesnNbp.actualConverterVal;
            }
            // cross rate using PLN 
            else
            {
                ratesnNbp.getDesirableCurrencyDetails(xml_rates);
                ratesnNbp.getActualCurrencyDetails(xml_rates);
                exchangedMoney = ratesnNbp.actualCurrencyCourse / ratesnNbp.actualConverterVal / ratesnNbp.desirableCurrencyCourse / ratesnNbp.desirableConverterVal * User_input.moneyToChange;

            }
            return DecimalToRound(exchangedMoney, 2);
        }
    }
   public class ExchangeRate: Convert
    {
      
     
        public void getDesirableCurrencyDetails(XDocument xdoc)
        {
            var query = from position in xdoc.Root.Elements("pozycja")
                        where position.Element("kod_waluty").Value == User_input.desirableCurrency
                        select position;

            foreach (var item in query)
            {
                desirableCurrencyCourse = StringToDecimal(item.Element("kurs_sredni").Value);
                desirableConverterVal = int.Parse(item.Element("przelicznik").Value);
            }
 
        }
        public void getActualCurrencyDetails(XDocument xdoc)
        {
            var query = from position in xdoc.Root.Elements("pozycja")
                        where position.Element("kod_waluty").Value == User_input.actualCurrency
                        select position;

            foreach (var item in query)
            {
                actualCurrencyCourse = StringToDecimal(item.Element("kurs_sredni").Value);
                actualConverterVal = int.Parse(item.Element("przelicznik").Value);
            }

        }
        public int desirableConverterVal { get; set; }
        public decimal desirableCurrencyCourse { get; set; }
        public decimal actualCurrencyCourse { get; set; }
        public int actualConverterVal { get; set; }

    }
}
