using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LectioDivina.Service
{
    public static class Localization
    {

        public static string Date2PlStr(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");

        }
        
        public static string Date2PlDayName(DateTime day)
        {
            return System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(day.DayOfWeek);
        }

        public static string Date2PlMonth(DateTime dateTime)
        {
            switch (dateTime.Month)
            {
                case 1: return "styczeń";
                case 2: return "luty";
                case 3: return "marzec";
                case 4: return "kwiecień";
                case 5: return "maj";
                case 6: return "czerwiec";
                case 7: return "lipiec";
                case 8: return "sierpień";
                case 9: return "wrzesień";
                case 10: return "październik";
                case 11: return "listopad";
                case 12: return "grudzień";
                default:
                    return "";
            }
        }
    }
}
