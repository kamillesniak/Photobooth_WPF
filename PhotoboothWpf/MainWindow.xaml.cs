using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
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
using System.IO;
using System.Linq.Expressions;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Threading;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;
using PhotoboothWpf.Classes;
using Image = System.Windows.Controls.Image;
using Path = System.IO.Path;
using Point = System.Drawing.Point;


namespace PhotoboothWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Threading.DispatcherTimer sliderTimer;
        public System.Windows.Threading.DispatcherTimer betweenPhotos;
        public System.Windows.Threading.DispatcherTimer secondCounter;

        CanonAPI APIHandler;
        Camera MainCamera;
        ImageBrush liveView = new ImageBrush();
        Action<BitmapImage> SetImageAction;
        List<Camera> CamList;
        PrintDialog pdialog = new PrintDialog();

        XDocument actualSettings = new XDocument();
        List<System.Windows.Media.ImageSource> resizedImages = new List<System.Windows.Media.ImageSource>();  

        public int photoNumber = 0;
        public int photoNumberInTemplate = 0;
        int timeLeft = 5;
        int timeLeftCopy = 5;
        int photosInTemplate = 0;
        int printNumber = 0;
        int maxCopies = 1;
        short actualNumberOfCopies = 1;
        int printtime = 10;

        public string SmtpServerName;
        public string SmtpPortNumber;
        public string EmailHostAddress;
        public string EmailHostPassword;

        string printPath = string.Empty;
        public string templateName = string.Empty;
        string printerName = string.Empty;
        private string currentDirectory = Environment.CurrentDirectory;
        public bool turnOnTemplateMenu = false;
        public bool PhotoTaken = false;

   
        public MainWindow()
        {
            InitializeComponent();
            FillSavedData();
            ActivateTimers();
            CheckTemplate();     

            //Canon:
            try
            {
                Create.TodayPhotoFolder();
                APIHandler = new CanonAPI();
                APIHandler.CameraAdded += APIHandler_CameraAdded;

                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;
                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;

                SetImageAction = (BitmapImage img) => { liveView.ImageSource = img; };
          
                RefreshCamera();
                OpenSession();
                MainCamera.SetCapacity(4096, 0x1FFFFFFF);
                
            }
            // TODO: Close main windows when null reference occures
            catch (NullReferenceException) { Report.Error("Chceck if camera is turned on and restart the program", true); }
            catch (DllNotFoundException) { Report.Error("Canon DLLs not found!", true); }
            catch (Exception ex) { Report.Error(ex.Message, true); }
           

        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
              
                MainCamera?.Dispose();
                APIHandler?.Dispose();
            }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        #region TakePhoto
        private void ReadyButton_Click(object sender, EventArgs e)
        {
            betweenPhotos.Start();
            secondCounter.Start();
        }

        public void MakePhoto(object sender, EventArgs e)
        {
            try
            {
                photoNumberInTemplate++;
                photosInTemplate++;
                // MainCamera.TakePhotoAsync();
                Debug.WriteLine("taking a shot");
                MainCamera.SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.Halfway);
                Debug.WriteLine("halfway");
                MainCamera.SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.Completely_NonAF);
                Debug.WriteLine("completely_nonaf");
                MainCamera.SendCommand(CameraCommand.PressShutterButton, (int)ShutterButton.OFF);
                Debug.WriteLine("Finished taking a shot");

                betweenPhotos.Stop();
                secondCounter.Stop();

                PhotoTextBox.Visibility = Visibility.Hidden;

                timeLeftCopy = timeLeft;


                Debug.WriteLine("photo number in template: " + photoNumberInTemplate);
                Debug.WriteLine("photos in template: " + photosInTemplate);
                Debug.WriteLine("photo number: " + photoNumber);

            }

            catch (Exception ex) { Report.Error(ex.Message, false); }
            // TODO: zamiast sleppa jakas metoda ktora sprawdza czy zdjecie juz sie zrobilio i potem kolejna linia kodu-

            
            Thread.Sleep(2000);
            //jak mam sleep 4000 to mi nie dziala


            PhotoTextBox.Visibility = Visibility.Visible;
                PhotoTextBox.Text = "Prepare for next Photo!";
            
            // Waiting for photo saving 
            while (PhotoTaken==false)
            {
                Thread.Sleep(1000);
            }
            ShowPhotoThumbnail();
            PhotoTaken = false;

            // One if than switch
                switch (templateName)
                {
                    case "foreground_1":
                        if (Control.photoTemplate(photosInTemplate, 1))
                        {
                            var printdata = new SavePrints(printNumber);
                            printPath = printdata.PrintDirectory;
                            LayTemplate.foreground1(printPath);
                            printNumber++;
                            PrintMenu();
                        }
                        break;

                    case "foreground_3":
                        if (Control.photoTemplate(photosInTemplate, 3))
                        {
                            var printdata = new SavePrints(printNumber);
                            printPath = printdata.PrintDirectory;
                            LayTemplate.foreground3(printPath);
                            printNumber++;
                            PrintMenu();
                        }
                        break;
                    case "foreground_4":
                        if (Control.photoTemplate(photosInTemplate, 4))
                        {
                            var printdata = new SavePrints(printNumber);
                            printPath = printdata.PrintDirectory;
                            LayTemplate.foreground4(printPath);
                            printNumber++;
                            PrintMenu();
                        }
                        break;

                    case "foreground_4_paski":
                        if (Control.photoTemplate(photosInTemplate, 4))
                        {
                            var printdata = new SavePrints(printNumber);
                            printPath = printdata.PrintDirectory;
                            LayTemplate.foreground4stripes(printPath);
                            printNumber++;
                            PrintMenu();
                        }
                        break;
                    default:
                        Debug.WriteLine("bug at switch which template");
                        break;
                }

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            sliderTimer.Start();
//TODO OR NOT WHEN NO CAMERA CONNECTED WILL CAUSE BUG WHILE CLICK STOP IN FOREGRUND MENU
            MainCamera.StopLiveView();
            photosInTemplate = 0;
            photoNumberInTemplate = 0;
            if (turnOnTemplateMenu) StartAllForegroundsWelcomeMenu();
            else StartWelcomeMenu();
            TurnOffForegroundMenu();


        }
        public void ShowTimeLeft(object sender, EventArgs e)
        {

                PhotoTextBox.Text = timeLeftCopy.ToString();
                timeLeftCopy--;         
        }
        #endregion

        #region slider
        public void slider(object sender, EventArgs e)
        {
            var sliderData = new Slider();
            
            ImageBrush slide = new ImageBrush();
          //  slide.Stretch = Stretch.Uniform;
        
            slide.ImageSource = new BitmapImage(new Uri(sliderData.imagePath));            
            Slider.Background = slide;
            
          //  var ratio = Math.Min(Slider.RenderSize.Width / slide.ImageSource.Width, Slider.RenderSize.Height / slide.ImageSource.Height);
        //    CreateDynamicBorder(slide.ImageSource.Width, slide.ImageSource.Height);

        }


    #endregion

    #region API Events

    private void APIHandler_CameraAdded(CanonAPI sender)
        {
            try { Dispatcher.Invoke((Action)delegate { RefreshCamera(); }); }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        private void MainCamera_StateChanged(Camera sender, StateEventID eventID, int parameter)
        {
            try { if (eventID == StateEventID.Shutdown) { Dispatcher.Invoke((Action)delegate { CloseSession(); }); } }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
            try
            {
                using (WrapStream s = new WrapStream(img))
                {
                    img.Position = 0;
                    BitmapImage EvfImage = new BitmapImage();
                    EvfImage.BeginInit();
                    EvfImage.StreamSource = s;
                    EvfImage.CacheOption = BitmapCacheOption.OnLoad;
                    EvfImage.EndInit();
                    EvfImage.Freeze();
                    Application.Current.Dispatcher.BeginInvoke(SetImageAction, EvfImage);
                }
            }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
           
            try
            {
                photoNumber++;
                var savedata = new SavePhoto(photoNumber);
                string  dir = savedata.FolderDirectory;

                Info.FileName = savedata.PhotoName;               
                sender.DownloadFile(Info, dir);
             
//                ReSize.ImageAndSave(savedata.PhotoDirectory,photosInTemplate,templateName);
                ReSize.ImageAndSave(savedata.PhotoDirectory,photoNumberInTemplate,templateName);

            }
            catch (Exception ex) { Report.Error(ex.Message, false); }

            PhotoTaken = true;
            
        }

        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
            string errorCode = ((int)ex).ToString("X");
            switch (errorCode)
             {
               case "8D01": // TAKE_PICTURE_AF_NG
                     if (photoNumberInTemplate!=0)
                         {
                            photoNumberInTemplate--;
                        }
                     if (photosInTemplate != 0)
                         {
                            photosInTemplate--;
                        }
                    PhotoTaken = true;
               Debug.WriteLine("Autofocus error");
               return;
             }
            Report.Error($"SDK Error code: {ex} ({((int)ex).ToString("X")})", false);
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            Report.Error(ex.Message, true);
        }

        #endregion

        #region Live view

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            sliderTimer.Stop();
            TurnOffForegroundMenu();
            SliderBorder.Visibility = Visibility.Visible;
            Slider.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Hidden;
            StartButtonMenu.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Visible;
            PhotoTextBox.Visibility = Visibility.Visible;
            PhotoTextBox.Text = "Are you ready for first picture?";

            try
            {
                MainCamera.SendCommand(CameraCommand.PressShutterButton, 1);

            }
            finally
            {
                MainCamera.SendCommand(CameraCommand.PressShutterButton, 0);
            }
            try
            {
                
                Slider.Background = liveView;
                    MainCamera.StartLiveView();

            }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        #endregion

        #region Subroutines

        private void CloseSession()
        {
            try
             {
                MainCamera.CloseSession();
             }
             catch (ObjectDisposedException) { Report.Error("Camera has been turned off! \nPlease turned it on and restart the application", true); }
            //SettingsGroupBox.IsEnabled = false;
            //LiveViewGroupBox.IsEnabled = false;
            //SessionButton.Content = "Open Session";
            //SessionLabel.Content = "No open session";
            //StarLVButton.Content = "Start LV";
        }

        private void RefreshCamera()
        {
            CameraListBox.Items.Clear();
            CamList = APIHandler.GetCameraList();
            foreach (Camera cam in CamList) CameraListBox.Items.Add(cam.DeviceName);
            if (MainCamera?.SessionOpen == true) CameraListBox.SelectedIndex = CamList.FindIndex(t => t.ID == MainCamera.ID);
            else if (CamList.Count > 0) CameraListBox.SelectedIndex = 0;
        }

        private void OpenSession()
        {
            if (CameraListBox.SelectedIndex >= 0)
            {
                MainCamera = CamList[CameraListBox.SelectedIndex];
                MainCamera.OpenSession();
                MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;           
                MainCamera.StateChanged += MainCamera_StateChanged;
                MainCamera.DownloadReady += MainCamera_DownloadReady;
                MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
                MainCamera.SetCapacity(4096, 0x1FFFFFFF);

            }
        }

        private void EnableUI(bool enable)
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.Invoke((Action)delegate { EnableUI(enable); });
            else
            {
            //    SettingsGroupBox.IsEnabled = enable;
             //   InitGroupBox.IsEnabled = enable;
           //     LiveViewGroupBox.IsEnabled = enable;
            }
        }



        #endregion

        #region Printing

        private void LoadAndPrint(string printPath)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(printPath);
            bi.EndInit();

            var vis = new DrawingVisual();
            var dc = vis.RenderOpen();
            dc.DrawImage(bi, new Rect { Width = bi.Width, Height = bi.Height });
            dc.Close();


            var printerSettings = new PrinterSettings();
            var labelPaperSize = new PaperSize
            {
                RawKind = (int)PaperKind.Custom, Height = 150, Width = 100
            };
            printerSettings.DefaultPageSettings.PaperSize = labelPaperSize;
            printerSettings.DefaultPageSettings.Margins = new Margins(0,0,0,0);

//            printerSettings.Copies = actualNumberOfCopies;
            pdialog.PrintVisual(vis, "My Image");
        }


        private void Print_Click(object sender, RoutedEventArgs e)
        {
            photosInTemplate = 0;
            photoNumberInTemplate = 0;
            Printing.Print(printPath,printerName,actualNumberOfCopies);
            actualNumberOfCopies = 1;
            if (turnOnTemplateMenu) StartAllForegroundsWelcomeMenu();
            else StartWelcomeMenu();
        }
        private void PrintMenu()
        {
            Slider.Visibility = Visibility.Hidden;
            SliderBorder.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Hidden;
            PhotoTextBox.Text = "Press button to continue";
            NumberOfCopiesTextBox.Text = actualNumberOfCopies.ToString();

            BitmapImage actualPrint = new BitmapImage();
            actualPrint.BeginInit();
            actualPrint.UriSource = new Uri(printPath);
            actualPrint.EndInit();

            ShowPrint.Source = actualPrint;
            Print.Visibility = Visibility.Visible;
            NumberOfCopiesTextBox.Visibility = Visibility.Visible;
            AddOneCopyButton.Visibility = Visibility.Visible;
            MinusOneCopyButton.Visibility = Visibility.Visible;
            SendEmailButton.Visibility = Visibility.Visible;


            ShowPrint.Visibility = Visibility.Visible;
    //        CreateDynamicBorder(ShowPrint.ActualWidth, ShowPrint.ActualHeight);
        }

        private void MinusOneCopyButtonClick(object sender, RoutedEventArgs e)
        {
            if (actualNumberOfCopies>1)
            {
                --actualNumberOfCopies;
                NumberOfCopiesTextBox.Text = actualNumberOfCopies.ToString();
            }
            
        }

        private void AddOneCopyButtonClick(object sender, RoutedEventArgs e)
        {
            if (actualNumberOfCopies < maxCopies)
            {
                ++actualNumberOfCopies;
                NumberOfCopiesTextBox.Text = actualNumberOfCopies.ToString();
            }
        }

        #endregion

        #region menu

        private void FillSavedData ()
        {
            string firstprinter;
            string secondprinter;

                if (!File.Exists(Path.Combine(currentDirectory, "menusettings.xml")))
                    {
                        Debug.WriteLine("XMLsettings doesnt exist");
                        Report.Error("XML settings cannot be found\nPlease Press F12 and update settings", true);
                        return;
                     }
            try
            {
                actualSettings = System.Xml.Linq.XDocument.Load(System.IO.Path.Combine(currentDirectory, "menusettings.xml"));
                actualSettings.Root.Elements("setting");
                templateName = actualSettings.Root.Element("actualTemplate").Value;
                if (actualSettings.Root.Element("actualTemplate").Value == "All")
                {
                    turnOnTemplateMenu = true;
                }
                firstprinter = actualSettings.Root.Element("actualPrinter").Value;
                secondprinter = actualSettings.Root.Element("secondPrinter").Value;
                maxCopies = System.Convert.ToInt32(actualSettings.Root.Element("maxNumberOfCopies").Value);
                timeLeft = System.Convert.ToInt32(actualSettings.Root.Element("timeBetweenPhotos").Value);
                printtime = System.Convert.ToInt32(actualSettings.Root.Element("printingTime").Value);
                printerName = PhotoboothWpf.Printing.ActualPrinter(templateName, firstprinter, secondprinter);
                timeLeftCopy = timeLeft;

                SmtpServerName = actualSettings.Root.Element("SmtpServerName").Value;
                SmtpPortNumber = actualSettings.Root.Element("SmtpPortNumber").Value;
                EmailHostAddress = actualSettings.Root.Element("EmailHostAddress").Value;
                EmailHostPassword = actualSettings.Root.Element("EmailHostPassword").Value;


            }
            catch (XmlException e)
            {
                Debug.WriteLine("XMLsettings cannot be load properly");
                Report.Error("XML settings cannot be load properly\nPlease Press F12 and update settings", true);
            }
                
           

        }

       

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                Menu menu1 = new Menu();
                this.Content = menu1;
            }
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion

        public void ActivateTimers()
        {
            sliderTimer = new System.Windows.Threading.DispatcherTimer();
            sliderTimer.Tick += new EventHandler(slider);
            sliderTimer.Interval = new TimeSpan(0, 0, 0, 2);
            sliderTimer.Start();

            betweenPhotos = new System.Windows.Threading.DispatcherTimer();
            betweenPhotos.Tick += new EventHandler(MakePhoto);
            betweenPhotos.Interval = new TimeSpan(0, 0, 0, timeLeft);

            secondCounter = new System.Windows.Threading.DispatcherTimer();
            secondCounter.Tick += new EventHandler(ShowTimeLeft);
            secondCounter.Interval = new TimeSpan(0, 0, 0, 0, 900);
        }

        #region Foreground_Menu

        
        private void Foreground_3_button_Click(object sender, RoutedEventArgs e)
        {
            
            templateName = "foreground_3";
            StartButton_Click(sender, e);

        }
        private void Foreground_4_button_Click(object sender, RoutedEventArgs e)
        { 
            templateName = "foreground_4";
            StartButton_Click(sender, e);
        }
        private void Foreground_1_button_Click(object sender, RoutedEventArgs e)
        {
            templateName = "foreground_1";
            StartButton_Click(sender, e);
        }
        private void Foreground_4_paski_button_Click(object sender, RoutedEventArgs e)
        {
            templateName = "foreground_4_paski";
            StartButton_Click(sender, e);
        }
       

        private void StartButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            TurnOnForegroundMenu();
           
        }
         public void TurnOnForegroundMenu()
        {
            sliderTimer.Stop();
            Slider.Visibility = Visibility.Hidden;
            SliderBorder.Visibility = Visibility.Hidden;

            Foreground_1_button.Visibility = Visibility.Visible;
            Foreground_3_button.Visibility = Visibility.Visible;
            Foreground_4_button.Visibility = Visibility.Visible;
            Foreground_4_paski_button.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Visible;

        }
        public void TurnOffForegroundMenu()
        {
            Foreground_1_button.Visibility = Visibility.Hidden;
            Foreground_3_button.Visibility = Visibility.Hidden;
            Foreground_4_button.Visibility = Visibility.Hidden;
            Foreground_4_paski_button.Visibility = Visibility.Hidden;
        }
        public void StartAllForegroundsWelcomeMenu()
        {
            PhotoTextBox.Visibility = Visibility.Visible;
            PhotoTextBox.Text = "Hello";
            SliderBorder.Visibility = Visibility.Visible;
            StartButtonMenu.Visibility = Visibility.Visible;
            Slider.Visibility = Visibility.Visible;
            sliderTimer.Start();

            StopButton.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Hidden;
            Print.Visibility = Visibility.Hidden;
            ShowPrint.Visibility = Visibility.Hidden;
            NumberOfCopiesTextBox.Visibility = Visibility.Hidden;
            AddOneCopyButton.Visibility = Visibility.Hidden;
            MinusOneCopyButton.Visibility = Visibility.Hidden;
            SendEmailButton.Visibility = Visibility.Hidden;
            FirstThumbnail.Visibility = Visibility.Hidden;
            SecondThumbnail.Visibility = Visibility.Hidden;
            ThirdThumbnail.Visibility = Visibility.Hidden;
            FourthThumbnail.Visibility = Visibility.Hidden;
            LeftThumbnail.Visibility = Visibility.Hidden;
            CenterThumbnail.Visibility = Visibility.Hidden;
            RightThumbnail.Visibility = Visibility.Hidden;
        }
        public void CheckTemplate()
        {
       
            if (turnOnTemplateMenu)
            {
                StartButtonMenu.Visibility = Visibility.Visible;
            }
            else
            {
                StartButton.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region frontend
        private void CreateDynamicBorder(double width, double height)
        {
            var printBorder = new Border();
            // border.Background = new SolidColorBrush(Colors.LightGray);
           printBorder.BorderThickness = new Thickness(10);
            printBorder.BorderBrush = new SolidColorBrush(Colors.Coral);
         //   border.CornerRadius = new CornerRadius(15);
            printBorder.Width = width;
            printBorder.Height = height;
           
            
        }
        private void StartWelcomeMenu()
        {
            PhotoTextBox.Visibility = Visibility.Visible;
            PhotoTextBox.Text = "Hello";

            sliderTimer.Start();
            SliderBorder.Visibility = Visibility.Visible;
            Slider.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Visible;

            StopButton.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Hidden;
            Print.Visibility = Visibility.Hidden;
            ShowPrint.Visibility = Visibility.Hidden;
            NumberOfCopiesTextBox.Visibility = Visibility.Hidden;
            AddOneCopyButton.Visibility = Visibility.Hidden;
            MinusOneCopyButton.Visibility = Visibility.Hidden;
            SendEmailButton.Visibility = Visibility.Hidden;
            FirstThumbnail.Visibility = Visibility.Hidden;
            SecondThumbnail.Visibility = Visibility.Hidden;
            ThirdThumbnail.Visibility = Visibility.Hidden;
            FourthThumbnail.Visibility = Visibility.Hidden;
            LeftThumbnail.Visibility = Visibility.Hidden;
            CenterThumbnail.Visibility = Visibility.Hidden;
            RightThumbnail.Visibility = Visibility.Hidden;
        }
        #endregion

        private void SendEmailButtonClick(object sender, RoutedEventArgs e)
        {
            //in case virtual keyboard doesnt work
            //Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar + "osk.exe");
            EmailSendDialog inputEmailSendDialog = new EmailSendDialog("Please enter your email address:", "anon@example.com");
            if (inputEmailSendDialog.ShowDialog() == true)
            {
                Debug.WriteLine("inputemailsend is ok, answer is :" + inputEmailSendDialog.Answer);
                EmailSender emailSender = new EmailSender();
                switch (templateName)
                {
                    case "foreground_1":
                        emailSender.SendEmail(photoNumber, 1, inputEmailSendDialog.Answer);
                        break;

                    case "foreground_3":
                        emailSender.SendEmail(photoNumber, 3, inputEmailSendDialog.Answer);
                        break;
                    case "foreground_4":
                        emailSender.SendEmail(photoNumber, 4, inputEmailSendDialog.Answer);
                        break;

                    case "foreground_4_paski":
                        emailSender.SendEmail(photoNumber, 4, inputEmailSendDialog.Answer);
                        break;
                    default:
                        Debug.WriteLine("bug at switch which template in email send button");
                        break;
                }
            }
        }
        public void ShowPhotoThumbnail ()
        {
            var getImageThumbnail = new GetImageThumbnail();

            ImageBrush thumbnailImageBrush = new ImageBrush();
            getImageThumbnail.GetThumbnailPath();

            switch (templateName)
            {
                case "foreground_1":
                    if (photoNumberInTemplate == 1)
                    {
                        CenterThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                        CenterThumbnail.Visibility = Visibility.Visible;
                    }
                    break;


                case "foreground_3":
                    switch (photoNumberInTemplate)
                    {
                        case 1:
                            LeftThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            LeftThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 2:
                            CenterThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            CenterThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 3:
                            RightThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            RightThumbnail.Visibility = Visibility.Visible;
                            break;
                        default:
                            Debug.WriteLine("bug at switch which template in ShowPhotoThumbnail - foreground3");
                            Debug.WriteLine("bug because photoNumberInTemplate = " + photoNumberInTemplate);

                            break;
                    }
                    break;
                case "foreground_4":
                    switch (photoNumberInTemplate)
                    {
                        case 1:
                            FirstThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            FirstThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 2:
                            SecondThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            SecondThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 3:
                            ThirdThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            ThirdThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 4:
                            FourthThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            FourthThumbnail.Visibility = Visibility.Visible;
                            break;

                        default:
                            Debug.WriteLine("bug at switch which template in ShowPhotoThumbnail - foreground 4");
                            Debug.WriteLine("bug because photoNumberInTemplate = " + photoNumberInTemplate);

                            break;
                    }
                    break;

                case "foreground_4_paski":
                    switch (photoNumberInTemplate)
                    {
                        case 1:
                            FirstThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            FirstThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 2:
                            SecondThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            SecondThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 3:
                            ThirdThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            ThirdThumbnail.Visibility = Visibility.Visible;
                            break;

                        case 4:
                            FourthThumbnailImage.Source = new BitmapImage(new Uri(getImageThumbnail.thumbnailPath));
                            FourthThumbnail.Visibility = Visibility.Visible;
                            break;

                        default:
                            Debug.WriteLine("bug at switch which template in ShowPhotoThumbnail = foreground 4 paski");
                            Debug.WriteLine("bug because photoNumberInTemplate = " + photoNumberInTemplate);
                            break;
                    }
                    break;
                default:
                    Debug.WriteLine("bug at switch which template in showphotothumbnail");
                    break;
            }
        }

        private void LeftThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender,e, 1);
        }

        private void CenterThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            switch (templateName)
            {
                case "foreground_1":
                    RepeatJustTakenPhoto(sender, e, 1);
                    break;
                case "foreground_3":
                    RepeatJustTakenPhoto(sender, e, 2);
                    break;
                default:
                    Debug.WriteLine("Bug at centerThumbnail_OnClick");
                    break;
            }
        }

        private void RightThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender, e, 3);
        }

        private void FirstThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender, e, 1);
        }

        private void SecondThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender, e, 2);
        }

        private void ThirdThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender, e, 3);
        }

        private void FourthThumbnail_OnClick(object sender, RoutedEventArgs e)
        {
            RepeatJustTakenPhoto(sender, e, 4);
        }

        public void RepeatJustTakenPhoto(object sender, RoutedEventArgs e, int photoNumberToRepeat)
        {
            RepeatPhotoDialog repeatPhotoDialog = new RepeatPhotoDialog();
            if (repeatPhotoDialog.ShowDialog() == true)
            {
                photosInTemplate--;
                photoNumberInTemplate = photoNumberToRepeat-1;
                Print.Visibility = Visibility.Hidden;
                ShowPrint.Visibility = Visibility.Hidden;
                NumberOfCopiesTextBox.Visibility = Visibility.Hidden;
                AddOneCopyButton.Visibility = Visibility.Hidden;
                MinusOneCopyButton.Visibility = Visibility.Hidden;
                SendEmailButton.Visibility = Visibility.Hidden;

                StartButton_Click(sender, e);
                //TODO: Repeat selected photo, not only the last one like now
            }
        }
    }
}


