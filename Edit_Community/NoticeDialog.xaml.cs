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
using User.HTStudioService;
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

        public bool ShowButton2
        {
            get { return (bool)GetValue(ShowButton2Property); }
            set { SetValue(ShowButton2Property, value); }
        }
        public static readonly DependencyProperty ShowButton2Property =
            DependencyProperty.Register("ShowButton2", typeof(bool), typeof(NoticeDialog), new PropertyMetadata(false,new PropertyChangedCallback(ShowButton2_Changed)));
        void OnShowButton2Changed()
        {
            if (ShowButton2)
            {
                TriggerButton2.Visibility = Visibility.Visible;
            }
            else
            {
                TriggerButton2.Visibility = Visibility.Collapsed;
            }
        }
        private static void ShowButton2_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NoticeDialog)d).OnShowButton2Changed();
        }

        public event EventHandler<NotificationInfo> Closed;
        public event EventHandler<NotificationInfo> ChooseToNotification;
        public event EventHandler<NotificationInfo> Choose;

        private void TriggerImage_Tapped(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, NotificationInfo);
        }
        private void TriggerButton1_Tapped(object sender, RoutedEventArgs e)
        {
            Choose?.Invoke(this, NotificationInfo);
        }
        private void TriggerButton2_Tapped(object sender, RoutedEventArgs e)
        {
            ChooseToNotification?.Invoke(this, NotificationInfo);
        }
    }
}
