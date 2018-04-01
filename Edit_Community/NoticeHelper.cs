using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.SoftWare.Service;

namespace Edit_Community
{
    public class NoticeHelper
    {
        public NoticeHelper()
        {
            Notification.Load();
            Notification.ListAdded += Notification_ListAdded;
        }
        private void Notification_ListAdded(object sender, NotificationInfo e)
        {
            Area.MainWindow.InvokeNotice(e);
        }
        public Notification Notification { get; } = new Notification(StartUp.LocalFolder,"Notification");
        public void Add(NotificationInfo info)
        {
            Notification.Add(info);
        }
       
    }
}
