﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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
using EOSDigital.API;
using EOSDigital.SDK;
using System.Threading;


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

        List<System.Windows.Media.ImageSource> resizedImages = new List<System.Windows.Media.ImageSource>();

        int photoNumber = 0;
        int timeLeft = 5;
        int photosInTemplate = 0;
        int printNumber = 0;


        string printPath = string.Empty;
        string templateName = string.Empty;



        public MainWindow()
        {
            InitializeComponent();
            //TODO TIMERS IN CLASS:

            sliderTimer = new System.Windows.Threading.DispatcherTimer();
            sliderTimer.Tick += new EventHandler(slider);
            sliderTimer.Interval = new TimeSpan(0, 0, 0, 2);
            sliderTimer.Start();

            betweenPhotos = new System.Windows.Threading.DispatcherTimer();
            betweenPhotos.Tick += new EventHandler(MakePhoto);
            betweenPhotos.Interval = new TimeSpan(0, 0, 0, 5);

            secondCounter = new System.Windows.Threading.DispatcherTimer();
            secondCounter.Tick += new EventHandler(ShowTimeLeft);
            secondCounter.Interval = new TimeSpan(0, 0, 0, 0, 900);
            //Canon:
          //  Timers.InitalizeTimers();


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
                photosInTemplate++;
                MainCamera.TakePhotoAsync();

                betweenPhotos.Stop();
                secondCounter.Stop();

                PhotoTextBox.Visibility = Visibility.Hidden;

                timeLeft = 5;
               
   
            }
            catch (Exception ex) { Report.Error(ex.Message, false); }

            // TODO: zamiast sleppa jakas metoda ktora sprawdza czy zdjecie juz sie zrobilio i potem kolejna linia kodu-
            Thread.Sleep(8000);

            PhotoTextBox.Visibility = Visibility.Visible;
            PhotoTextBox.Text = "Prepare for next Photo!";

            if (Control.photoTemplate(photosInTemplate, 3))
            {
                var printdata = new SavePrints(printNumber);
                printPath = printdata.PrintDirectory;
                // tu trzeba beędzie zrobić case z 4 opcjami szablonu narazie są 3 paski:
                LayTemplate.foreground3(printPath);
                printNumber++;

                //print menu on
                PrintMenu();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            sliderTimer.Start();
            Slider.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Visible;
            Print.Visibility = Visibility.Hidden;
            ShowPrint.Visibility = Visibility.Hidden;
            CloseSession();
        }
        public void ShowTimeLeft(object sender, EventArgs e)
        {

                PhotoTextBox.Text = timeLeft.ToString();
                timeLeft--;         
        }
        #endregion

        #region slider
        public void slider(object sender, EventArgs e)
        {
            var sliderData = new Slider();
            ImageBrush slide = new ImageBrush();           
            slide.ImageSource = new BitmapImage(new Uri(sliderData.imagePath));            
            Slider.Background = slide;
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

        //private void MainCamera_ProgressChanged(object sender, int progress)
        //{
        //    try { MainProgressBar.Dispatcher.Invoke((Action)delegate { MainProgressBar.Value = progress; }); }
        //    catch (Exception ex) { Report.Error(ex.Message, false); }
        //}

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
                string dir = savedata.FolderDirectory;

                Info.FileName = savedata.PhotoName;
                sender.DownloadFile(Info, dir);

                
                ReSize.ImageAndSave(savedata.PhotoDirectory,photosInTemplate);

         
            }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }
        private void ResizeImage()
        {

        }
        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
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
            StartButton.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Visible;
            PhotoTextBox.Visibility = Visibility.Visible;
            PhotoTextBox.Text = "Are you ready for first picture?";
            try
            {
                if (!MainCamera.IsLiveViewOn)
                {
                   Slider.Background = liveView;
                    MainCamera.StartLiveView();
                    //.Content = "Stop LV";
                }
                else
                {
                    MainCamera.StopLiveView();
                  //  StarLVButton.Content = "Start LV";
                    Slider.Background = Brushes.LightGray;
                }
            }
            catch (Exception ex) { Report.Error(ex.Message, false); }
        }

        #endregion
        #region Subroutines

        private void CloseSession()
        {
            MainCamera.CloseSession();

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

          

            pdialog.PrintVisual(vis, "My Image");

        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            LoadAndPrint(printPath);
        }
        private void PrintMenu()
        {
            Slider.Visibility = Visibility.Hidden;
            ReadyButton.Visibility = Visibility.Hidden;

            BitmapImage actualPrint = new BitmapImage();
            actualPrint.BeginInit();
            actualPrint.UriSource = new Uri(printPath);
            actualPrint.EndInit();

            ShowPrint.Source = actualPrint;
            Print.Visibility = Visibility.Visible;
            ShowPrint.Visibility = Visibility.Visible;
        }
        #endregion

    }

}