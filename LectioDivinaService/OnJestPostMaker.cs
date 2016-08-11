using LectioDivina.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LectioDivina.Service
{
    class OnJestPostMaker
    {
        private HtmlText htmlText;
        string dayKey;
        string postTemplate;

        public OnJestPostMaker(string dayKey, string postTemplate) {

            if (!File.Exists(postTemplate))
                throw new Exception("Nie znaleziono pliku z szablonem postu: " + postTemplate);

            this.dayKey = dayKey;
            this.postTemplate = postTemplate;
            htmlText = new HtmlText();
        }

        public string CreateContentFromTemplate(OneDayContemplation oneDay)
        {
            return CreateText(oneDay);
        }

        private string CreateText(OneDayContemplation contemplation)
        {

            string postText = ReadTemplate(postTemplate);

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
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation1Key, contemplation.Contemplation1);

            //[sunday_contemplation2]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation2Key, contemplation.Contemplation2);

            //[sunday_contemplation3]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation3Key, contemplation.Contemplation3);

            //[sunday_contemplation4]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation4Key, contemplation.Contemplation4);

            //[sunday_contemplation5]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation5Key, contemplation.Contemplation5);

            //[sunday_contemplation6]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayContemplation6Key, contemplation.Contemplation6);
            
            //[sunday_prayer]
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayPrayerKey, contemplation.Prayer);
            
            //[sunday_reading_text]         
            replaceOptionalTags(ref postText, Properties.Settings.Default.DayReadingTextKey, contemplation.ReadingText);

            return postText;
        }

        private void replaceOptionalTags(ref string text, string itemKey, string content)
        {
            string key = MakeKey(dayKey, itemKey);
            if (String.IsNullOrEmpty(content))
                HtmlRemoveTagWithText(ref text, key);
            else
                HtmlReplaceAllText(ref text, key, content);
        }

        private void HtmlRemoveTagWithText(ref string template, string text)
        {
            template = htmlText.RemoveTagWithText(template, text);
        }

        private void HtmlReplaceAllText(ref string template, string search, string replace)
        {
            template = htmlText.ReplaceAllText(template, search, replace);
        }

        public string ReadTemplate(string templateFile)
        {
            using (StreamReader sr = new StreamReader(templateFile))
            {
                return sr.ReadToEnd();
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
    }
}
