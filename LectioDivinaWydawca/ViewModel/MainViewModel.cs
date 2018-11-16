using System;
using System.Collections.Generic;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LectioDivina.Model;
using LectioDivina.Service;
using LectioDivinaWydawca;
using LectioDivina.Wydawca.Model;

using MvvmLight.Extensions;
using System.Collections.ObjectModel;

namespace LectioDivina.Wydawca.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 
        private ILectioDataService dataService;
        private IDialogService dialogService;

        public MainViewModel(ILectioDataService dataService, IDialogService dialogService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            this.dataService = dataService;
            this.dialogService = dialogService;

            CreateCommands();
            InitiateData();
            SubscribeMessages();
            Log("Start");
        }

        private void InitiateData()
        {
            var multiWeek = dataService.LoadMulti();
            Weeks = new ObservableCollection<OneWeekViewModel>(multiWeek.Weeks.Select((w) => new OneWeekViewModel(dataService, dialogService, w)));

            IsDirty = false;
        }

        #region VM properties
        private ObservableCollection<OneWeekViewModel> weeks;
        public ObservableCollection<OneWeekViewModel> Weeks
        {
            get { return this.weeks; }
            set { this.weeks = value; }
        }

        public string Logs { get; set; }

        public string ExeVersion
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(4); }
        }

        private OneWeekViewModel selectedWeek;
        public OneWeekViewModel SelectedWeek
        {
            get
            {
                return selectedWeek;
            }
            set
            {
                if (selectedWeek != null)
                    selectedWeek.IsSelectedL = false;
                selectedWeek = value;
                selectedWeek.IsSelectedL = true;
            }
        }

        private bool IsDirty
        {
            get
            {
                bool isD = false;

                foreach (var w in Weeks)
                    isD = isD || w.IsDirty;
                return isD;
            }
            set
            {
                foreach (var w in Weeks)
                    w.IsDirty = value;
            }
        }
        #endregion
        #region Commands

        public RelayCommand AddWeek { get; set; }
        public RelayCommand Remove { get; set; }
        public RelayCommand Save { get; set; }
        public RelayCommand CloseApp { get; set; }
        #endregion

        #region private methods
        private void Log(string msg)
        {
            if (!String.IsNullOrEmpty(Logs))
                Logs = Logs + "\r\n";
            Logs = Logs + DateTime.Now.ToString("HH.mm.ss.fff") + " " + msg;
            RaisePropertyChanged(() => Logs);

        }

        private void SubscribeMessages()
        {
            MessengerInstance.Register<Notification.RemoveWeekMessage>(this, (action) => RemoveGivenWeek(action));
        }


        private void CreateCommands()
        {
            AddWeek = new RelayCommand(AddOneWeek);
            CloseApp = new RelayCommand(Close);
            Save = new RelayCommand(SaveLectio);
        }

        private void AddOneWeek()
        {
            var newWeekVM = new OneWeekViewModel(this.dataService, this.dialogService, new IdWeek() { Week = new LectioDivinaWeek() });
            newWeekVM.TitlePage.SundayDate = dataService.GetNearestSunday();
            newWeekVM.TitlePage.WeekDescription = "<nowy tydzieñ>";
            newWeekVM.Sunday.Day = newWeekVM.TitlePage.SundayDate;
            newWeekVM.Monday.Day = newWeekVM.Sunday.Day.AddDays(1);
            newWeekVM.Tuesday.Day = newWeekVM.Sunday.Day.AddDays(2);
            newWeekVM.Wednesday.Day = newWeekVM.Sunday.Day.AddDays(3);
            newWeekVM.Thursday.Day = newWeekVM.Sunday.Day.AddDays(4);
            newWeekVM.Friday.Day = newWeekVM.Sunday.Day.AddDays(5);
            newWeekVM.Saturday.Day = newWeekVM.Sunday.Day.AddDays(6);
            Weeks.Add(newWeekVM);

            Log("Dodano nowy tydzieñ na pozycjê " + (Weeks.IndexOf(newWeekVM) + 1).ToString());
        }

        private void RemoveGivenWeek(Notification.RemoveWeekMessage msg)
        {
            var weekVM = FindInWeeksById(msg.WeekId);
            if (weekVM != null)
            {
                Weeks.Remove(weekVM);
                Log("Usuniêto tydzieñ: " + weekVM.TitlePage.WeekDescription);
            }
            else
            {
                Log("B£¥D: nie uda³o siê usun¹æ tygodnia " + msg.WeekId.ToString());
            }
        }

        private void Close()
        {
            if (IsDirty)
            {
                dialogService.ShowMessage("Rozwa¿ania by³y zmienione. Zapisaæ przed zakoñczeniem?",
                    "Potwierdzenie",
                    buttonConfirmText: "Tak", buttonCancelText: "Nie",
                    afterHideCallback: (confirmed) =>
                    {
                        if (confirmed)
                            SaveLectio();
                        App.Current.Shutdown();
                    });
            }
            else
                App.Current.Shutdown();
        }


        private void SaveLectio()
        {
            try
            {
                List<IdWeek> weeks = new List<IdWeek>();
                foreach (var wVM in Weeks)
                    weeks.Add(new IdWeek() { Id = wVM.Id, Week = wVM.LectioWeek });

                dataService.Save(new LectioDivinaMultiWeek() { Weeks = weeks });
                IsDirty = false;
                Log("Wszystko zapisane");
            }
            catch (Exception ex)
            {
                string msg = "Nie uda³o siê zapisaæ Lectio:\r\n" + ex.Message;
                Log(msg);
                dialogService.ShowError(msg, "B³¹d", "OK", null);
            }
        }

        private OneWeekViewModel FindInWeeksById(long id)
        {
            var week = Weeks.FirstOrDefault((w) => w.Id.Equals(id));

            return week;
        }
        #endregion
    }
}