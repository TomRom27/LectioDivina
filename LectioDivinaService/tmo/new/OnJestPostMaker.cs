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
    public class OnJestPostSender
    {

        private LectioDivina.OnJestSlowoProxy.OnJestSlowoProxy onJestProxy;
        private string tempPath;
        private OnJestPostMaker2 htmlContentCreator;

        public event EventHandler<NotificationEventArgs> Notification;

        public OnJestPostSender()
        {
            onJestProxy = new OnJestSlowoProxy.OnJestSlowoProxy();

            tempPath = System.IO.Path.GetTempPath();

            htmlContentCreator = new OnJestPostMaker2(Properties.Settings.Default.OnJestPostOneDayKey, Properties.Settings.Default.OnJestPostTemplate);
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

            string postText = htmlContentCreator.CreateContentFromTemplate(oneDay);

            onJestProxy.SendPost(Properties.Settings.Default.OnJestUser, Properties.Settings.Default.OnJestPwd, oneDay.Title,
                Properties.Settings.Default.OnJestPostCategory, oneDay.Day, postText);
        }
        #endregion

        private void SendLectioFile(string lectioWordFile, LectioDivinaWeek lectioWeek)
        {
            string fileName = CreateName(Properties.Settings.Default.OnJestLectioNameTemplate, lectioWeek);
            OnNotification("Wysyłam całość Lectio Divina jako " + fileName);
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
            OnNotification("Wysyłam obrazek jako " + picName);
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
