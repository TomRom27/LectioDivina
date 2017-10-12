using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMaker.Word.Common
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


    public class WordCommon
    {
        public const string Word2013Version = "14.0";

        public const int wdActiveEndAdjustedPageNumber = 1;

        public const int wdReplaceOne = 1;
        public const int wdReplaceAll = 2;

        public const int wdPrintView = 3;
        public const int wdReadingView = 7;

        public const int wdFormatDocument97 = 0;
        public const int wdFormatDocumentDefault = 16;
        public const int wdFormatPDF = 17;

        public const int wdUnderlineSingle = 1;
    }
}
