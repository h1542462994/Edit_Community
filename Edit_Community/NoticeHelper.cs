using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.HTStudioService;
using User.SoftWare.Service;

namespace Edit_Community
{
    public class NoticeHelper
    {
        public NoticeHelper()
        {
            Notification.Load();
        }
        private void Notification_Changed(object sender, object e)
        {
            Area.MainWindow.LoadNotice();
        }
        public Notification Notification { get; } = new Notification(StartUp.LocalFolder, "Notification");
        public void Add(NotificationInfo info)
        {
            Notification.Add(info);
            Notification_Changed(null, null);
            Area.MainWindow.InvokeNotice(info);
        }
        public void Remove(NotificationInfo info)
        {
            Notification.Remove(info);
            Notification_Changed(null, null);
        }
        public async void DownloadNoticeAsync()
        {
            await Task.Run(() =>
            {
                if (Notification.DownloadNew("Edit_Community", Area.Local.NoticeLastTime, out int count))
                {
                    Area.MainWindow.Dispatcher.Invoke(() =>
                    {
                        if (count >= 2)
                        {
                            Area.MainWindow.OnDownloadNotice(count);
                        }
                        else if (count == 1)
                        {
                            Area.MainWindow.InvokeNotice(Notification.Last());
                        }
                        if (count!=0)
                        {
                            Notification_Changed(null, null);
                        }
                        Area.MainWindow.LblNoticeError.Visibility = System.Windows.Visibility.Collapsed;
                    });
                    Area.Local.NoticeLastTime = DateTime.Now;
                }
                else
                {
                    Area.MainWindow.Dispatcher.Invoke(() =>
                    {
                        Area.MainWindow.LblNoticeError.Visibility = System.Windows.Visibility.Visible;
                    });
                }
            });
        }
        public async Task<bool> SendNoticeAsync(NotificationInfo info)
        {
            return  await Task.Run(()=>Notification.Apply("Edit_Community",info));
        }
        public void Clear()
        {
            Notification.Clear();
            Notification_Changed(null, null);
        }
    }
}
