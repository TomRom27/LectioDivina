using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectioDivina.Model
{
    public class TitlePage 
    {

        public DateTime SundayDate { get; set; }

        public string WeekInvocation { get; set; }
        public string WeekDescription { get; set; }
        public string WeekPictureName { get; set; }
        public string WeekShortContemplationName { get; set; }
        public bool IsPictureFromShortContemplation { get; set; }
        public string LectioTemplateFile { get; set; }
        public string LectioTargetFolder { get; set; }
        public string LectioTargetFile { get; set; }
        public string LectioEbookSourceFolder { get; set; }
        public string LectioEbookTargetFile { get; set; }
    }
}
