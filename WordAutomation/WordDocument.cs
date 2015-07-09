using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSWord = Microsoft.Office.Interop.Word;

namespace WordAutomation
{
    public enum WordFormats
    {
        WordDocX,
        WordDoc,
        Pdf
    };

    public enum ImageFormats
    {
        Jpg,
        Png,
        Gif
    };

    public class WordDocument : IDisposable
    {
        const string WindowsEndLine = "\n";
        const string WordEndLine = "\r";

        private MSWord.Application word;
        private MSWord.Document doc;

        private object missing = Type.Missing;

        public WordDocument()
        {
            word = null;
            doc = null;
        }


        public void Open(string path, bool isVisible)
        {
            EnsureToClose();

            word = new MSWord.Application();
            word.Visible = isVisible;

            object readOnly = false;
            object isVisibleObj = word.Visible;
            object filename = path;
            doc = word.Documents.Open(ref filename, ref missing, ref readOnly, ref missing, ref missing, ref missing,
                                    ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisibleObj,
                                    ref missing, ref missing, ref missing, ref missing);
            MakeDocEditable();
        }

        private void MakeDocEditable()
        {
            // Word 2013 and later can open in reading view where edit is not possible
            // so we need to change the mode to "normal"
            if (word.Version.CompareTo("14.0") >= 0)
                if (word.ActiveWindow.View.Type == MSWord.WdViewType.wdReadingView)
                    word.ActiveWindow.View.Type = MSWord.WdViewType.wdPrintView;
        }

        ~WordDocument()  // destructor
        {
            this.Dispose(false);
        }

        #region IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EnsureToClose();
            }
        }
        #endregion

        public void ReplaceText(string textToReplace, string replacingText)
        {
            EnsureDocument();

            replacingText = CorrectEndLines(replacingText);

            // there is a limit for both text to find and text to replace in Word
            // here we adress only the later issue - we have two different methods to replace text depending 
            // on it's length
            if (String.IsNullOrEmpty(replacingText) || (replacingText.Length < 255))
                ReplaceShortText(textToReplace, replacingText);
            else
                ReplaceLongText(textToReplace, replacingText);
        }

        public void ReplaceShortText(string textToReplace, string replacingText)
        {

            object findText = textToReplace;
            object replaceText = replacingText;

            object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
            object forward = true;
            object matchAllWord = true;

            MSWord.Range rng = doc.Content;

            rng.Find.ClearFormatting();

            rng.Find.Execute(ref findText, ref missing, ref matchAllWord, ref missing, ref missing, ref missing,
                            ref forward, ref missing, ref missing, ref replaceText, ref replaceAll, ref missing,
                            ref missing, ref missing, ref missing);
        }

        /// <summary>
        ///  text to replace can't be empty !!!
        /// </summary>
        /// <param name="textToReplace"></param>
        /// <param name="replacingText"></param>
        public void ReplaceLongText(string textToReplace, string replacingText)
        {
            EnsureDocument();

            object findText = textToReplace;
            object replaceText = "^c";

            System.Windows.Forms.Clipboard.SetText(replacingText);

            object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
            object forward = true;
            object matchAllWord = true;

            MSWord.Range rng = doc.Content;

            rng.Find.ClearFormatting();

            rng.Find.Execute(ref findText, ref missing, ref matchAllWord, ref missing, ref missing, ref missing,
                            ref forward, ref missing, ref missing, ref replaceText, ref replaceAll, ref missing,
                            ref missing, ref missing, ref missing);
        }

        public void RemoveParagraphWithText(string textToRemove)
        {
            EnsureDocument();

            foreach (MSWord.Paragraph paragraph in doc.Paragraphs)
            {
                if (paragraph.Range.Text.IndexOf(textToRemove) >= 0)
                {
                    paragraph.Range.Delete();
                    break;
                }
            }
        }

        public void SetItalicForHtmTag(string tagName)
        {
            SetFontWithinHtmlTag(tagName, delegate(MSWord.Font font) { font.Italic = 1; });
        }

        public void SetBoldForHtmTag(string tagName)
        {
            SetFontWithinHtmlTag(tagName, delegate(MSWord.Font font) { font.Bold = 1; });
        }

        public void SetUnderlineForHtmTag(string tagName)
        {
            SetFontWithinHtmlTag(tagName, delegate(MSWord.Font font) { font.Underline = MSWord.WdUnderline.wdUnderlineSingle; });
        }
        public void SaveAs(string newName)
        {
            EnsureDocument();

            SaveAsDifferentFormat(newName, WordFormats.WordDocX);
        }

        public void Save()
        {
            EnsureDocument();

            doc.Save();
        }


        public void SaveAsDifferentFormat(string newName, WordFormats format)
        {
            EnsureDocument();

            object fileName = newName;
            object formatObj;

            switch (format)
            {
                case WordFormats.WordDoc:
                    {
                        formatObj = MSWord.WdSaveFormat.wdFormatDocument97;
                        break;
                    }
                case WordFormats.Pdf:
                    {
                        formatObj = MSWord.WdSaveFormat.wdFormatPDF;
                        break;
                    }
                default:
                    {
                        formatObj = MSWord.WdSaveFormat.wdFormatDocumentDefault;
                        break;
                    }
            }

            doc.SaveAs(ref fileName, ref formatObj, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

        }


        public void ReplaceTextWithImageFromFile(string textToReplace, string imageFilename)
        {
            object findText = textToReplace;
            object replaceText = "";

            object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceOne;
            object forward = true;
            object matchAllWord = true;

            MSWord.Range rng = doc.Range(doc.Content.Start, doc.Content.End);
            MSWord.Find findInRange = rng.Find;


            findInRange.ClearFormatting();

            bool found = findInRange.Execute(ref findText, ref missing, ref matchAllWord, ref missing, ref missing, ref missing,
                            ref forward, ref missing, ref missing, ref replaceText, ref replaceAll, ref missing,
                            ref missing, ref missing, ref missing);
            if (found)
            {
                // ensure the valid filename
                if (!System.IO.Path.IsPathRooted(imageFilename))
                    imageFilename = System.IO.Path.Combine(Environment.CurrentDirectory, imageFilename);

                var shape = rng.InlineShapes.AddPicture(imageFilename, missing, true, missing);
                RescaleImageToDocumentWidth(shape, rng);
            }
        }

        public void ExtractImage(int index, string saveAsName, ImageFormats imageFormat)
        {
            EnsureDocument();

            if (System.Threading.Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new Exception("This operation can only be executed in STA thread");

            if (index > doc.InlineShapes.Count)
                throw new Exception(String.Format("There is no image with index {0}. Image count is {1}", index, doc.InlineShapes.Count));

            MSWord.InlineShape inlineShape = doc.InlineShapes[index];

            if (inlineShape == null)
                throw new Exception("Unexpectly image object in the document is null");

            // the image in the Word document, can have size re-scaled to display
            // but we want to have it in original format, so we will scale it to original size
            var originalScale = inlineShape.ScaleWidth;
            inlineShape.ScaleWidth = 100;

            // copy to clipboard
            inlineShape.Select();
            word.Selection.Copy();

            // revert to Word scale
            inlineShape.ScaleWidth = originalScale;

            if (System.Windows.Forms.Clipboard.GetDataObject() != null)
            {
                System.Windows.Forms.IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
                if (data.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                {
                    System.Drawing.Image image = (System.Drawing.Image)data.GetData(System.Windows.Forms.DataFormats.Bitmap, true);

                    System.Drawing.Imaging.ImageFormat drwImageFormat;
                    switch (imageFormat)
                    {
                        case ImageFormats.Png: { drwImageFormat = System.Drawing.Imaging.ImageFormat.Png; break; }
                        case ImageFormats.Gif: { drwImageFormat = System.Drawing.Imaging.ImageFormat.Gif; break; }
                        default: { drwImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg; break; }
                    }

                    image.Save(saveAsName, drwImageFormat);

                }
                else
                    throw new Exception("The data in the clipboard is not an image");

            }
            else
                throw new Exception("Nothing found in the clipboard to save");
        }

        public List<string> GetSpellingErrors()
        {
            EnsureDocument();

            List<String> spellingErrorList = new List<string>();
            if (doc.SpellingErrors.Count > 0)
                foreach (MSWord.Range sError in doc.SpellingErrors)
                {
                    var pageNumber = (int)sError.Information[Microsoft.Office.Interop.Word.WdInformation.wdActiveEndAdjustedPageNumber]; //wdActiveEndPageNumber
                    spellingErrorList.Add(String.Format("Strona {0}: {1}", pageNumber, sError.Text));
                }

            return spellingErrorList;
        }

        private void RescaleImageToDocumentWidth(MSWord.InlineShape image, MSWord.Range parentRange)
        {
            var availableWidth = parentRange.PageSetup.PageWidth - parentRange.PageSetup.LeftMargin -
                                 parentRange.PageSetup.RightMargin;
            if (image.Width < availableWidth)
            {
                int scaleFactor = Convert.ToInt32(100 * (availableWidth) / image.Width);
                image.ScaleWidth = image.ScaleHeight = scaleFactor;
            }
        }

        public void Close()
        {
            EnsureToClose();
        }

        private void EnsureDocument()
        {
            if (doc == null)
                throw new Exception("Document object is not initialized. You must open it first.");
        }

        private void EnsureToClose()
        {
            try
            {
                if (doc != null)
                    ((Microsoft.Office.Interop.Word._Document)doc).Close();

                doc = null;

                object no = false;
                if (word != null)
                    ((Microsoft.Office.Interop.Word._Application)word).Quit(ref no, ref no, ref no);
                word = null;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception when closing Word:\n" + ex.Message);
                // we supress any errors as the exception may be caused by the fact, that user has closed the document already 
            }
        }


        private delegate void SetFont(MSWord.Font font);

        private void SetFontWithinHtmlTag(string tagName, SetFont setFont)
        {
            int start = doc.Content.Start;
            bool found = true;
            while (found)
            {
                found = false;
                int openingTagPos = OneTimeFindAndReplace(start, GetOpeningTag(tagName), "");
                if (openingTagPos >= 0)
                {
                    int closingTagPos = OneTimeFindAndReplace(openingTagPos, GetClosingTag(tagName), "");
                    if (closingTagPos >= 0)
                    {
                        MSWord.Range fontRange = doc.Range((object)openingTagPos, (object)closingTagPos);
                        setFont(fontRange.Font);

                        found = true;
                    }
                }
            }
        }

        private int OneTimeFindAndReplace(int start, string textToReplace, string replacingText)
        {
            object findText = textToReplace;
            object replaceText = replacingText;

            object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceOne;
            object forward = true;
            object matchAllWord = true;

            MSWord.Range rng = doc.Range((object)start, doc.Content.End);
            MSWord.Find findInRange = rng.Find;


            findInRange.ClearFormatting();


            bool found = findInRange.Execute(ref findText, ref missing, ref matchAllWord, ref missing, ref missing, ref missing,
                            ref forward, ref missing, ref missing, ref replaceText, ref replaceAll, ref missing,
                            ref missing, ref missing, ref missing);
            if (found)
                return rng.Start;
            else
                return -1;
        }

        private string GetClosingTag(string tagName)
        {
            return "</" + tagName + ">";
        }

        private string GetOpeningTag(string tagName)
        {
            return "<" + tagName + ">";
        }

        private string CorrectEndLines(string replacingText)
        {
            // Word uses single \r as a paragraph end, so we must replace Windows style end lines with Word style

            while ((replacingText!= null) && (replacingText.IndexOf(WindowsEndLine) >= 0))
                replacingText = replacingText.Replace(WindowsEndLine, WordEndLine);

            return replacingText;
        }

    }

    // http://www.codeproject.com/Articles/21247/Word-Automation
}