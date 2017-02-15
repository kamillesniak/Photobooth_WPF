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
using System.Windows.Shapes;

namespace PhotoboothWpf
{
    /// <summary>
    /// Interaction logic for RepeatPhotoDialog.xaml
    /// </summary>
    public partial class RepeatPhotoDialog : Window
    {
        public RepeatPhotoDialog()
        {
            InitializeComponent();
        }

        private void RepeatPhotoButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
