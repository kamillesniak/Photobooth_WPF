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
using Path = System.IO.Path;

namespace PhotoboothWpf
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
   
    public partial class Menu : Page
    {
        List<string> foregroundList = new List<string>();
        List<string> printerList = new List<string>();
        List<int> copiesCount = new List<int>();
        XDocument settings = new XDocument();
        private string currentDirectory = Environment.CurrentDirectory;
        public Menu()
        {
            InitializeComponent();
            FillList();
            FillComboBox();
        }
        public void FillList()
        {
            foregroundList.Add("foreground_1");
            foregroundList.Add("foreground_3");
            foregroundList.Add("foreground_4");
            foregroundList.Add("foreground_4_paski");
            foregroundList.Add("All");

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printerList.Add(printer);
            }

            for(int i=1;i<6;i++)
            {
                copiesCount.Add(i);
            }
        }
        public void FillComboBox()
        {
            ForegroundComboBox.ItemsSource = foregroundList;
            PrinterComboBox.ItemsSource = printerList;
            Printer2ComboBox.ItemsSource = printerList;
            CopiesComboBox.ItemsSource = copiesCount;
            LoadDefaultValues();

        }
        public void LoadDefaultValues()
        {
            settings = XDocument.Load(Path.Combine(currentDirectory, "menusettings.xml"));
            settings.Root.Elements("setting");
            ForegroundComboBox.SelectedValue = settings.Root.Element("actualTemplate").Value;
            PrinterComboBox.SelectedValue = settings.Root.Element("actualPrinter").Value;
            Printer2ComboBox.SelectedValue = settings.Root.Element("secondPrinter").Value;
            CopiesComboBox.SelectedValue = Convert.ToInt32(settings.Root.Element("maxNumberOfCopies").Value);
            TimeBetweenPhotosSlider.Value = Convert.ToDouble(settings.Root.Element("timeBetweenPhotos").Value);
            PrintingTimeSlider.Value = Convert.ToDouble(settings.Root.Element("printingTime").Value);

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            Save();
            Application.Current.Shutdown();
        }
        private void Save()
        {
            var savedata = new MenuData();
            savedata.FillValues(ForegroundComboBox.SelectedValue.ToString(),
                                PrinterComboBox.SelectedValue.ToString(),
                                TimeBetweenPhotosSlider.Value.ToString(),
                                PrintingTimeSlider.Value.ToString(),
                                CopiesComboBox.SelectedValue.ToString(),
                                Printer2ComboBox.SelectedValue.ToString());


        }

        private void WithoutSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MoreOptionsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
