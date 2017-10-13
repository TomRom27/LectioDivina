using LectioDivina.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LectioDivina.Service
{
    public class OnJestEbookMaker
    {
        private string ebookFilesPath;
        private LectioDivinaWeek lectioWeek;

        public event EventHandler<NotificationEventArgs> Notification;

        public OnJestEbookMaker(string ebookFilesPath, LectioDivinaWeek lectioWeek)
        {
            this.ebookFilesPath = ebookFilesPath;
            this.lectioWeek = lectioWeek;
        }

        public void GenerateEbook(string ebookOutputFile)
        {
            string templateFileName = GetFileWithNamePattern(ebookFilesPath, "template*.html");
            OnNotification("startuje tworzenie ebooka na podstawie szablonu " + templateFileName);

            OnJestPostMaker2 postCreator = new OnJestPostMaker2(Properties.Settings.Default.OnJestPostOneDayKey, Properties.Settings.Default.OnJestPostTemplate);
            string sundayPost = postCreator.CreateContentFromTemplate(lectioWeek.Sunday);
            string mondayPost = postCreator.CreateContentFromTemplate(lectioWeek.Monday);
            string tuesdayPost = postCreator.CreateContentFromTemplate(lectioWeek.Tuesday);
            string wednesdayPost = postCreator.CreateContentFromTemplate(lectioWeek.Wednesday);
            string thursdayPost = postCreator.CreateContentFromTemplate(lectioWeek.Thursday);
            string fridayPost = postCreator.CreateContentFromTemplate(lectioWeek.Friday);
            string saturdayPost = postCreator.CreateContentFromTemplate(lectioWeek.Saturday);

            string htmlContent = postCreator.ReadTemplate(templateFileName);

            HtmlText htmlText = new HtmlText();
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[sundayPost]", sundayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[mondayPost]", mondayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[tuesdayPost]", tuesdayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[wednesdayPost]", wednesdayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[thursdayPost]", thursdayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[fridayPost]", fridayPost);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[saturdayPost]", saturdayPost);

            htmlContent = htmlText.ReplaceAllText(htmlContent, "[sundayTitle]", lectioWeek.Sunday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[mondayTitle]", lectioWeek.Monday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[tuesdayTitle]", lectioWeek.Tuesday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[wednesdayTitle]", lectioWeek.Wednesday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[thursdayTitle]", lectioWeek.Thursday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[fridayTitle]", lectioWeek.Friday.Title);
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[saturdayTitle]", lectioWeek.Saturday.Title);

            htmlContent = htmlText.ReplaceAllText(htmlContent, "[sundayDate]", Localization.Date2PlStr(lectioWeek.Sunday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[mondayDate]", Localization.Date2PlStr(lectioWeek.Monday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[tuesdayDate]", Localization.Date2PlStr(lectioWeek.Tuesday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[wednesdayDate]", Localization.Date2PlStr(lectioWeek.Wednesday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[thursdayDate]", Localization.Date2PlStr(lectioWeek.Thursday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[fridayDate]", Localization.Date2PlStr(lectioWeek.Friday.Day));
            htmlContent = htmlText.ReplaceAllText(htmlContent, "[saturdayDate]", Localization.Date2PlStr(lectioWeek.Saturday.Day));

            string outputFileName = GetOutputFileName(templateFileName);
            SaveContent(htmlContent, outputFileName);
            GenerateMobiFile(ebookOutputFile);
        }

        private string GetFileWithNamePattern(string path, string filePattern)
        {
            string[] files = System.IO.Directory.GetFiles(path, filePattern);
            CheckNumberOfFilesAndThrowException(files, path, filePattern);

            return files[0];
        }

        private void CheckNumberOfFilesAndThrowException(string[] files, string path, string filePattern)
        {
            if (files.Length > 1)
            {
                throw new Exception("Za duzo plików '" + filePattern + "' w " + path);
            }
            else if (files.Length == 0)
            {
                throw new Exception("Nie mogę znaleźć pliku '" + filePattern + "' w " + path);
            }
        }

        private string GetOutputFileName(string templateFileName)
        {
            return templateFileName.Replace("template", "");
        }

        private void SaveContent(string content, string outputFileName)
        {
            OnNotification("zapisuje przetworzony plik do " + outputFileName);
            System.IO.File.WriteAllText(outputFileName, content);
        }

        private void GenerateMobiFile(string ebookOutputFile)
        {
            string opfFileName = GetFileWithNamePattern(ebookFilesPath, "*.opf");
            OnNotification("generuje plik mobi na podstawie pliku " + opfFileName);

            startCommand(ebookFilesPath + "\\kindlegen.exe", @" " + opfFileName + " -o ebook.mobi");
            startCommand("cmd", @"/c copy " + ebookFilesPath + "\\ebook.mobi \"" + ebookOutputFile + "\"");
        }

        private void startCommand(string command, string parameters)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            proc.EnableRaisingEvents = false;
            proc.StartInfo.WorkingDirectory = ebookFilesPath;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = command;
            proc.StartInfo.Arguments = parameters;
            proc.Start();
            proc.WaitForExit();
        }

        private void OnNotification(string notification)
        {
            if (Notification != null)
            {
                var args = new NotificationEventArgs("ebook: " + notification);
                Notification.BeginInvoke(this, args, null, null);
            }
        }
    }
}
