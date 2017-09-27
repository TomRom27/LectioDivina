
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocMaker;
using DocMaker.WordAutomation;

namespace LectioDivina.Service
{
    public class LectioDivinaGenerator
    {
        public event EventHandler<NotificationEventArgs> Notification;

        public string BoldTag1 = "STRONG";
        public string BoldTag2 = "b";
        public string ItalicTag1 = "EM";
        public string ItalicTag2 = "i";
        public string UnderlineTag1 = "U";
        public string HtmlWhitespace = "&nbsp;";
        public string ParagraphTagOpenning = "<P>";
        public string ParagraphTagClosing = "</P>";
        public string LineBreak = "<BR>";

        public void GenerateLectio(string templateFilename, string picturepath, string targetFilename, Model.LectioDivinaWeek lectioDivina, bool showWord)
        {
            if (!File.Exists(templateFilename))
                throw new Exception("Nie znaleziono szablonu Lectio Divina: " + templateFilename);

            if (!File.Exists(picturepath))
                throw new Exception("Nie znaleziono pliku z obrazkiem");

            using (IDocMaker doc = ResolveDocMaker())
            {
                OnNotification("Otwieram szablon i zapisuję pod nową nazwą");
                doc.Open(templateFilename, showWord);

                doc.SaveAs(targetFilename);

                OnNotification("Teraz tytuł i obrazek");
                ReplaceTextForTitle(lectioDivina.Title, picturepath, doc);

                OnNotification("Teraz niedziela");
                ReplaceTextForOneDay(Properties.Settings.Default.SundayKey, lectioDivina.Sunday, doc);
                OnNotification("Teraz poniedziałek");
                ReplaceTextForOneDay(Properties.Settings.Default.MondayKey, lectioDivina.Monday, doc);
                OnNotification("Teraz wtorek");
                ReplaceTextForOneDay(Properties.Settings.Default.TuesdayKey, lectioDivina.Tuesday, doc);
                OnNotification("Teraz środa");
                ReplaceTextForOneDay(Properties.Settings.Default.WednesdayKey, lectioDivina.Wednesday, doc);
                OnNotification("Teraz czwartek");
                ReplaceTextForOneDay(Properties.Settings.Default.ThursdayKey, lectioDivina.Thursday, doc);
                OnNotification("Teraz piątek");
                ReplaceTextForOneDay(Properties.Settings.Default.FridayKey, lectioDivina.Friday, doc);
                OnNotification("Teraz sobota");
                ReplaceTextForOneDay(Properties.Settings.Default.SaturdayKey, lectioDivina.Saturday, doc);

                OnNotification("Zmieniam kroje pisma w rozważaniach");
                ApplyFormatting(doc);

                OnNotification("Zapisuję po zmianach");
                doc.Save();

                if (Properties.Settings.Default.CheckSpellingErrors)
                {
                    // check spelling errors (but do not crash, if it fails)
                    try
                    {
                        NotifyOnSpellingErrors(doc);
                    }
                    catch
                    {
                    }
                }

                doc.Close();
            }
        }

        private IDocMaker ResolveDocMaker()
        {
            var o = new WordDocument();
            return o;
        }

        public void GenerateOneDayLectio(string templateFilename, string dayKey, Model.OneDayContemplation contemplation, Action afterCallback)
        {
            if (!File.Exists(templateFilename))
                throw new Exception("Nie znaleziono szablonu: " + templateFilename);

            using (IDocMaker doc = ResolveDocMaker())
            {
                doc.Open(templateFilename, true);

                doc.SaveAs(System.IO.Path.GetTempFileName());
                ReplaceTextForOneDay(dayKey, contemplation, doc);

                ApplyFormatting(doc);
                doc.Save();

                if (afterCallback != null)
                    afterCallback();
                doc.Close();
            }
        }

        private void ReplaceTextForTitle(Model.TitlePage titlePage, string picturepath, IDocMaker doc)
        {
            doc.ReplaceText(MakeKey(Properties.Settings.Default.WeekInvocationKey), titlePage.WeekInvocation);
            doc.ReplaceText(MakeKey(Properties.Settings.Default.WeekDescriptionKey), titlePage.WeekDescription);
            doc.ReplaceText(MakeKey(Properties.Settings.Default.IssueMonthKey), Localization.Date2PlStr(titlePage.SundayDate));
            doc.ReplaceText(MakeKey(Properties.Settings.Default.IssueYearKey), (titlePage.SundayDate.Year.ToString()));
            doc.ReplaceTextWithImageFromFile(MakeKey(Properties.Settings.Default.WeekPicture), picturepath);
        }

        private void ApplyFormatting(IDocMaker doc)
        {
            doc.SetBoldForHtmTag(BoldTag1);
            doc.SetBoldForHtmTag(BoldTag2);
            doc.SetItalicForHtmTag(ItalicTag1);
            doc.SetItalicForHtmTag(ItalicTag2);
            doc.SetUnderlineForHtmTag(UnderlineTag1);
            doc.ReplaceShortText(HtmlWhitespace, " ");
            doc.ReplaceShortText(ParagraphTagOpenning,"");
            doc.ReplaceShortText(ParagraphTagClosing, "");
            doc.ReplaceShortText(LineBreak, "");
        }

        private void ReplaceTextForOneDay(string dayKey, Model.OneDayContemplation contemplation, IDocMaker doc)
        {
            string key;
            // [sunday_date], 
            doc.ReplaceText(MakeKey(dayKey, Properties.Settings.Default.DayDateKey), Localization.Date2PlStr(contemplation.Day));
            // [sunday_name]
            doc.ReplaceText(MakeKey(dayKey, Properties.Settings.Default.DayNameKey), Localization.Date2PlDayName(contemplation.Day));
            // sunday_description
            doc.ReplaceText(MakeKey(dayKey, Properties.Settings.Default.DayDescriptionKey), contemplation.DayDescription);
            // [sunday_title]
            doc.ReplaceText(MakeKey(dayKey, Properties.Settings.Default.DayTitleKey), contemplation.Title);
            //[sunday_reading_ref]
            doc.ReplaceText(MakeKey(dayKey, Properties.Settings.Default.DayReadingRefKey), contemplation.ReadingReference);
            //[sunday_contemplation1]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation1Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation1))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation1);
            //[sunday_contemplation2]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation2Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation2))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation2);
            //[sunday_contemplation3]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation3Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation3))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation3);
            //[sunday_contemplation4]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation4Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation4))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation4);
            //[sunday_contemplation5]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation5Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation5))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation5);
            //[sunday_contemplation6]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation6Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation6))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Contemplation6);
            //[sunday_prayer]
            key = MakeKey(dayKey, Properties.Settings.Default.DayPrayerKey);
            if (String.IsNullOrEmpty(contemplation.Prayer))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.Prayer);

            //[sunday_reading_text]         
            key = MakeKey(dayKey, Properties.Settings.Default.DayReadingTextKey);
            if (String.IsNullOrEmpty(contemplation.ReadingText))
                doc.RemoveParagraphWithText(key);
            else
                doc.ReplaceText(key, contemplation.ReadingText);
        }


        private void NotifyOnSpellingErrors(IDocMaker doc)
        {
            List<string> spellingErrors;

            spellingErrors = doc.GetSpellingErrors();
            if (spellingErrors.Count > 0)
            {
                string errorNotification = String.Format("Znaleziono {0} błędy w pisowni", spellingErrors.Count);
                for (int i = 0; i <= spellingErrors.Count - 1; i++)
                    errorNotification = errorNotification + "\r\n" +
                                   (i + 1).ToString() + ". " + spellingErrors[i];

                OnNotification(errorNotification);
            }
        }


        private string MakeKey(string key)
        {
            if (!key.StartsWith("["))
                key = "[" + key;
            if (!key.EndsWith("]"))
                key = key + "]";

            return key;
        }

        private string MakeKey(string dayKey, string itemKey)
        {
            return MakeKey(dayKey + "_" + itemKey);
        }

        private void OnNotification(string notification)
        {
            if (Notification != null)
            {
                var args = new NotificationEventArgs(notification);
                Notification.BeginInvoke(this, args, null, null);
            }
        }
    }
}


/*
[week_title]
[week_invocation]
[issue_month] [issue_year]
[week_picture]

[sunday_date], [sunday_name]
[sunday_title]
[sunday_reading_ref]
[sunday_contemplation1]
[sunday_contemplation2]
[sunday_contemplation3]
[sunday_contemplation4]
[sunday_contemplation5]
[sunday_contemplation6]
[sunday_prayer]

[sunday_reading_text] 
 
*/