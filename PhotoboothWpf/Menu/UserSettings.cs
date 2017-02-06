using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
namespace PhotoboothWpf
{
    public class UserSettings
    {
        #region textBoxes
        public string WelcomeText { get; set; }
        public string BeforePhotoText { get; set; }
        public string FirstPhotoText { get; set; }
        public string SecondPhotoText { get; set; }
        public string ThirdPhotoText { get; set; }
        public string FourthPhotoText { get; set; }
        public string backgroundPath { get; set; }
        public string buttonsColor { get; set; }
        public string borderColor { get; set; }
        public string textBoxColor { get; set; }
        public string buttonHighlightColor { get; set; }

        public void SetNewUserSettings(string welcometxt, string b4phototxt, string firstphototxt, string secondphototxt, string thirdphototxt
            , string fourthphototxt,string nbacgroundpath, string nbuttonscolor, string nbordercolor, string ntextboxcolor
            , string nbuttonhighlightcolor)
        {
            WelcomeText = welcometxt;
            BeforePhotoText = b4phototxt;
            FirstPhotoText = firstphototxt;
            SecondPhotoText = secondphototxt;
            ThirdPhotoText = thirdphototxt;
            FourthPhotoText = fourthphototxt;
            backgroundPath = nbacgroundpath;
            buttonsColor = nbuttonscolor;
            borderColor = nbordercolor;
            textBoxColor = ntextboxcolor;
            buttonHighlightColor = nbuttonhighlightcolor;
        }
  
        #endregion
        public void SaveOptions(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(@"UserSettings.xml"))
            {
                writer.WriteStartElement("FrontEnd");
                writer.WriteElementString("WelcomeText", WelcomeText);
                writer.WriteElementString("BeforePhotoText", BeforePhotoText);
                writer.WriteElementString("FirstPhotoText", FirstPhotoText);
                writer.WriteElementString("SecondPhotoText", SecondPhotoText);
                writer.WriteElementString("ThirdPhotoText", ThirdPhotoText);
                writer.WriteElementString("FourthPhotoText", FourthPhotoText);
                writer.WriteElementString("backgroundPath", backgroundPath);
                writer.WriteElementString("buttonsColor", buttonsColor);
                writer.WriteElementString("borderColor", borderColor);
                writer.WriteElementString("textBoxColor", textBoxColor);
                writer.WriteElementString("buttonHighlightColor", buttonHighlightColor);
                writer.WriteEndElement();
                writer.Flush();
            }
        }
        public void ChangeText(string name,string value)
        {
            XDocument xdoc = XDocument.Load(@"UserSettings.xml");
            var query = from c in xdoc.Elements("FrontEnd")
                        select c;
            foreach (XElement book in query)
            {
                book.Element(name).Value = value;
            }
            xdoc.Save(@"UserSettings.xml");

        }

    }
}
