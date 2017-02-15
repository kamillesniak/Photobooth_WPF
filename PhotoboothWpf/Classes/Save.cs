using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhotoboothWpf
{
    class SavePhoto
    {

        public int PhotoNumber { get; set; }
        public string FolderDirectory { get; set; }

        public string PhotoName { get; set; }

        public string PhotoDirectory { get; set; }


        public SavePhoto(int numb)
        {
            PhotoNumber = numb;
            FolderDirectory = currenFolderDirectory();
            PhotoName = actualPhotoName();
            PhotoDirectory = nPhotoDirectory();

        }
        public bool checkIfExsit (string fileName)
        {
             string currFile = Path.Combine(FolderDirectory, fileName);
            return File.Exists(currFile);
        }
        public string currenFolderDirectory()
        {
            string p1 = Environment.CurrentDirectory;
            string p2 = Actual.DateNow();

            var path = Path.Combine(p1, p2);
            return path;
        }

        public string actualPhotoName()
        {
            int number = PhotoNumber;
            string photoName = photoNaming(number);
            while(checkIfExsit(photoName)==true)
            {
                number++;
                photoName = photoNaming(number);
            }
            return photoName;
        }

        public int PhotoNumberJustTaken()
        {
            int number = PhotoNumber;
            string photoName = photoNaming(number);
            while (checkIfExsit(photoName) == true)
            {
                number++;
                photoName = photoNaming(number);
            }
            number--;
            return number;

        }
        public string photoNaming(int number)
        {
            string temp = "IMG_" + number+".jpg";
            return temp;
        }
       public string nPhotoDirectory()
        {
            string p1 = Actual.FilePath();
            string p2 = PhotoName;
            return Path.Combine(p1, p2);
        }
       
    }
    class SavePrints
    {
        public int PrintNumber { get; set; }
        public string PrintsFolderDirectory { get; set; }

        public string PrintName { get; set; }

        public string PrintDirectory { get; set; }

        public SavePrints(int numb)
        {
            PrintNumber = numb;
            PrintsFolderDirectory = currenFolderDirectory();
            PrintName = actualPhotoName();
            PrintDirectory = nPhotoDirectory();

        }
        public bool checkIfExsit(string fileName)
        {
            string currFile = Path.Combine(PrintsFolderDirectory, fileName);
            return File.Exists(currFile);
        }
        public string currenFolderDirectory()
        {
            string p1 = Actual.FilePath();
            string p2 = "prints";

            var path = Path.Combine(p1, p2);
            return path;
        }

        public string actualPhotoName()
        {
            int number = PrintNumber;
            string printName = printNaming(number);
            while (checkIfExsit(printName) == true)
            {
                number++;
                printName = printNaming(number);
            }

            return printName;

        }
        public string printNaming(int number)
        {
            string temp = "print_" + number + ".jpg";
            return temp;
        }
        public string nPhotoDirectory()
        {
            string p1 = currenFolderDirectory();
            string p2 = PrintName;
            return Path.Combine(p1, p2);
        }

    }
}
