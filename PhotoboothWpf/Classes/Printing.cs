using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;

namespace PhotoboothWpf
{
    class Printing
    {
        static public void Print(string printPath,string actualPrinter, short actualNumberOfCopies)
        {
            PrintDocument pd = new PrintDocument();

            PrintController printController = new StandardPrintController();
            pd.PrintController = printController;

            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            pd.PrinterSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            pd.PrintPage += (sndr, args) =>
            {
                System.Drawing.Image i = System.Drawing.Image.FromFile(printPath);
                i.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                System.Drawing.Rectangle m = args.MarginBounds;

                if ((double)i.Width / (double)i.Height > (double)m.Width / (double)m.Height)
                {
                    m.Height = (int)((double)i.Height / (double)i.Width * (double)m.Width);
                }
                else
                {
                    m.Width = (int)((double)i.Width / (double)i.Height * (double)m.Height);
                }

                pd.DefaultPageSettings.Landscape = m.Height > m.Width;
                m.Y = (int)((((System.Drawing.Printing.PrintDocument)(sndr)).DefaultPageSettings.PaperSize.Height - m.Height) / 2);
                m.X = (int)((((System.Drawing.Printing.PrintDocument)(sndr)).DefaultPageSettings.PaperSize.Width - m.Width) / 2);
                args.Graphics.DrawImage(i, m);
            };

            pd.PrinterSettings.PrinterName = actualPrinter;
            Debug.WriteLine("actual number of copies: " + actualNumberOfCopies);
//            pd.PrinterSettings.Copies = actualNumberOfCopies;
//            pd.Print();
            for (int i = 0; i < actualNumberOfCopies; i++)
            {
                pd.Print();
            }
        }
        static public string ActualPrinter(string actualForeground, string firstprinter, string secondprinter)
        {
            if ((actualForeground == "foreground_3") || (actualForeground == "foregrund_4_paski"))
            {
                return secondprinter;
            }
            else return firstprinter;
        }
    }
}
