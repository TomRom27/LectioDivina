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

namespace LectioDivina.Wydawca.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl, ViewModel.ILoggingService
    {
        public MainView()
        {
            InitializeComponent();

            ViewModel.MainViewModel mvm = (DataContext as ViewModel.MainViewModel);
            if (mvm != null)
                mvm.LoggingService = this;
            Log("Start " + ExeVersion);
        }

        private void AddToStatus(string msg)
        {
            //if (!String.IsNullOrEmpty(LogBox.Text))
            //    msg = Environment.NewLine + msg;

            //LogBox.AppendText(msg);
            //LogBox.ScrollToEnd();
            //LogBoxScroll.ScrollToEnd();
        }

        public void Log(string msg)
        {
            msg = DateTime.Now.ToString("HH.mm.ss.fff") + " " + msg;

            if (this.Dispatcher.CheckAccess())
                AddToStatus(msg);
            else
                this.Dispatcher.Invoke(new Action(() => { AddToStatus(msg); }));
        }
        public string ExeVersion
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(4); }
        }
    }
}
