using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DocMaker.Word.Automation;
using DocMaker.Word.Common;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LectioDivina.Model;
using LectioDivina.Service;
using LectioDivinaWydawca;
using LectioDivina.Wydawca.Model;

using MvvmLight.Extensions;


namespace LectioDivina.Wydawca.ViewModel
{
    public class OneWeekViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 
        //private LectioDivinaWeek lectioDivinaWeek;
        private ILectioDataService dataService;
        private IDialogService dialogService;
        private bool isDirty;


        public OneWeekViewModel()
        {

        }

        public OneWeekViewModel(ILectioDataService dataService, IDialogService dialogService)
        {
            this.dataService = dataService;
            this.dialogService = dialogService;
            //this.loggingService = loggingService;

            CreateCommands();
            LectioWeek = dataService.Load();
            Id = DateTime.Now.Ticks;
            InitiateData();
        }

        public OneWeekViewModel(ILectioDataService dataService, IDialogService dialogService, IdWeek weekData)
        {
            this.dataService = dataService;
            this.dialogService = dialogService;
            //this.loggingService = loggingService;

            CreateCommands();

            LectioWeek = weekData.Week;
            Id = weekData.Id;
            InitiateData();
        }

        private void InitiateData()
        {
            TitlePage = new TitlePageVM(LectioWeek.Title);

            Sunday = new OneDayContemplationVM(LectioWeek.Sunday);
            Monday = new OneDayContemplationVM(LectioWeek.Monday);
            Tuesday = new OneDayContemplationVM(LectioWeek.Tuesday);
            Wednesday = new OneDayContemplationVM(LectioWeek.Wednesday);
            Thursday = new OneDayContemplationVM(LectioWeek.Thursday);
            Friday = new OneDayContemplationVM(LectioWeek.Friday);
            Saturday = new OneDayContemplationVM(LectioWeek.Saturday);

            RefreshTargetFileProperty();

            IsDirty = false;
        }

        public string Logs { get; set; }

        #region VM properties
        public LectioDivinaWeek LectioWeek { get; private set; }

        public long Id { get; private set; }

        private TitlePageVM titlePage;
        public TitlePageVM TitlePage
        {
            get { return titlePage; }
            set
            {
                titlePage = value;
                RaisePropertyChanged(() => this.TitlePage);

                titlePage.PropertyChanged += titlePage_PropertyChanged;
            }
        }

        void titlePage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsDirty = true;
            if (e.PropertyName.Equals("WeekInvocation") ||
                e.PropertyName.Equals("WeekDescription") ||
                e.PropertyName.Equals("LectioTargetFolder")) // todo replace string with expression
            {
                RefreshTargetFileProperty();
            }
        }

        #region week days objects
        private OneDayContemplationVM sunday;
        public OneDayContemplationVM Sunday
        {
            get { return sunday; }
            set
            {
                sunday = value;
                RaisePropertyChanged(() => this.Sunday);

                sunday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM monday;
        public OneDayContemplationVM Monday
        {
            get { return monday; }
            set
            {
                monday = value;
                RaisePropertyChanged(() => this.Monday);

                monday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM tuesday;
        public OneDayContemplationVM Tuesday
        {
            get { return tuesday; }
            set
            {
                tuesday = value;
                RaisePropertyChanged(() => this.Tuesday);

                tuesday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM wednesday;
        public OneDayContemplationVM Wednesday
        {
            get { return wednesday; }
            set
            {
                wednesday = value;
                RaisePropertyChanged(() => this.Wednesday);

                wednesday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM thursday;
        public OneDayContemplationVM Thursday
        {
            get { return thursday; }
            set
            {
                thursday = value;
                RaisePropertyChanged(() => this.Thursday);

                thursday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM friday;
        public OneDayContemplationVM Friday
        {
            get { return friday; }
            set
            {
                friday = value;
                RaisePropertyChanged(() => this.Friday);

                friday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM saturday;
        public OneDayContemplationVM Saturday
        {
            get { return saturday; }
            set
            {
                saturday = value;
                RaisePropertyChanged(() => this.Saturday);

                saturday.PropertyChanged += oneDay_PropertyChanged;
            }
        }
        #endregion // week days objects

        void oneDay_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }
        #endregion

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(() => IsDirty);
                    Save.RaiseCanExecuteChanged();
                }
            }
        }

        #region Commands
        public RelayCommand Save { get; set; }
        public RelayCommand Send { get; set; }
        public RelayCommand Receive { get; set; }
        //public RelayCommand CloseApp { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand OpenLectioTarget { get; set; }
        public RelayCommand GenerateLectioTarget { get; set; }
        public RelayCommand SelectTemplate { get; set; }
        public RelayCommand SelectTarget { get; set; }
        public RelayCommand SelectEbookSource { get; set; }
        public RelayCommand SelectPicture { get; set; }
        public RelayCommand SelectShortContemplation { get; set; }

        public RelayCommand Remove { get; set; }
        #endregion

        #region private methods

        private void Log(string msg)
        {
            if (!String.IsNullOrEmpty(Logs))
                Logs = Logs + "\r\n";
            Logs = Logs + DateTime.Now.ToString("HH.mm.ss.fff") + " " + msg;
            RaisePropertyChanged(() => Logs);
        }

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveLectio);
            Send = new RelayCommand(SendLectioToServer);
            Receive = new RelayCommand(ReceiveLectiosFromServer);
            Clear = new RelayCommand(ClearLectio);
            //CloseApp = new RelayCommand(Close);
            OpenLectioTarget = new RelayCommand(OpenLectioTargetInWord);
            GenerateLectioTarget = new RelayCommand(GenerateLectioTargetDoc);
            SelectTemplate = new RelayCommand(SelectLectioTemplate);
            SelectTarget = new RelayCommand(SelectLectioTarget);
            SelectEbookSource = new RelayCommand(SelectLectioEbookSource);
            SelectPicture = new RelayCommand(SelectPictureFile);
            SelectShortContemplation = new RelayCommand(SelectShortContemplationFile);
            Remove = new RelayCommand(RemoveThisWeek);
        }

        private void RemoveThisWeek()
        {
            dialogService.ShowMessage("Usunąć tydzień " + this.TitlePage.WeekDescription + " ?",
                "Uwaga",
                buttonConfirmText: "Tak", buttonCancelText: "Nie",
                afterHideCallback: (confirmed) =>
                {
                    if (confirmed)
                    {
                        MessengerInstance.Send<Notification.RemoveWeekMessage>(new Notification.RemoveWeekMessage(Id));
                    }
                });
        }

        private void SelectShortContemplationFile()
        {
            string template = dialogService.SelectFile("Wybierz pliku Rozważań krótkich", "*.doc;*.docx");

            if (!String.IsNullOrEmpty(template))
                TitlePage.WeekShortContemplationName = template;
        }

        private void SelectPictureFile()
        {
            string picture = dialogService.SelectFile("Wybierz obrazek do bieżącego Lectio Divina", "*.jpg;*.png");

            if (!String.IsNullOrEmpty(picture))
                TitlePage.WeekPictureName = picture;
        }

        private void SelectLectioTemplate()
        {
            string template = dialogService.SelectFile("Wybierz szablon pliku Lectio Divina", "*.doc;*.docx");

            if (!String.IsNullOrEmpty(template))
                TitlePage.LectioTemplateFile = template;
        }

        private void SelectLectioTarget()
        {
            string folder = dialogService.SelectFolder("Wybierz katalog, w którym umieszczony zostanie bieżący plik Lectio Divina", TitlePage.LectioTargetFolder);
            if (!String.IsNullOrEmpty(folder))
                TitlePage.LectioTargetFolder = folder;
        }

        private void SelectLectioEbookSource()
        {
            string folder = dialogService.SelectFolder("Wybierz katalog zawierajacy pliki html do tworzenia ebooka Lectio Divina", TitlePage.LectioEbookSourceFolder);
            if (!String.IsNullOrEmpty(folder))
                TitlePage.LectioEbookSourceFolder = folder;
        }

        private void RefreshTargetFileProperty()
        {
            TitlePage.LectioTargetFile = dataService.ProposeLectioTargetName(TitlePage.LectioTargetFolder, TitlePage.WeekInvocation, TitlePage.WeekDescription);

        }
        private void OpenLectioTargetInWord()
        {
            try
            {
                System.Diagnostics.Process.Start(TitlePage.LectioTargetFile);
            }
            catch (Exception ex)
            {
                string msg = "Nie udało się otworzyć Lectio:\r\n" + ex.Message;
                Log(msg);
                dialogService.ShowError(msg, "Błąd", "OK", null);
            }
        }

        private void GenerateLectioTargetDoc()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            bool showFinishInfo = true;

            System.Threading.Tasks.Task.Factory
            /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                List<string> issues = LectioWeek.Validate();

                if (issues.Count > 0)
                {
                    Log("Braki w Lectio\r\n" + issues.Aggregate((a, b) => a + "\r\n" + b));

                    dialogService.ShowMessage("W rozważaniach są braki. Tworzyć Lectio mimo wszystko?",
                                            "Potwierdzenie",
                                            buttonConfirmText: "Tak", buttonCancelText: "Nie",
                                            afterHideCallback: (confirmed) =>
                                            {
                                                if (confirmed)
                                                    GenerateLectio();
                                                else
                                                {
                                                    Log("Lectio nie zostało utworzone ze względu na braki.");
                                                    showFinishInfo = false;
                                                    return;
                                                }


                                            });
                }
                else
                    GenerateLectio();

            }, CancellationToken.None, TaskCreationOptions.LongRunning, scheduler)
            /* when completed, display response */
            .ContinueWith((t) =>
            {
                dialogService.SetNormal();
                if (t.Exception != null)
                {
                    string msg = "Nie udało się utworzyć Lectio:\r\n" + t.Exception.InnerException.Message;
                    Log(msg);
                    dialogService.ShowError(msg, "Błąd", "OK", null);
                }
                else if (showFinishInfo)
                {
                    Log("Zakończono tworzenie Lectio");
                    dialogService.ShowMessage("Zakończono tworzenie Lectio", "Informacja");
                }
            }, CancellationToken.None, TaskContinuationOptions.None, scheduler);

        }

        private void GenerateLectio()
        {
            dialogService.SetBusy();
            Log("Rozpoczęto tworzenie Lectio. Czekaj...");

            if (TitlePage.IsPictureFromShortContemplation)
                ExtractPictureFromShortContemplation();

            try
            {
                if (!String.IsNullOrEmpty(TitlePage.LectioEbookSourceFolder))
                {
                    var ebookLectioGenerator = new OnJestEbookMaker(TitlePage.LectioEbookSourceFolder, LectioWeek);

                    ebookLectioGenerator.Notification += Progress_Notification;
                    TitlePage.LectioEbookTargetFile = ebookLectioGenerator.GenerateEbook();
                }
            }
            catch (Exception exception)
            {
                TitlePage.LectioEbookTargetFile = "";
                Log("Błąd podczas generowania ebook-a\r\n" + exception.Message);
            }

            var lectioGenerator = new LectioDivinaGenerator();

            lectioGenerator.Notification += Progress_Notification;
            lectioGenerator.GenerateLectio(TitlePage.LectioTemplateFile, TitlePage.WeekPictureName, TitlePage.LectioTargetFile,
                LectioWeek,
                Properties.Settings.Default.ShowWord);

        }

        void Progress_Notification(object sender, NotificationEventArgs e)
        {
            Log(e.Notification);
        }

        private void Close()
        {
            if (IsDirty)
            {
                dialogService.ShowMessage("Rozważania były zmienione. Zapisać przed zakończeniem?",
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

        private void ClearLectio()
        {
            dialogService.ShowMessage("Wyczyścić wszystkie pola?",
                "Uwaga",
                buttonConfirmText: "Tak", buttonCancelText: "Nie",
                afterHideCallback: (confirmed) =>
                {
                    if (confirmed)
                    {
                        Log("Wszystke pola wyczyszczone.");
                        TitlePage.SundayDate = dataService.GetNearestSunday();
                        TitlePage.WeekDescription = "";
                        ReplaceDay(Sunday, new OneDayContemplation());
                        ReplaceDay(Monday, new OneDayContemplation());
                        ReplaceDay(Tuesday, new OneDayContemplation());
                        ReplaceDay(Wednesday, new OneDayContemplation());
                        ReplaceDay(Thursday, new OneDayContemplation());
                        ReplaceDay(Friday, new OneDayContemplation());
                        ReplaceDay(Saturday, new OneDayContemplation());
                    }
                });
        }

        private void SendLectioToServer()
        {
            System.Threading.Tasks.Task.Factory
            /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                List<string> issues = LectioWeek.Validate();

                if (issues.Count > 0)
                {
                    Log("Braki w Lectio\r\n" + issues.Aggregate((a, b) => a + "\r\n" + b));

                    dialogService.ShowMessage("W rozważaniach są braki. Wysłać Lectio mimo wszystko?",
                                            "Potwierdzenie",
                                            buttonConfirmText: "Tak", buttonCancelText: "Nie",
                                            afterHideCallback: (confirmed) =>
                                            {
                                                if (confirmed)
                                                    SendLectio();
                                                else
                                                {
                                                    Log("Lectio nie zostało wysłane ze względu na braki.");
                                                    return;
                                                }


                                            });
                }
                else
                    SendLectio();

            })
            /* when completed, display response */
            .ContinueWith((t) =>
            {
                if (t.Exception != null)
                {
                    string msg = "Coś poszło źle przy wysyłaniu Lectio:\r\n" + t.Exception.InnerException.Message;
                    Log(msg);
                    dialogService.ShowError(msg, "Błąd", "OK", null);
                }
                else
                {
                    Log("Zakończono wysyłanie Lectio");
                    dialogService.ShowMessage("Zakończono wysyłanie Lectio", "Informacja");
                }
            });
        }

        private void SendLectio()
        {
            var poster = new OnJestPostSender();

            poster.Notification += Progress_Notification;

            poster.SendLectio(LectioWeek.Title.WeekPictureName, LectioWeek.Title.LectioTargetFile, TitlePage.LectioEbookTargetFile,
                                LectioWeek);
        }

        private void ReceiveLectiosFromServer()
        {
            int count = 0;
            System.Threading.Tasks.Task.Factory
                .StartNew(() =>
                {
                    Log("Odbieram  z serwera Lectio od autorów");
                    MailTransport transport = new MailTransport();
                    transport.Notification += Progress_Notification;
                    List<OneDayContemplation> contemplations = null;

                    if (Properties.Settings.Default.LectiosFromWeekOnly)
                    {
                        // if the mode is Week, that it does NOT matter dates of lectios, so we ignore the dates and we take all
                        contemplations = transport.RetrieveContemplations(null, null);
                    }
                    else
                    {
                        // if the mode is not Week, then we get lectios for given week only
                        contemplations = transport.RetrieveContemplations(TitlePage.SundayDate, TitlePage.SundayDate.AddDays(6));
                    }
                    foreach (var contemplation in contemplations)
                    {
                        AddContemplationToWeek(contemplation);
                        count++;
                    }

                })
                .ContinueWith((t) =>
                {
                    if (t.Exception != null)
                    {
                        string msg = "Nie udało się odebrać Lectio:\r\n" + t.Exception.InnerException.Message;
                        Log(msg);
                        dialogService.ShowError(msg, "Błąd", "OK", null);
                    }
                    else
                    {
                        Log(String.Format("Odebrano {0} rozważań", count));
                        dialogService.ShowMessage(String.Format("Zakończono odbieranie Lectio, odebrano {0}. ", count), "Informacja");
                    }
                }
                );
        }

        private void AddContemplationToWeek(OneDayContemplation contemplation)
        {
            switch (contemplation.Day.DayOfWeek)
            {
                case DayOfWeek.Sunday: { ReplaceDay(Sunday, contemplation); break; }
                case DayOfWeek.Monday: { ReplaceDay(Monday, contemplation); break; }
                case DayOfWeek.Tuesday: { ReplaceDay(Tuesday, contemplation); break; }
                case DayOfWeek.Wednesday: { ReplaceDay(Wednesday, contemplation); break; }
                case DayOfWeek.Thursday: { ReplaceDay(Thursday, contemplation); break; }
                case DayOfWeek.Friday: { ReplaceDay(Friday, contemplation); break; }
                case DayOfWeek.Saturday: { ReplaceDay(Saturday, contemplation); break; }
            }
        }

        private void ReplaceDay(OneDayContemplationVM target, OneDayContemplation source)
        {
            target.Day = source.Day;
            target.DayDescription = source.DayDescription;
            target.Title = source.Title;
            target.ReadingReference = source.ReadingReference;
            target.ReadingText = source.ReadingText;
            target.Contemplation1 = source.Contemplation1;
            target.Contemplation2 = source.Contemplation2;
            target.Contemplation3 = source.Contemplation3;
            target.Contemplation4 = source.Contemplation4;
            target.Contemplation5 = source.Contemplation5;
            target.Contemplation6 = source.Contemplation6;
            target.Prayer = source.Prayer;
        }

        private void SaveLectio()
        {
            try
            {
                dataService.Save(LectioWeek);
                IsDirty = false;
                Log("Lectio zapisane");
            }
            catch (Exception ex)
            {
                string msg = "Nie udało się zapisać Lectio:\r\n" + ex.Message;
                Log(msg);
                dialogService.ShowError(msg, "Błąd", "OK", null);
            }
        }


        private void ExtractPictureFromShortContemplation()
        {
            // we will save the picture under the name from WeekPictureName
            // if it is ewmpty, we must create it

            if (String.IsNullOrEmpty(TitlePage.WeekPictureName))
                TitlePage.WeekPictureName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TitlePage.WeekShortContemplationName),
                                            "obrazek.jpg");
            Log("Biorę obrazek z " + TitlePage.WeekShortContemplationName);
            Log("zapisuję jako " + TitlePage.WeekPictureName);

            WordDocument word = new WordDocument();
            word.Open(TitlePage.WeekShortContemplationName, false);
            word.ExtractImage(Properties.Settings.Default.ShortContemplationPictureNumber, TitlePage.WeekPictureName, ImageFormats.Jpg);
            word.Close();
        }


        #endregion
    }
}
