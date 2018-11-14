using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using LectioDivina.Model;
using LectioDivina.Wydawca.Model;

namespace LectioDivina.Wydawca.ViewModel
{
    public interface ILectioDataService
    {
        LectioDivinaWeek Load();
        LectioDivinaMultiWeek LoadMulti();

        void Save(LectioDivinaWeek lectioDivina);

        DateTime GetNearestSunday();
        string ProposeLectioTargetName(string folder, string weekInvocation, string weekDescription);

    }

    public class LectioDataService : ILectioDataService
    {
        public const string AppDataSubfolder = "LectioDivina";
        public const string LectioDataFilename = "LectioDivina.xml";
        public const string MultiLectioFilename = "MultiLectioDivina.xml";

        public string ProposeLectioTargetName(string folder, string weekInvocation, string weekDescription)
        {
            string proposedName = (weekInvocation + " " + weekDescription + "." + Properties.Settings.Default.LectioTargetExtension).Trim();

            proposedName = Path.GetInvalidFileNameChars().Aggregate(proposedName, (current, c) => current.Replace(c, '_'));

            return  Path.Combine(folder, proposedName);
        }

        public LectioDivinaWeek Load()
        {
            LectioDivinaWeek lectioDivina;

            try
            {
                EnsureDataFolder();

                using (var sr = new StreamReader(GetLectioFileName()))
                {
                    var s = sr.ReadToEnd();
                    lectioDivina = SerializationHelper.Deserialize<LectioDivinaWeek>(s);
                }
            }
            catch (Exception)
            {
                lectioDivina = GetEmpty();
            }

            return lectioDivina;
        }


        public LectioDivinaMultiWeek LoadMulti()
        {
            LectioDivinaMultiWeek multiLectioDivina;

            try
            {
                EnsureDataFolder();

                using (var sr = new StreamReader(GetMultiLectioFileName()))
                {
                    var s = sr.ReadToEnd();
                    multiLectioDivina = SerializationHelper.Deserialize<LectioDivinaMultiWeek>(s);
                }
            }
            catch (Exception)
            {
                multiLectioDivina = new LectioDivinaMultiWeek();
                // we load one week - as a migration from previous data
                LectioDivinaWeek oneWeek = Load();
                multiLectioDivina.Weeks.Add(new IdWeek() { Week = oneWeek });
            }

            return multiLectioDivina;
        }

        public void Save(LectioDivinaWeek lectioDivina)
        {
            string xml;

            EnsureDataFolder();

            xml = SerializationHelper.Serialize(lectioDivina);
            using (var sw = new System.IO.StreamWriter(GetLectioFileName()))
            {
                sw.WriteLine(xml);
            }

        }

        public DateTime GetNearestSunday()
        {
            DateTime day = DateTime.Today;
            DayOfWeek dayOfWeek = day.DayOfWeek;

            // we get this or next Sunday
            if (day.DayOfWeek != DayOfWeek.Sunday)
                day = day.AddDays(7 + DayOfWeek.Sunday - day.DayOfWeek);

            return day;
        }

        private LectioDivinaWeek GetEmpty()
        {
            LectioDivinaWeek lectioDivina = new LectioDivinaWeek();

            lectioDivina.Title = new TitlePage() { SundayDate = GetNearestSunday(), WeekDescription = "np. 10 Tydzień zwykły", WeekPictureName = GetDataFolderName() + @"\obrazektygodnia.jpg", LectioTemplateFile = "szablon.docx", LectioTargetFolder = ""};
            lectioDivina.Sunday = GetEmptyContemplation();
            lectioDivina.Sunday.Day = lectioDivina.Title.SundayDate;
            lectioDivina.Monday = GetEmptyContemplation();
            lectioDivina.Monday.Day = lectioDivina.Sunday.Day.AddDays(1);
            lectioDivina.Tuesday = GetEmptyContemplation();
            lectioDivina.Tuesday.Day = lectioDivina.Sunday.Day.AddDays(2);
            lectioDivina.Wednesday = GetEmptyContemplation();
            lectioDivina.Wednesday.Day = lectioDivina.Sunday.Day.AddDays(3);
            lectioDivina.Thursday = GetEmptyContemplation();
            lectioDivina.Thursday.Day = lectioDivina.Sunday.Day.AddDays(4);
            lectioDivina.Friday = GetEmptyContemplation();
            lectioDivina.Friday.Day = lectioDivina.Sunday.Day.AddDays(5);
            lectioDivina.Saturday = GetEmptyContemplation();
            lectioDivina.Saturday.Day = lectioDivina.Sunday.Day.AddDays(6);

            return lectioDivina;
        }


        private OneDayContemplation GetEmptyContemplation()
        {
            OneDayContemplation contemplation = new OneDayContemplation()
            {
                Day = DateTime.Today,
                DayDescription = "",
                Title = "",
                ReadingReference = "",
                ReadingText = "",
                Contemplation1 = "",
                Contemplation2 = "",
                Contemplation3 = "",
                Contemplation4 = "",
                Contemplation5 = "",
                Contemplation6 = "",
                Prayer = ""
            };

            return contemplation;
        }

        private void EnsureDataFolder()
        {
            if (!Directory.Exists(GetDataFolderName()))
                Directory.CreateDirectory(GetDataFolderName());
        }
        private string GetLectioFileName()
        {
            return Path.Combine(GetDataFolderName(), LectioDataFilename);
        }
        private string GetMultiLectioFileName()
        {
            return Path.Combine(GetDataFolderName(), MultiLectioFilename);
        }

        private string GetDataFolderName()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDataSubfolder);
        }

    }
}
