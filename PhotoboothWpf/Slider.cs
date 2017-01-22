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
using System.IO;
using System.Timers;

namespace PhotoboothWpf
{
    class Slider
    {
        public string imgName { get; set; }
        public string imagePath { get; set; }
        public FileInfo [] imageNumbers { get; set; }

        public string actualFolder { get; set; }

        public Slider()
        {         
            actualFolder = chooseImageToShow();
            imageNumbers = new DirectoryInfo(actualFolder).GetFiles("*.jpg");

            imgName = randomIMG();
            imagePath = ImageLocation();
           
            
        }

        public string chooseImageToShow()
        {
            string todayFolder = Actual.DateNow();
            var tempNumbers = new DirectoryInfo(todayFolder).GetFiles("*.jpg");

            if (tempNumbers.Length<=2)
            {
                return "sample";
            }
            else
            {
                return todayFolder;
            }
        }
     private string randomIMG()
        {            
            int length = imageNumbers.Length-1;

            Random r = new Random();
            int rInt = r.Next(0, length);

            string randomImage = imageNumbers[rInt].Name;
            return randomImage;
        }   

        private string ImageLocation()
        {
            string p1 = Environment.CurrentDirectory;
            var path = System.IO.Path.Combine(p1, actualFolder, imgName);
            return path;
        }

    }
}
