using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Smith.WPF.HtmlEditor;

namespace LectioDivina.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class OneDayControl : UserControl
    {
        public OneDayControl()
        {
            InitializeComponent();
        }

        static OneDayControl()
        {
            // this way we decide on what the watermark (text if the field is empty)
            // should be displated for DatePicker
            EventManager.RegisterClassHandler(typeof(DatePicker),
                DatePicker.LoadedEvent,
                new RoutedEventHandler(DatePicker_Loaded));
        }


        #region dependency properties
        #region Day Property
        public DateTime Day
        {
            get { return (DateTime)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(DateTime), typeof(OneDayControl),
                new FrameworkPropertyMetadata(DateTime.MinValue, new PropertyChangedCallback(OnDayChanged)));

        private static void OnDayChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OneDayControl editor = (OneDayControl)sender;

            if ((DateTime)e.NewValue != null)
                editor.DayOfWeek.Text = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(((DateTime)e.NewValue).DayOfWeek);
            else
                editor.DayOfWeek.Text = "";
        }
        
        #endregion

        #region DayDescription property
        public string DayDescription
        {
            get { return (string)GetValue(DayDescriptionProperty); }
            set { SetValue(DayDescriptionProperty, value); }
        }

        public static readonly DependencyProperty DayDescriptionProperty =
            DependencyProperty.Register("DayDescription", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion DayDescription

        #region DayTitle property
        public string DayTitle
        {
            get { return (string)GetValue(DayTitleProperty); }
            set { SetValue(DayTitleProperty, value); }
        }

        public static readonly DependencyProperty DayTitleProperty =
            DependencyProperty.Register("DayTitle", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion DayTitle

        #region ReadingRef property
        public string ReadingRef
        {
            get { return (string)GetValue(ReadingRefProperty); }
            set { SetValue(ReadingRefProperty, value); }
        }

        public static readonly DependencyProperty ReadingRefProperty =
            DependencyProperty.Register("ReadingRef", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion ReadingRef

        #region ReadingText property
        public string ReadingText
        {
            get { return (string)GetValue(ReadingTextProperty); }
            set { SetValue(ReadingTextProperty, value); }
        }

        public static readonly DependencyProperty ReadingTextProperty =
            DependencyProperty.Register("ReadingText", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion ReadingText

        #region Contemplation1 property
        public string Contemplation1
        {
            get { return (string)GetValue(Contemplation1Property); }
            set { SetValue(Contemplation1Property, value); }
        }

        public static readonly DependencyProperty Contemplation1Property =
            DependencyProperty.Register("Contemplation1", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation1

        #region Contemplation2 property
        public string Contemplation2
        {
            get { return (string)GetValue(Contemplation2Property); }
            set { SetValue(Contemplation2Property, value); }
        }

        public static readonly DependencyProperty Contemplation2Property =
            DependencyProperty.Register("Contemplation2", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation2

        #region Contemplation3 property
        public string Contemplation3
        {
            get { return (string)GetValue(Contemplation3Property); }
            set { SetValue(Contemplation3Property, value); }
        }

        public static readonly DependencyProperty Contemplation3Property =
            DependencyProperty.Register("Contemplation3", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation3

        #region Contemplation4 property
        public string Contemplation4
        {
            get { return (string)GetValue(Contemplation4Property); }
            set { SetValue(Contemplation4Property, value); }
        }

        public static readonly DependencyProperty Contemplation4Property =
            DependencyProperty.Register("Contemplation4", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation4

        #region Contemplation5 property
        public string Contemplation5
        {
            get { return (string)GetValue(Contemplation5Property); }
            set { SetValue(Contemplation5Property, value); }
        }

        public static readonly DependencyProperty Contemplation5Property =
            DependencyProperty.Register("Contemplation5", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation5

        #region Contemplation6 property
        public string Contemplation6
        {
            get { return (string)GetValue(Contemplation6Property); }
            set { SetValue(Contemplation6Property, value); }
        }

        public static readonly DependencyProperty Contemplation6Property =
            DependencyProperty.Register("Contemplation6", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Contemplation6

        #region Prayer property
        public string Prayer
        {
            get { return (string)GetValue(PrayerProperty); }
            set { SetValue(PrayerProperty, value); }
        }

        public static readonly DependencyProperty PrayerProperty =
            DependencyProperty.Register("Prayer", typeof(string), typeof(OneDayControl),
                new FrameworkPropertyMetadata(""));

        #endregion Prayer
        #endregion

        #region DatePicker customization
        static void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            var dp = sender as DatePicker;
            if (dp == null) return;

            var tb = GetChildOfType<DatePickerTextBox>(dp);
            if (tb == null) return;

            var wm = tb.Template.FindName("PART_Watermark", tb) as ContentControl;
            if (wm == null) return;

            wm.Content = "Wybierz datę";
        }

        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
        #endregion
    }
}
