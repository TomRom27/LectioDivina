using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectioDivina.Model;

namespace LectioDivina.Model
{
    public class LectioDivinaWeek
    {
        public TitlePage Title { get; set; }

        public OneDayContemplation Sunday { get; set; }
        public OneDayContemplation Monday { get; set; }
        public OneDayContemplation Tuesday { get; set; }
        public OneDayContemplation Wednesday { get; set; }
        public OneDayContemplation Thursday { get; set; }
        public OneDayContemplation Friday { get; set; }
        public OneDayContemplation Saturday { get; set; }


        public List<string> Validate()
        {
            List<string> issues = new List<string>();

            issues = ValidateWeek();

            List<string> dayIssues = ValidateDay(Sunday, DayOfWeek.Sunday);
            issues.AddRange(dayIssues);

            dayIssues = ValidateDay(Monday, DayOfWeek.Monday);
            issues.AddRange(dayIssues);

            dayIssues = ValidateDay(Wednesday, DayOfWeek.Wednesday);
            issues.AddRange(dayIssues);

            dayIssues = ValidateDay(Thursday, DayOfWeek.Thursday);
            issues.AddRange(dayIssues);

            dayIssues = ValidateDay(Friday, DayOfWeek.Friday);
            issues.AddRange(dayIssues);

            dayIssues = ValidateDay(Saturday, DayOfWeek.Saturday);
            issues.AddRange(dayIssues);

            return issues;
        }

        private List<string> ValidateWeek()
        {
            List<string> issues = new List<string>();

            if ((Sunday.Day - Title.SundayDate).TotalDays != 0)
                issues.Add("Data niedzieli na panelu kontrolnym i data na zakładce Niedziela są różne, a muszą być takie same.");

            return issues;
        }


        private List<string> ValidateDay(OneDayContemplation dayContemplation, DayOfWeek expectedDay)
        {
            List<string> issues = new List<string>();
            string dayName = GetDayName(expectedDay);

            if (dayContemplation.Day.DayOfWeek != expectedDay)
                issues.Add(String.Format("{0}: zły dzień tygodnia {1}, a powinien być {0}", dayName, GetDayName(dayContemplation.Day.DayOfWeek)));

            if (dayContemplation.Day.DayOfWeek != expectedDay)
                issues.Add(String.Format("{0}: zła data {1}, a powinino być {2}", dayName, dayContemplation.Day.ToString("dd.MM.yyyy"), Title.SundayDate.AddDays(DiffFromFirstSunday(expectedDay)).ToString("dd.MM.yyyy")));

            if (String.IsNullOrEmpty(dayContemplation.ReadingReference))
                issues.Add(String.Format("{0}: brak odnośnika do czytań", dayName));

            if (String.IsNullOrEmpty(dayContemplation.ReadingText))
                issues.Add(String.Format("{0}: brak tekstu czytań", dayName));

            if (String.IsNullOrEmpty(dayContemplation.Contemplation1) &&
                String.IsNullOrEmpty(dayContemplation.Contemplation2) &&
                String.IsNullOrEmpty(dayContemplation.Contemplation3) &&
                String.IsNullOrEmpty(dayContemplation.Contemplation4) &&
                String.IsNullOrEmpty(dayContemplation.Contemplation5) &&
                String.IsNullOrEmpty(dayContemplation.Contemplation6) )
                issues.Add(String.Format("{0}: nie ma żadnego rozważania", dayName));

            if (String.IsNullOrEmpty(dayContemplation.Prayer))
                issues.Add(String.Format("{0}: brak modlitwy", dayName));

            return issues;
        }

        private string GetDayName(DayOfWeek day)
        {
            return System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(day);
        }

        private int GetDays(DateTime later, DateTime earlier)
        {
            var days = (later - earlier).TotalDays;

            return Convert.ToInt32(days);
        }

        private int DiffFromFirstSunday(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday: return 0;
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
                default: return 0;
            }
        }

    }
}
