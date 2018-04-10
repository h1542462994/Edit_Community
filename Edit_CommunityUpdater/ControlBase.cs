using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Edit_CommunityUpdater
{
    public enum ControlStyle
    {
        Transparent,
        Light,
        Dark
    }
    public enum SlideStyle
    {
        Default,
        Brush,
        TickValue
    }

    public class ControlBase
    {
        /// <summary>
        /// 颜色为66FFFFFF的纯色笔刷.
        /// </summary>
        public static Brush UWhiteBrush => new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
        public static Brush UBlackBrush => new SolidColorBrush(Color.FromArgb(102, 0, 0, 0));
        /// <summary>
        /// 颜色为#CCFFFFFF的纯色笔刷.
        /// </summary>
        public static Brush DWhiteBrush => new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
        /// <summary>
        /// 颜色为#CC000000的纯色笔刷.
        /// </summary>
        public static Brush DBlackBrush => new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
        /// <summary>
        /// 颜色为#CC333333的纯色笔刷.
        /// </summary>
        public static Brush DeepGrayBrush => new SolidColorBrush(Color.FromArgb(204, 51, 51, 51));
        /// <summary>
        /// 颜色为#CCCCCCCC的笔刷.
        /// </summary>
        public static Brush LightGrayBrush => new SolidColorBrush(Color.FromArgb(204, 204, 204, 204));
        public static Color ThemeColorDefault => Color.FromRgb(0x1b, 0x97, 0x73);
        public static Brush GetLinearGradiantBrush(Color color1, Color color2, double percent)
        {
            if (percent <= 0)
            {
                return new SolidColorBrush(color2);
            }
            else if (percent >= 1)
            {
                return new SolidColorBrush(color1);
            }
            else
            {
                GradientStop[] stops = new GradientStop[]
                {
                    new GradientStop(color1,0),
                    new GradientStop(color1,percent),
                    new GradientStop(color2,percent),
                    new GradientStop(color2,1)
                };
                return new LinearGradientBrush(new GradientStopCollection(stops), new Point(0, 0.5), new Point(1, 0.5));
            }
        }
    }
    /// <summary>
    /// 为用户控件提供统一的内容模型.
    /// </summary>
    public class UControl : UserControl
    {
        public static readonly DependencyProperty IsOpenedProperty =
           DependencyProperty.Register("IsOpened", typeof(bool), typeof(UControl), new PropertyMetadata(true, new PropertyChangedCallback(IsOpened_Changed)));
        public static readonly DependencyProperty ControlStyleProperty =
           DependencyProperty.Register("ControlStyle", typeof(ControlStyle), typeof(UControl), new PropertyMetadata(ControlStyle.Transparent, new PropertyChangedCallback(ControlStyle_Changed)));
        public static readonly DependencyProperty IsHighLightProperty =
           DependencyProperty.Register("IsHighLight", typeof(bool), typeof(UControl), new PropertyMetadata(false, new PropertyChangedCallback(IsHighLight_Changed)));
        public static readonly DependencyProperty ThemeColorProperty =
           DependencyProperty.Register("ThemeColor", typeof(Color), typeof(UControl), new PropertyMetadata(ControlBase.ThemeColorDefault, new PropertyChangedCallback(ThemeColor_Changed)));
        bool isLeftMouseDown;

        public UControl()
        {
            this.MouseLeftButtonDown += UControl_MouseLeftButtonDown;
            this.MouseLeave += UControl_MouseLeave;
            this.MouseLeftButtonUp += UControl_MouseLeftButtonUp;
        }

        public bool IsOpened
        {
            get { return (bool)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }
        public ControlStyle ControlStyle
        {
            get { return (ControlStyle)GetValue(ControlStyleProperty); }
            set { SetValue(ControlStyleProperty, value); }
        }
        public bool IsHighLight
        {
            get { return (bool)GetValue(IsHighLightProperty); }
            set { SetValue(IsHighLightProperty, value); }
        }
        public Color ThemeColor
        {
            get { return (Color)GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }
        public event RoutedEventHandler Tapped;
        protected virtual void OnIsOpenChanged()
        {

        }
        protected virtual void OnControlStyleChanged() { }
        protected virtual void OnHighLightChanged() { }
        protected virtual void OnThemeColor() { }
        private void UControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLeftMouseDown = true;
        }
        private void UControl_MouseLeave(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = false;
        }
        private void UControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsOpened && isLeftMouseDown)
            {
                Tapped?.Invoke(this, new RoutedEventArgs());
            }
        }
        private static void IsOpened_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UControl)d).OnIsOpenChanged();
        }
        private static void ControlStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnControlStyleChanged();
        }
        private static void IsHighLight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnHighLightChanged();
        }
        private static void ThemeColor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnThemeColor();
        }
    }
    /// <summary>
    /// 为具有Check属性的控件提供基类.
    /// </summary>
    public class CheckControl : UControl
    {

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public bool CanAutoCheck
        {
            get { return (bool)GetValue(CanAutoCheckProperty); }
            set { SetValue(CanAutoCheckProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckControl), new PropertyMetadata(false, new PropertyChangedCallback(IsChecked_Changed)));
        public static readonly DependencyProperty CanAutoCheckProperty =
            DependencyProperty.Register("CanAutoCheck", typeof(bool), typeof(CheckControl), new PropertyMetadata(true));

        protected virtual void OnChecked() { }

        private static void IsChecked_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckControl arg = (CheckControl)d;
            arg.OnChecked();
        }
    }
}
