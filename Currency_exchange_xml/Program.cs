using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Currency_exchange_xml
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchangeRateNBP = new ExchangeRateDownloader("http://www.nbp.pl/kursy/xml/a240z161213.xml");
            var nbpRates = exchangeRateNBP.DownloadExchangeRates();
            var user = new User_input();
       
            var convertCurrency = new CurrencyConventer();
            decimal convertedMoney = convertCurrency.Exchange(nbpRates);

            Console.WriteLine(" {0} {1} is {2} {3}",User_input.moneyToChange, User_input.actualCurrency,convertedMoney,User_input.desirableCurrency);

        }

    }

}
