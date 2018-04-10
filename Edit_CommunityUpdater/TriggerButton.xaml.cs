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

namespace Edit_CommunityUpdater
{
    /// <summary>
    /// TriggerButton.xaml 的交互逻辑
    /// </summary>
    public partial class TriggerButton :CheckControl
    {
        public TriggerButton()
        {
            InitializeComponent();
        }

        public object InnerContent
        {
            get { return (object)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        protected override void OnControlStyleChanged()
        {
            if (ControlStyle == ControlStyle.Transparent)
            {
                if (!IsChecked)
                    GridMain.Background = Brushes.Transparent;
                Bdr1.BorderBrush = ControlBase.UWhiteBrush;
                Bdr.BorderBrush = ControlBase.UWhiteBrush;
            }
            else if (ControlStyle == ControlStyle.Light)
            {
                if (!IsChecked)
                    GridMain.Background = ControlBase.LightGrayBrush;
                Bdr1.BorderBrush = ControlBase.UBlackBrush;
                Bdr.BorderBrush = ControlBase.UBlackBrush;
            }
            else
            {
                if (!IsChecked)
                    GridMain.Background = ControlBase.DeepGrayBrush;
                Bdr1.BorderBrush = ControlBase.UWhiteBrush;
                Bdr.BorderBrush = ControlBase.UWhiteBrush;
            }
        }
        protected override void OnChecked()
        {
            if (IsChecked)
            {
                GridMain.Background = new SolidColorBrush(ThemeColor);
            }
            else
            {
                OnControlStyleChanged();
            }
        }
        protected override void OnThemeColor()
        {
            OnChecked();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IsHighLight = false;
            if (e.ChangedButton == MouseButton.Left)
            {
                Bdr1.Visibility = Visibility.Visible;
                Scale.ScaleX = 0.9;
                Scale.ScaleY = 0.9;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Bdr.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.Bdr.Visibility = Visibility.Hidden;
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }
        void OnInnerContentChanged()
        {
            LblContent.Content = InnerContent;
        }

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(TriggerButton), new PropertyMetadata("显示文本", new PropertyChangedCallback(InnerContent_Changed)));

        private static void InnerContent_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TriggerButton)d).OnInnerContentChanged();
        }
    }
}
