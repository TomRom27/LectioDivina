using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LectioDivina.Model;

namespace LectioDivina.Wydawca.Model
{
    public class IdWeek
    {
        public IdWeek()
        {
            Id = DateTime.Now.Ticks;
        }
        public long Id { get; set; }
        public LectioDivinaWeek Week { get; set; }
    }
}
