using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Smith.WPF.HtmlEditor;

namespace SmithHtmlEditorTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            HtmlText = "thix is my <b>sample</b> text which must be long enogh in ordert o check what happens if very long";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HtmlText = "";//DateTime.Now.ToString("hh:mm:ss.fff") + " changed";

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("HtmlText"));
        }

        public string HtmlText
        {
            get { return (string)GetValue(HtmlTextProperty); }
            set { SetValue(HtmlTextProperty, value); }
        }

        public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register(
            "HtmlText", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(HtmlTextEditor.ContentHtml);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string s = HtmlTextEditor.ContentHtml;
            MessageBox.Show(s+"\r\n"+HtmlText);
        }
    }
}
