using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using LectioDivina.Model;

namespace LectioDivina.Autor.ViewModel
{
    public interface IContemplationDataService
    {
        OneDayContemplation Load();
        OneDayContemplation GetEmpty();
        void Save(OneDayContemplation contemplation);
        string GetAppDataFolder();
    }

    public class ContemplationDataService : IContemplationDataService
    {
        public const string AppDataSubfolder = "LectioDivina";
        public const string DataFilename = "Contemplation.xml";

        public OneDayContemplation Load()
        {
            OneDayContemplation contemplation;

            try
            {
                EnsureDataFolder();

                using (var sr = new StreamReader(GetLocalFileName()))
                {
                    var s = sr.ReadToEnd();
                    contemplation = SerializationHelper.Deserialize<OneDayContemplation>(s);
                }
            }
            catch (Exception)
            {
                contemplation = GetEmpty();
            }

            return contemplation;
        }

        public void Save(OneDayContemplation contemplation)
        {
            string xml;

            EnsureDataFolder();

            xml = SerializationHelper.Serialize(contemplation);
            using (var sw = new System.IO.StreamWriter(GetLocalFileName()))
            {
                sw.WriteLine(xml);
            }

        }


        public OneDayContemplation GetEmpty()
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


        public string GetAppDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDataSubfolder);
        }


        private void EnsureDataFolder()
        {
            if (!Directory.Exists(GetAppDataFolder()))
                Directory.CreateDirectory(GetAppDataFolder());
        }
        private string GetLocalFileName()
        {
            return Path.Combine(GetAppDataFolder(), DataFilename);
        }

    }
}
