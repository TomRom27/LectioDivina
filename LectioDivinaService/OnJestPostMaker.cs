using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using LectioDivina.Model;
using DocMaker.Word.Automation;
using DocMaker.Word.Common;

namespace LectioDivina.Service
{
    public class OnJestPostMaker
    {

        private LectioDivina.OnJestSlowoProxy.OnJestSlowoProxy onJestProxy;
        private string tempPath;
        private HtmlText htmlText;

        public event EventHandler<NotificationEventArgs> Notification;

        public OnJestPostMaker()
        {
            onJestProxy = new OnJestSlowoProxy.OnJestSlowoProxy();

            tempPath = System.IO.Path.GetTempPath();

            htmlText = new HtmlText();
        }

        public void SendLectio(string picturepath, string lectioWordFile, LectioDivinaWeek lectioWeek)
        {
            if (!File.Exists(lectioWordFile))
                throw new Exception("Nie znaleziono pliku Lectio Divina: " + lectioWordFile);

            if (!File.Exists(picturepath))
                throw new Exception("Nie znaleziono pliku z obrazkiem");

            string postTemplate = Properties.Settings.Default.OnJestPostTemplate;
            if (!File.Exists(postTemplate))
                throw new Exception("Nie znaleziono pliku z szablonem postu: " + postTemplate);

            OnNotification("Wysyłanie rozpoczęte.");

            SendPicture(picturepath, lectioWeek);

            SendLectioFile(lectioWordFile, lectioWeek);

            SendDayPost(lectioWeek.Sunday);
            SendDayPost(lectioWeek.Monday);
            SendDayPost(lectioWeek.Tuesday);
            SendDayPost(lectioWeek.Wednesday);
            SendDayPost(lectioWeek.Thursday);
            SendDayPost(lectioWeek.Friday);
            SendDayPost(lectioWeek.Saturday);

            SendOnJestAdminInfo(lectioWeek);

            OnNotification("Wysyłanie zakończone");
        }

        private void SendOnJestAdminInfo(LectioDivinaWeek lectioWeek)
        {
            OnNotification("Wysyłam mail do admina OnJest");
            var mailer = new MailTransport();

            mailer.SendMail("Lectio Divina " + Localization.Date2PlStr(lectioWeek.Title.SundayDate), "wysłane", Properties.Settings.Default.OnJestAdminEmail, "OnJest Admin");
        }

        #region sending posts
        private void SendDayPost(OneDayContemplation oneDay)
        {
            string dayName = Localization.Date2PlDayName(oneDay.Day);
            OnNotification("Wysyłam " + dayName);

            string postText = CreatePostText(oneDay);

            onJestProxy.SendPost(Properties.Settings.Default.OnJestUser, Properties.Settings.Default.OnJestPwd, oneDay.Title, 
                Properties.Settings.Default.OnJestPostCategory, oneDay.Day, postText);
        }

        private string CreatePostText(OneDayContemplation contemplation)
        {

            // replace
            string key;
            string dayKey = Properties.Settings.Default.OnJestPostOneDayKey;
            string postText = ReadTemplate(Properties.Settings.Default.OnJestPostTemplate);

            // [sunday_date], 
            HtmlReplaceAllText(ref postText, MakeKey(dayKey, Properties.Settings.Default.DayDateKey), Localization.Date2PlStr(contemplation.Day));
            // [sunday_name]
            HtmlReplaceAllText(ref postText, MakeKey(dayKey, Properties.Settings.Default.DayNameKey), Localization.Date2PlDayName(contemplation.Day));
            // sunday_description
            HtmlReplaceAllText(ref postText, MakeKey(dayKey, Properties.Settings.Default.DayDescriptionKey), contemplation.DayDescription);
            // [sunday_title]
            HtmlReplaceAllText(ref postText, MakeKey(dayKey, Properties.Settings.Default.DayTitleKey), contemplation.Title);
            //[sunday_reading_ref]
            HtmlReplaceAllText(ref postText, MakeKey(dayKey, Properties.Settings.Default.DayReadingRefKey), contemplation.ReadingReference);
            //[sunday_contemplation1]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation1Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation1))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation1);
            //[sunday_contemplation2]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation2Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation2))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation2);
            //[sunday_contemplation3]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation3Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation3))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation3);
            //[sunday_contemplation4]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation4Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation4))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation4);
            //[sunday_contemplation5]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation5Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation5))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation5);
            //[sunday_contemplation6]
            key = MakeKey(dayKey, Properties.Settings.Default.DayContemplation6Key);
            if (String.IsNullOrEmpty(contemplation.Contemplation6))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Contemplation6);
            //[sunday_prayer]
            key = MakeKey(dayKey, Properties.Settings.Default.DayPrayerKey);
            if (String.IsNullOrEmpty(contemplation.Prayer))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.Prayer);

            //[sunday_reading_text]         
            key = MakeKey(dayKey, Properties.Settings.Default.DayReadingTextKey);
            if (String.IsNullOrEmpty(contemplation.ReadingText))
                HtmlRemoveTagWithText(ref postText, key);
            else
                HtmlReplaceAllText(ref postText, key, contemplation.ReadingText);

            return postText;
        }

        private void HtmlRemoveTagWithText(ref string template, string text)
        {
            template = htmlText.RemoveTagWithText(template, text);
        }

        private void HtmlReplaceAllText(ref string template, string search, string replace)
        {
            template = htmlText.ReplaceAllText(template, search, replace);
        }

        private string ReadTemplate(string templateFile)
        {
            using (StreamReader sr = new StreamReader(templateFile))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        private void SendLectioFile(string lectioWordFile, LectioDivinaWeek lectioWeek)
        {
            string fileName = CreateName(Properties.Settings.Default.OnJestLectioNameTemplate, lectioWeek);
            OnNotification("Wysyłam całość Lectio Divina jako "+fileName );
            string newName = System.IO.Path.Combine(tempPath, fileName);
            WordDocument word = new WordDocument();

            word.Open(lectioWordFile, false);

            word.SaveAsDifferentFormat(newName, WordFormats.Pdf);
            word.Close();

            onJestProxy.UploadFile(Properties.Settings.Default.OnJestUser, Properties.Settings.Default.OnJestPwd, newName);
        }

        private void SendPicture(string picturepath, LectioDivinaWeek lectioWeek)
        {
            string picName = CreateName(Properties.Settings.Default.OnJestPictureNameTemplate, lectioWeek);
            OnNotification("Wysyłam obrazek jako "+picName);
            string newName = System.IO.Path.Combine(tempPath, picName);

            // ensure right size
            MvvmLight.Extensions.Wpf.ImageToolHelper.DecreaseToWidth(picturepath,
                newName, Properties.Settings.Default.OnJestPictureMaxWidth, null);

            // send
            onJestProxy.UploadFile(Properties.Settings.Default.OnJestUser, Properties.Settings.Default.OnJestPwd, newName);
        }

        private string CreateName(string nameTemplate, LectioDivinaWeek lectioWeek)
        {
            return lectioWeek.Title.SundayDate.ToString(nameTemplate);
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

        /*
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
    }
}
