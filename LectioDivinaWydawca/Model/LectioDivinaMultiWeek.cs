using System;
using System.Collections.Generic;

namespace LectioDivina.Wydawca.Model
{
    public class LectioDivinaMultiWeek
    {
        public LectioDivinaMultiWeek()
        {
            Weeks = new List<IdWeek>();
        }
        public List<IdWeek> Weeks { get; set; }
    }
}
