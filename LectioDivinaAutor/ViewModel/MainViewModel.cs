using System;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LectioDivina.Model;
using LectioDivina.Service;

using MvvmLight.Extensions;

namespace LectioDivina.Autor.ViewModel
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
        private OneDayContemplation contemplation;
        private IContemplationDataService dataService;
        private IDialogService dialogService;
        private bool isDirty;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IContemplationDataService dataService, IDialogService dialogService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            //
            this.dataService = dataService;
            this.dialogService = dialogService;

            CreateCommands();
            InitiateData();
        }

        private void InitiateData()
        {
            contemplation = dataService.Load();
            IsDirty = false;
        }

        #region properties

        public string ExeVersion
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(3); }
        }

        public DateTime Day
        {
            get { return contemplation.Day; }
            set
            {
                if (!contemplation.Day.Equals(value))
                {
                    contemplation.Day = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Day);
                }
            }
        }

        public string DayDescription
        {
            get { return contemplation.DayDescription; }
            set
            {
                if (!contemplation.DayDescription.Equals(value))
                {
                    contemplation.DayDescription = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.DayDescription);
                }
            }
        }


        public string Title
        {
            get { return contemplation.Title; }
            set
            {
                if (!contemplation.Title.Equals(value))
                {
                    contemplation.Title = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Title);
                }
            }
        }

        public string ReadingReference
        {
            get { return contemplation.ReadingReference; }
            set
            {
                if (!contemplation.ReadingReference.Equals(value))
                {
                    contemplation.ReadingReference = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.ReadingReference);
                }
            }
        }

        public string ReadingText
        {
            get { return contemplation.ReadingText; }
            set
            {
                if (!contemplation.ReadingText.Equals(value))
                {
                    contemplation.ReadingText = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.ReadingText);
                }
            }
        }

        public string Contemplation1
        {
            get { return contemplation.Contemplation1; }
            set
            {
                if (!contemplation.Contemplation1.Equals(value))
                {
                    contemplation.Contemplation1 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation1);
                }
            }
        }

        public string Contemplation2
        {
            get { return contemplation.Contemplation2; }
            set
            {
                if (!contemplation.Contemplation2.Equals(value))
                {
                    contemplation.Contemplation2 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation2);
                }
            }
        }

        public string Contemplation3
        {
            get { return contemplation.Contemplation3; }
            set
            {
                if (!contemplation.Contemplation3.Equals(value))
                {
                    contemplation.Contemplation3 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation3);
                }
            }
        }

        public string Contemplation4
        {
            get { return contemplation.Contemplation4; }
            set
            {
                if (!contemplation.Contemplation4.Equals(value))
                {
                    contemplation.Contemplation4 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation4);
                }
            }
        }

        public string Contemplation5
        {
            get { return contemplation.Contemplation5; }
            set
            {
                if (!contemplation.Contemplation5.Equals(value))
                {
                    contemplation.Contemplation5 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation5);
                }
            }
        }

        public string Contemplation6
        {
            get { return contemplation.Contemplation6; }
            set
            {
                if (!contemplation.Contemplation6.Equals(value))
                {
                    contemplation.Contemplation6 = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Contemplation6);
                }
            }
        }

        public string Prayer
        {
            get { return contemplation.Prayer; }
            set
            {
                if (!contemplation.Prayer.Equals(value))
                {
                    contemplation.Prayer = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => this.Prayer);
                }
            }
        }


        public DateTime LastSave
        {
            get { return contemplation.LastSave; }
            set
            {
                if (!contemplation.LastSave.Equals(value))
                {
                    contemplation.LastSave = value;
                    RaisePropertyChanged(() => this.LastSave);
                    RaisePropertyChanged(() => this.LastSaveString);
                }
            }
        }

        public string LastSaveString
        {
            get { return contemplation.LastSave.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

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

        #endregion properties

        #region Commands

        public RelayCommand Save { get; set; }
        public RelayCommand Send { get; set; }
        public RelayCommand CloseApp { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand TryOut { get; set; }
        
        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveContemplation);
            Send = new RelayCommand(SendContemplation);
            Clear = new RelayCommand(ClearContemplation);
            TryOut = new RelayCommand(GenerateWordContemplation);
            CloseApp = new RelayCommand(Close);
        }

        private void GenerateWordContemplation()
        {
            try
            {
                LectioDivinaGenerator lectioGenerator = new LectioDivinaGenerator();

                lectioGenerator.GenerateOneDayLectio(
                    DetermineTemplateFilename(), 
                    Properties.Settings.Default.OneDayKey, contemplation, ShowAfterGeneration);
            }
            catch (Exception ex)
            {
                dialogService.ShowError("Coœ posz³o Ÿle:\r\n" +
                                        GetNestedExceptionMessage(ex),
                    "Problem", "Ok",
                    () => { });
            }
        }

        private string DetermineTemplateFilename()
        {
            string filename;
            // app launch folder
            filename = System.IO.Path.Combine(Environment.CurrentDirectory, Properties.Settings.Default.OneDayLectioTemplate);
            if (System.IO.File.Exists(filename))
                return filename;
            // if not - app data in user folder
            filename = System.IO.Path.Combine(dataService.GetAppDataFolder(), Properties.Settings.Default.OneDayLectioTemplate);
            if (System.IO.File.Exists(filename))
                return filename;

            return Properties.Settings.Default.OneDayLectioTemplate;
        }

        private void ShowAfterGeneration()
        {
            dialogService.ShowMessage("Operacja zakoñczona\r\nPrzejrzy tekst Word'zie, a nastêpnie kliknij OK w tym oknie.", "Informacja");
        }


        private void ClearContemplation()
        {
            // Message with custom buttons and callback action.
            dialogService.ShowMessage("Wyczyœciæ wszystkie pola ?",
                "Uwaga",
                buttonConfirmText: "Tak", buttonCancelText: "Nie",
                afterHideCallback: (confirmed) =>
                {
                    if (confirmed)
                    {
                        contemplation = dataService.GetEmpty();
                        // raise property changed for all properties
                        RaisePropertyChanged(() => Day);
                        RaisePropertyChanged(() => DayDescription);
                        RaisePropertyChanged(() => Title);
                        RaisePropertyChanged(() => ReadingReference);
                        RaisePropertyChanged(() => ReadingText);
                        RaisePropertyChanged(() => Contemplation1);
                        RaisePropertyChanged(() => Contemplation2);
                        RaisePropertyChanged(() => Contemplation3);
                        RaisePropertyChanged(() => Contemplation4);
                        RaisePropertyChanged(() => Contemplation5);
                        RaisePropertyChanged(() => Prayer);
                    }
                    else
                    {
                        // User has pressed the "cancel" button
                        // (or has discared the dialog box).
                        // ...
                    }
                });
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
                        {
                            SaveContemplation();
                            App.Current.Shutdown();
                        }
                    });
            }
            else
                App.Current.Shutdown();
        }

        private void SendContemplation()
        {
            // if the contemplation day is older then some days from today
            // it's suspicious
            if (contemplation.Day.AddDays(Properties.Settings.Default.OldDateWarningDays) < DateTime.Today)
            {
                dialogService.ShowMessage(
                        "Data rozwa¿ania " + contemplation.Day.ToString("yyyy-MM-dd") +
                        " wydaje siê zbyt odleg³a.\r\nWys³aæ rozwa¿anie mimo wszystko?",
                        "Potwierdzenie",
                        buttonConfirmText: "Tak", buttonCancelText: "Nie",
                        afterHideCallback: (confirmed) =>
                        {
                            if (confirmed)
                                SendAndHandleErrors();
                        });
            }
            else
            {
                dialogService.ShowMessage("Wys³aæ rozwa¿ania ?",
                    "Potwierdzenie",
                    buttonConfirmText: "Tak", buttonCancelText: "Nie",
                    afterHideCallback: (confirmed) =>
                    {
                        if (confirmed)
                        {
                            SendAndHandleErrors();
                        }
                    });
            }

        }

        private void SendAndHandleErrors()
        {
            try
            {
                dialogService.SetBusy();

                LectioDivina.Service.MailTransport transport = new MailTransport();
                transport.SendToPublisher(contemplation);

                dialogService.ShowMessage("Wysy³anie zakoñczone", "Informacja");
            }
            catch (Exception ex)
            {
                dialogService.ShowError("Nie uda³o siê wys³aæ rozwa¿ania.\r\n" +
                                        GetNestedExceptionMessage(ex) +
                                        "\r\nSpróbuj ponownie i jeœli problem siê powtarza, skontaktuj siê z T.R.",
                    "Problem", "Ok",
                    () => { });
            }
            finally
            {
                dialogService.SetNormal();
            }

        }

        private void SaveContemplation()
        {
            try
            {
                LastSave = DateTime.Now;
                dataService.Save(contemplation);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                dialogService.ShowError("Nie uda³o siê zapisaæ rozwa¿ania.\r\n" +
                                        GetNestedExceptionMessage(ex),
                    "Problem", "Ok",
                    () => { });
            }
        }

        private string GetNestedExceptionMessage(Exception ex)
        {
            string msg = "";
            while (ex != null)
            {
                if (!String.IsNullOrEmpty(msg))
                    msg = msg + "\r\n";
                msg = msg + ex.Message;
                ex = ex.InnerException;
            }
            return msg;
        }
        #endregion


    }
}