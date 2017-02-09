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
using System.IO;
using System.Drawing;
using System.Xml.Linq;
using System.Xml.Schema;
using PhotoboothWpf.Properties;

namespace PhotoboothWpf
{
    class Report
    {
        static public void Error(string message, bool lockdown)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Debug.WriteLine("Error happend");
        }
    }
  

    class Actual
    {
        static public string DateNow()
        {
            var date1 = DateTime.Now;
            string todayDate = string.Empty;
            todayDate = date1.ToString("dd") + "." + date1.ToString("MM") + "." + "20" + date1.ToString("yy");
            return todayDate;
        }

        static public string FilePath()
        {
            string p1 = Environment.CurrentDirectory;
            string p2 = Actual.DateNow();
            string pathString = System.IO.Path.Combine(p1, p2);
            return pathString;
        }  
    }

    class Control
    {
        static public bool photoTemplate(int actualPhotoNumber, int photoInTemplate)
        {
            if (actualPhotoNumber == photoInTemplate) return true;
            else return false;
        }
    }

    class Create
    {
        static public void TodayPhotoFolder()
        {
            string p1 = Environment.CurrentDirectory;
            string p2 = Actual.DateNow();
            string pathString = System.IO.Path.Combine(p1, p2);
            Directory.CreateDirectory(pathString);
            string p3 = "prints";
            pathString = System.IO.Path.Combine(p2, p3);
            Directory.CreateDirectory(pathString);
        }
    }

    class ReSize
    {

        static public void ImageAndSave(string imagepath, int photoInTemplateNumb, string templateName)
        {

            switch (templateName)
            {
                case "foreground_1":
                    { 
                    byte[] imageBytes = LoadImageData(imagepath);                      
                    ImageSource imageSource = (CreateImage(imageBytes, 1390, 0));                        
                    imageBytes = GetEncodedImageData(imageSource, ".jpg");


                        SaveImageData(imageBytes, naming(photoInTemplateNumb));
            }
                break;

                case "foreground_3":
                    { 
                    byte[] imageBytes = LoadImageData(imagepath);
                    ImageSource imageSource = CreateImage(imageBytes, 410, 0);
                    imageBytes = GetEncodedImageData(imageSource, ".jpg");

                    SaveImageData(imageBytes, naming(photoInTemplateNumb));
                    }
                    break;

                case "foreground_4":
                    {
                        byte[] imageBytes = LoadImageData(imagepath);
                        ImageSource imageSource = CreateImage(imageBytes, 560, 0);
                        imageBytes = GetEncodedImageData(imageSource, ".jpg");

                        SaveImageData(imageBytes, naming(photoInTemplateNumb));
                    }
                                
                    break;

                case "foreground_4_paski":
                    { 
                    byte[] imageBytes = LoadImageData(imagepath);
                    ImageSource imageSource = CreateImage(imageBytes, 410, 0);
                    imageBytes = GetEncodedImageData(imageSource, ".jpg");
                      SaveImageData(imageBytes, naming(photoInTemplateNumb));
                    }
                    break;
                default:
                    Debug.WriteLine("bug on switch which template in ImageAndSave");
                    break;
            }
            
        }

        public static byte[] LoadImageData(string filePath)

        {
            FileStream fs = new FileStream(filePath, FileMode.Open, System.IO.FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);

            byte[] imageBytes = br.ReadBytes((int) fs.Length);

            br.Close();

            fs.Close();

            return imageBytes;
        }

        public static ImageSource CreateImage(byte[] imageData,
            int decodePixelWidth, int decodePixelHeight)

        {
            if (imageData == null) return null;

            BitmapImage result = new BitmapImage();

            result.BeginInit();

            if (decodePixelWidth > 0)
            {
                result.DecodePixelWidth = decodePixelWidth;
            }

            if (decodePixelHeight > 0)
            {
                result.DecodePixelHeight = decodePixelHeight;
            }

            result.StreamSource = new MemoryStream(imageData);
            result.CreateOptions = BitmapCreateOptions.None;
            result.CacheOption = BitmapCacheOption.Default;

            result.EndInit();

            return result;
        }


        private static void SaveImageData(byte[] imageData, string filePath)

        {
            //if filepath not exist create one
            FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(imageData);

            bw.Close();

            fs.Close();
        }

        static public byte[] GetEncodedImageData(ImageSource image,
            string preferredFormat)

        {
            byte[] result = null;

            BitmapEncoder encoder = null;

            switch (preferredFormat.ToLower())

            {
                case ".jpg":

                case ".jpeg":

                    encoder = new JpegBitmapEncoder();

                    break;

                case ".png":

                    encoder = new PngBitmapEncoder();

                    break;
            }


            if (image is BitmapSource)

            {
                MemoryStream stream = new MemoryStream();

                encoder.Frames.Add(
                    BitmapFrame.Create(image as BitmapSource));

                encoder.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                result = new byte[stream.Length];

                BinaryReader br = new BinaryReader(stream);

                br.Read(result, 0, (int) stream.Length);

                br.Close();

                stream.Close();
            }
            return result;
        }

        public static string naming(int numb)
        {
            string p1 = Environment.CurrentDirectory;
            string p2 = "resize";
            string p3 = ("resize" + numb.ToString() + ".jpg");
            return System.IO.Path.Combine(p1, p2, p3);
        }
    }
}