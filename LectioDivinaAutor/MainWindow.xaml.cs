using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

using LectioDivina.Autor.ViewModel;

namespace LectioDivina.Autor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += this_Closed;

            LoadSavedSize();
        }

        private void LoadSavedSize()
        {
            if ((Properties.Settings.Default.MWWidth != 0) && (Properties.Settings.Default.MWHeight != 0))
            {
                this.Left = Properties.Settings.Default.MWLeft;
                this.Top = Properties.Settings.Default.MWTop;
                this.Width = Properties.Settings.Default.MWWidth;
                this.Height = Properties.Settings.Default.MWHeight;
            }
        }

        private void this_Closed(object sender, EventArgs eventArgs)
        {
            // save size and pos
            Properties.Settings.Default.MWLeft = this.Left;
            Properties.Settings.Default.MWTop = this.Top;
            Properties.Settings.Default.MWWidth = this.Width;
            Properties.Settings.Default.MWHeight = this.Height;

            Properties.Settings.Default.Save();
        }
    }
}
