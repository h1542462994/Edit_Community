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
            AppData.MainWindow.LoadNotice();
        }
        public Notification Notification { get; } = new Notification(AppData.LocalFolder, "Notification");
        public void Add(NotificationInfo info)
        {
            Notification.Add(info);
            Notification_Changed(null, null);
            AppData.MainWindow.InvokeNotice(info);
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
                if (Notification.DownloadNew("Edit_Community", AppData.Local.NoticeLastTime, out int count))
                {
                    AppData.MainWindow.Dispatcher.Invoke(() =>
                    {
                        if (count >= 2)
                        {
                            AppData.MainWindow.OnDownloadNotice(count);
                        }
                        else if (count == 1)
                        {
                            AppData.MainWindow.InvokeNotice(Notification.Last());
                        }
                        if (count!=0)
                        {
                            Notification_Changed(null, null);
                        }
                        AppData.MainWindow.LblNoticeError.Visibility = System.Windows.Visibility.Collapsed;
                    });
                    AppData.Local.NoticeLastTime = DateTime.Now;
                }
                else
                {
                    AppData.MainWindow.Dispatcher.Invoke(() =>
                    {
                        AppData.MainWindow.LblNoticeError.Visibility = System.Windows.Visibility.Visible;
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
