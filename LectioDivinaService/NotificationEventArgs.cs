using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectioDivina.Service
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(string notification)
        {
            Notification = notification;
        }

        public string Notification { get; private set; }
    }
}
