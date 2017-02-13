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
using System.Windows.Shapes;

namespace PhotoboothWpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EmailSendDialog : Window
    {
        public EmailSendDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            labelEmailQuestion.Content = question;
            textBoxEmailAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Debug.WriteLine("textBoxEmailAnswer is: " + Answer);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            textBoxEmailAnswer.SelectAll();
            textBoxEmailAnswer.Focus();
        }

        public string Answer
        {
            get { return textBoxEmailAnswer.Text; }
        }
    }
}
