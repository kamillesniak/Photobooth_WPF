using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PhotoboothWpf
{
    class LayTemplate
    {
        static public void foreground3(string printPath)
        {

            var firstImage = System.Drawing.Image.FromFile(ReSize.naming(1));

            var secondImage = System.Drawing.Image.FromFile(ReSize.naming(2));

            var thirdImage = System.Drawing.Image.FromFile(ReSize.naming(3));

            string tempPath = System.IO.Path.Combine(ActualTemplateDirectory(), "foreground_3.png");
            var foreground = System.Drawing.Image.FromFile(tempPath);

            tempPath = System.IO.Path.Combine(ActualTemplateDirectory(), "empty.png");
            var empty = System.Drawing.Image.FromFile(tempPath);

            //   Bitmap changedImage = new Bitmap(Convert.ToInt32(1024), Convert.ToInt32(1024), System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            using (Graphics grfx = Graphics.FromImage(empty))
            {
                grfx.DrawImage(firstImage, 50, 80);
                grfx.DrawImage(secondImage, 50, 492);
                grfx.DrawImage(thirdImage, 50, 906);
                grfx.DrawImage(firstImage, 645, 80);
                grfx.DrawImage(secondImage, 645, 492);
                grfx.DrawImage(thirdImage, 645, 906);
                grfx.DrawImage(foreground, 0, 0);


                empty.Save(printPath);
            }
        }
        static public string ActualTemplateDirectory()
        {
            string p1 = Environment.CurrentDirectory;
            string p2 = "templates";
            return System.IO.Path.Combine(p1, p2);
        }
        static public string PrintsDirectory()
        {
            string p1 = Actual.FilePath();
            string p2 = "prints";
            return System.IO.Path.Combine(p1, p2);
        }

    }
}
