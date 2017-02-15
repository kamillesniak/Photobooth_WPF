using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;
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

        UserSettings userSettings = new UserSettings();
        XDocument settings = new XDocument();
        XDocument userSettingXDoc = new XDocument();
        private string currentDirectory = Environment.CurrentDirectory;
        List<string> variables = new List<string>();


        public Menu()
        {
           // FillDefaultValue();
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

            variables.Add("WelcomeText");
            variables.Add("BeforePhotoText");
            variables.Add("FirstPhotoText");
            variables.Add("SecondPhotoText");
            variables.Add("ThirdPhotoText");
            variables.Add("FourthPhotoText");
            variables.Add("backgroundPath");
            variables.Add("buttonsColor");
            variables.Add("borderColor");
            variables.Add("textBoxColor");
            variables.Add("buttonHighlightColor");

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
            SettingsComboBox.ItemsSource = variables;
            LoadDefaultValues();

        }
        public void LoadDefaultValues()
        {
            try
            {
                settings = XDocument.Load(Path.Combine(currentDirectory, "menusettings.xml"));
                settings.Root.Elements("setting");
                ForegroundComboBox.SelectedValue = settings.Root.Element("actualTemplate").Value;
                PrinterComboBox.SelectedValue = settings.Root.Element("actualPrinter").Value;
                Printer2ComboBox.SelectedValue = settings.Root.Element("secondPrinter").Value;
                CopiesComboBox.SelectedValue = Convert.ToInt32(settings.Root.Element("maxNumberOfCopies").Value);
                TimeBetweenPhotosSlider.Value = Convert.ToDouble(settings.Root.Element("timeBetweenPhotos").Value);
                PrintingTimeSlider.Value = Convert.ToDouble(settings.Root.Element("printingTime").Value);
                ChangeEmailAddressTextBox.Text = settings.Root.Element("EmailHostAddress").Value;
                ChangeEmailPasswordTextBox.Text = settings.Root.Element("EmailHostPassword").Value;
                ChangeEmailServerTextBox.Text = settings.Root.Element("SmtpServerName").Value;
                ChangeEmailPortTextBox.Text = settings.Root.Element("SmtpPortNumber").Value;
            }
            catch (XmlException e)
            { Debug.WriteLine("LoadDefaultValues exception");}
            catch(NullReferenceException e) { Debug.WriteLine("missing settings in menusettings.xml");}
            

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            Save();
            Application.Current.Shutdown();
        }

        private void Save()
        {
            try
            {
                var savedata = new MenuData();
                savedata.FillValues(ForegroundComboBox.SelectedValue.ToString(),
                                    PrinterComboBox.SelectedValue.ToString(),
                                    TimeBetweenPhotosSlider.Value.ToString(),
                                    PrintingTimeSlider.Value.ToString(),
                                    CopiesComboBox.SelectedValue.ToString(),
                                    Printer2ComboBox.SelectedValue.ToString(),
                                    ChangeEmailAddressTextBox.Text,
                                    ChangeEmailPasswordTextBox.Text,
                                    ChangeEmailServerTextBox.Text,
                                    ChangeEmailPortTextBox.Text);
            }
            catch (Exception e)
            {
                // TODO: Update which exception
                Console.WriteLine(e);
            }
            

        }

        private void WithoutSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangeTextButton_Click(object sender, RoutedEventArgs e)
        {
            string name = SettingsComboBox.SelectedValue.ToString();
            string value = SettingsTextBox.Text;
            userSettings.ChangeText(name, value);
        }

        private void SettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            userSettingXDoc = XDocument.Load(Path.Combine(currentDirectory, "UserSettings.xml"));
            userSettingXDoc.Root.Elements("FrontEnd");
            string actualValue = SettingsComboBox.SelectedValue.ToString();
            string var = userSettingXDoc.Root.Element(actualValue).Value;
            SettingsTextBox.Text = var;
        }
        private void FillDefaultValue()
        {            
            userSettings.SetNewUserSettings("Hello", "Prepare for first photo", "Get ready for second one", "Third photo coming, prepare!", "the end",
               "backgrounds", "backgrounds", "red", "orange", "orange", "blue");
            userSettings.SaveOptions("ble");
        }
    }
}
