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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string actualCurrencyCode = "";
        string desirableCurrencyCode = "";
        decimal amount;
        decimal actualCurrencyCourse = 0;
        decimal desirableCurrencyCourse = 0;
        int actualCurrencyConverter = 1;
        int desirableCurrencyConverter = 1;

        decimal exchangedValue = 0;


        List<CurrencyBoxData> checkBoxVal = new List<CurrencyBoxData>();       
        
        public MainWindow()
        {
            InitializeComponent();
            CompleteComboBox();
        }
        public void CompleteComboBox()
        {
            var TodayURL = new GetActualExchangeRate("http://www.nbp.pl/kursy/xml/dir.txt");
            var exchangeRateNBP = new ExchangeRateDownloader(TodayURL.xmlURL);
            var nbpXml = exchangeRateNBP.DownloadExchangeRates();
            nbpXml = XmlAdd.AddPLN(nbpXml);
            FillComboBox nbp = new FillComboBox();
            checkBoxVal = nbp.LoadComboBox(nbpXml);
        }

        public void CurrencyBoxLoad(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = checkBoxVal;
            comboBox.DisplayMemberPath = "displayValue";
            comboBox.SelectedValuePath = "currencyCourse";
            comboBox.SelectedIndex = 0;
        }


        private void Count_Click(object sender, RoutedEventArgs e)
        {
            //Collecting info from user
            actualCurrencyCourse = ((CurrencyBoxData)ActualCurrencyBox.SelectedItem).currencyCourse;
            actualCurrencyConverter = ((CurrencyBoxData)ActualCurrencyBox.SelectedItem).currencyConverter;
            actualCurrencyCode = ((CurrencyBoxData)ActualCurrencyBox.SelectedItem).currencyCode;

            desirableCurrencyCourse = ((CurrencyBoxData)DesirableCurrencyBox.SelectedItem).currencyCourse;
            desirableCurrencyConverter = ((CurrencyBoxData)DesirableCurrencyBox.SelectedItem).currencyConverter;
            desirableCurrencyCode = ((CurrencyBoxData)DesirableCurrencyBox.SelectedItem).currencyCode;

            amount = Convert.StringToDecimal(AmountTextBox.Text);

            //calculating
            exchangedValue = CurrencyExchanger.Exchange(actualCurrencyCode, actualCurrencyCourse, actualCurrencyConverter, desirableCurrencyCode, desirableCurrencyCourse, desirableCurrencyConverter, amount);

            
            FillResult();     
        }     
        public void FillResult()
            {
            ResultTextBlock.Text = exchangedValue.ToString();
            ResultCodeTextBlock.Text = desirableCurrencyCode;
            }
     

    }


}
