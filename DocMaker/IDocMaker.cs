using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMaker
{
    public interface IDocMaker : IDisposable
    {
        void Open(string path, bool isVisible);
        void ReplaceText(string textToReplace, string replacingText);
        void ReplaceShortText(string textToReplace, string replacingText);
        void ReplaceTextWithImageFromFile(string textToReplace, string imageFilename);
        void RemoveParagraphWithText(string textToRemove);
        void SetItalicForHtmTag(string tagName);
        void SetBoldForHtmTag(string tagName);
        void SetUnderlineForHtmTag(string tagName);
        List<string> GetSpellingErrors();
        void SaveAs(string newName);
        void Save();
        void Close();
    }
}
