using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectioDivina.Model
{
    public class OneDayContemplation
    {
        public OneDayContemplation()
        {
            Day = DateTime.MinValue;
            Title = "";
            DayDescription = "";
            ReadingReference = "";
            ReadingText = "";
            Contemplation1 = "";
            Contemplation2 = "";
            Contemplation3 = "";
            Contemplation4 = "";
            Contemplation5 = "";
            Contemplation6 = "";
            Prayer = "";
        }
        public DateTime Day { get; set; }
        public string Title { get; set; }
        public string DayDescription { get; set; }
        public string ReadingReference { get; set; }
        public string ReadingText { get; set; }
        public string Contemplation1 { get; set; }
        public string Contemplation2 { get; set; }
        public string Contemplation3 { get; set; }
        public string Contemplation4 { get; set; }
        public string Contemplation5 { get; set; }
        public string Contemplation6 { get; set; }
        public string Prayer { get; set; }
        public DateTime LastSave { get; set; }

    }
}
