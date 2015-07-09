using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LectioDivina.Service
{
    public class HtmlText
    {
        public string ReplaceAllText(string htmlText, string search, string replace)
        {
            while (htmlText.IndexOf(search) >= 0)
            {
                htmlText = htmlText.Replace(search, replace);
            }
            return htmlText;
        }

        public string RemoveTagWithText(string htmlText, string text)
        {

            int keyPos = htmlText.IndexOf(text);
            if (keyPos <= 0)
                return htmlText;

            // find the postition of the opening of the tag i.e. <
            int tagOpeningPos = keyPos;
            while ((tagOpeningPos > 0) && (!htmlText[tagOpeningPos].Equals('<')))
                tagOpeningPos--;
            if (tagOpeningPos <= 0)
                return htmlText;

            // find the position of the closing tag i.e. >
            int tagClosingPos = keyPos;
            while ((tagClosingPos < htmlText.Length) && (!htmlText[tagClosingPos].Equals('>')))
                tagClosingPos++;

            if (tagClosingPos >= htmlText.Length)
                return htmlText;

            // at this point we have position of 1st and last character to remove

            return htmlText.Substring(0,tagOpeningPos)+htmlText.Substring(tagClosingPos+1);
        }

        //private string GetFirstHtmlTag(string htmlText)
        //{
        //    const string openingTagRegex = "<(\"[^\"]*\"|'[^']*'|[^'\">])*>";
        //    var match = Regex.Match(htmlText, openingTagRegex);
        //    var s = match.Result("");
            
        //}

    }
}
