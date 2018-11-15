using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight.Messaging;


namespace LectioDivina.Wydawca.Notification
{
    public class RemoveWeekMessage

    {
        public RemoveWeekMessage(long id)
        {
            WeekId = id;
        }

        public long WeekId { get; private set; }
    }
}
