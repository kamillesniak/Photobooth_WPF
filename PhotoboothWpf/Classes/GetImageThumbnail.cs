using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EOSDigital.API;
using EOSDigital.SDK;
using ImageSource = EOSDigital.SDK.ImageSource;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = EOSDigital.SDK.Size;


namespace PhotoboothWpf.Classes
{
    class GetImageThumbnail
    {
        public string thumbnailPath { get; set; }

        private int photoNumberInTemplate;

        private int photoNumber;
        public void GetThumbnailPath()
        {           
            var save = new SavePhoto(1);
            photoNumber = save.PhotoNumberJustTaken();
            string photoName = save.photoNaming(photoNumber);
            thumbnailPath = Path.Combine(Actual.FilePath(), photoName);
        }
    }
}
