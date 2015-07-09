using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using LectioDivina.Model;

namespace LectioDivina.Wydawca.ViewModel
{
    public class OneDayContemplationVM : ViewModelBase
    {
        private OneDayContemplation contemplation;

        public OneDayContemplationVM(OneDayContemplation contemplation)
        {
            this.contemplation = contemplation;
        }

        public DateTime Day
        {
            get { return contemplation.Day; }
            set
            {
                if (!contemplation.Day.Equals(value))
                {
                    contemplation.Day = value;
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
                    RaisePropertyChanged(() => this.Prayer);
                }
            }
        }
        
    }
}
