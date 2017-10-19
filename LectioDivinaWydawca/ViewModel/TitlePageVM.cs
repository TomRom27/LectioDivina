using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LectioDivina.Model;

namespace LectioDivina.Wydawca.ViewModel
{
    public class TitlePageVM : ViewModelBase
    {
        private TitlePage titlepage;

        public TitlePageVM(TitlePage titlepage)
        {
            this.titlepage = titlepage;
        }

        public DateTime SundayDate
        {
            get { return titlepage.SundayDate; }
            set
            {
                if (!titlepage.SundayDate.Equals(value))
                {
                    titlepage.SundayDate = value;
                    RaisePropertyChanged(() => this.SundayDate);
                }
            }
        }

        public string WeekInvocation
        {
            get { return titlepage.WeekInvocation; }
            set
            {
                titlepage.WeekInvocation = value;
                RaisePropertyChanged(() => this.WeekInvocation);
            }
        }

        public string WeekDescription
        {
            get { return titlepage.WeekDescription; }
            set
            {
                titlepage.WeekDescription = value;
                RaisePropertyChanged(() => this.WeekDescription);
            }
        }

        public string WeekPictureName
        {
            get { return titlepage.WeekPictureName; }
            set
            {
                titlepage.WeekPictureName = value;
                RaisePropertyChanged(() => this.WeekPictureName);
            }
        }

        public string WeekShortContemplationName
        {
            get { return titlepage.WeekShortContemplationName; }
            set
            {
                titlepage.WeekShortContemplationName = value;
                RaisePropertyChanged(() => this.WeekShortContemplationName);
            }
        }

        public bool IsPictureFromShortContemplation
        {
            get { return titlepage.IsPictureFromShortContemplation; }
            set
            {
                titlepage.IsPictureFromShortContemplation = value;
                RaisePropertyChanged(() => this.IsPictureFromShortContemplation);
            }
        }

        public bool IsPictureFromOwnFile
        {
            get { return !this.IsPictureFromShortContemplation; }
            set
            {
                this.IsPictureFromShortContemplation = !value;
                RaisePropertyChanged(() => this.IsPictureFromOwnFile);
            }
        }

        public string LectioTemplateFile
        {
            get { return titlepage.LectioTemplateFile; }
            set
            {
                titlepage.LectioTemplateFile = value;
                RaisePropertyChanged(() => this.LectioTemplateFile);
            }
        }

        public string LectioTargetFile
        {
            get { return titlepage.LectioTargetFile; }
            set
            {
                titlepage.LectioTargetFile = value;
                RaisePropertyChanged(() => this.LectioTargetFile);
            }
        }

        public string LectioTargetFolder
        {
            get { return titlepage.LectioTargetFolder; }
            set
            {
                titlepage.LectioTargetFolder = value;
                RaisePropertyChanged(() => this.LectioTargetFolder);
            }
        }

        public string LectioEbookSourceFolder
        {
            get { return titlepage.LectioEbookSourceFolder; }
            set
            {
                titlepage.LectioEbookSourceFolder = value;
                RaisePropertyChanged(() => this.LectioEbookSourceFolder);
            }
        }


        public string LectioEbookTargetFile
        {
            get { return titlepage.LectioEbookTargetFile; }
            set
            {
                titlepage.LectioEbookTargetFile = value;
                RaisePropertyChanged(() => this.LectioEbookTargetFile);
            }
        }

    }
}
