using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using User;
using User.SoftWare.Service;

namespace Edit_Community
{
    /// <summary>
    /// NoticeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NoticeDialog : UserControl
    {
        public NoticeDialog()
        {
            InitializeComponent();
            
        }
        public NotificationInfo NotificationInfo
        {
            get { return (NotificationInfo)GetValue(NotificationInfoProperty); }
            set { SetValue(NotificationInfoProperty, value); }
        }
        public static readonly DependencyProperty NotificationInfoProperty =
            DependencyProperty.Register("NotificationInfo", typeof(NotificationInfo), typeof(NoticeDialog), new PropertyMetadata(new NotificationInfo(), new PropertyChangedCallback(NotificationInfo_Changed)));
        void OnNotificationInfoChanged()
        {
            LblTitle.Content = NotificationInfo.Title;
            LblTime.Content = NotificationInfo.DateTime.ToFriendString();
            TbkDescription.Text = NotificationInfo.Description;
            TriggerButton1.InnerContent = NotificationInfo.Button;
            if (NotificationInfo.Button == "")
            {
                TriggerButton1.Visibility = Visibility.Collapsed;
            }
        }
        private static void NotificationInfo_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NoticeDialog)d).OnNotificationInfoChanged();
        }

        public event RoutedEventHandler Closed;
        public event EventHandler ChooseToNotification;
        public event EventHandler<NotificationInfo> Choose;

        private void TriggerImage_Tapped(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, e);
        }
        private void TriggerButton1_Tapped(object sender, RoutedEventArgs e)
        {
            Choose?.Invoke(this, NotificationInfo);
        }
        private void TriggerButton2_Tapped(object sender, RoutedEventArgs e)
        {
            ChooseToNotification?.Invoke(this, new EventArgs());
        }
    }
}
