﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using User;
using User.HTStudioService;
using User.SoftWare;
using User.SoftWare.Service;
using User.UI;

namespace Edit_Community
{ 
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 公用参数
        bool isWindowLoaded = false;
        bool isEditLoaded = false;
        int isEditChanged = 0;

        internal RichTextBox[] RTbx = new RichTextBox[6];
        internal Label[] LblF = new Label[2];
        internal Ellipse[] LblC = new Ellipse[9];
        internal Ellipse[] LblH = new Ellipse[4];
        const double elpWidth = 8;
        const double elpdefimin = 0.05;
        const int fontsizemin = 10;
        const int fontsizemax = 76;
        bool ismouseleftdown = false;
        object LblEditMouseDownobj;
        int rtxFocusIndex;
        int lastRtxFocusIndex = -1;
        bool needReturnFocus = false;
        bool isGridbackMousedown = false;
        bool isgridmenumoreshow;
        public static Size ScreenSize => new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        public bool IsWindowLoaded { get => isWindowLoaded; set => isWindowLoaded = value; }
        public bool IsEditLoaded { get => isEditLoaded; set => isEditLoaded = value; }
        public int IsEditChanged { get => isEditChanged; set => isEditChanged = value; }
        public bool Ismouseleftdown { get => ismouseleftdown; set => ismouseleftdown = value; }
        public int LastRtxFocusIndex { get => lastRtxFocusIndex; set => lastRtxFocusIndex = value; }
        public int RtxFocusIndex { get => rtxFocusIndex; set => rtxFocusIndex = value; }
        public bool NeedReturnFocus { get => needReturnFocus; set => needReturnFocus = value; }
        public bool IsGridbackMousedown { get => isGridbackMousedown; set => isGridbackMousedown = value; }
        public bool _AllowsTransprency { get; set; }
        #endregion
        #region 计时器模块
        private void RegisterTimer()
        {
            AppData.TimerInventory.Register(TimerDisplayName.ExitEdit, new TimerQueueInfo(AppData.Local.ExitEditInterval, new EventHandler(Timer_ExitEdit), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.HideImg, new TimerQueueInfo(6, new EventHandler(Timer_HideImg), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.HideMouse, new TimerQueueInfo(6, new EventHandler(Timer_HideMouse), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.BackgroundPic, new TimerQueueInfo(10, new EventHandler(Timer_BackgroundPic), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.Weather, new TimerQueueInfo(10, new EventHandler(Timer_Weather), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.Update, new TimerQueueInfo(10, new EventHandler(Timer_Update), false, true));
            AppData.TimerInventory.Register(TimerDisplayName.Notification, new TimerQueueInfo(10, new EventHandler(Timer_Notification), false, true));
        }
        private void Timer_Notification(object sender, EventArgs e)
        {
            AppData.NoticeHelper.DownloadNoticeAsync();
        }
        private void Timer_Update(object sender, EventArgs e)
        {
            if (AppData.Local.IsAutoUpdate)
            {
                OnUpdateAsync(false);
            }
        }
        private void Timer_Weather(object sender, EventArgs e)
        {
            if (AppData.Local.WeatherisOpen)
            {
                OnWeatherAsync();
            }
        }
        private void Timer_BackgroundPic(object sender, EventArgs e)
        {
            int mode = AppData.Local.BackgroundMode;
            OnBackgrondPic(mode);
            Console.WriteLine("Timer=> BackgroundPic");
        }
        private void Timer_HideMouse(object sender, EventArgs e)
        {
            HideMouse();
        }
        private void Timer_HideImg(object sender, EventArgs e)
        {
            HideImg();
        }
        private void Timer_ExitEdit(object sender, EventArgs e)
        {
            Console.WriteLine("=> Timer_ExitEdit" + AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime);
            ExitEdit();
        }
        #endregion
        #region 全局
        public MainWindow()
        {
            InitializeComponent();
            //透明特性
            _AllowsTransprency = AppData.Local.AllowTranspancy;
            AllowsTransparency = _AllowsTransprency;
            if (_AllowsTransprency)
            {
                WindowStyle = WindowStyle.None;
            }

            AppData.MainWindow = this;
            this.Title = "Edit Community " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            for (int i = 0; i < RTbx.Length; i++)
            {
                string displayname = "Rtx" + i;
                RTbx[i] = (RichTextBox)this.FindName(displayname);
            }
            for (int i = 0; i < LblF.Length; i++)
            {
                string displayname = "LblEditF" + i;
                LblF[i] = (Label)FindName(displayname);
                LblF[i].MouseDown += LblEdit_MouseDown;
                LblF[i].MouseLeave += LblEdit_MouseLeave;
                LblF[i].MouseUp += LblEditF_MouseUp;

            }
            for (int i = 0; i < LblC.Length; i++)
            {
                string displayname = "LblEditC" + i;
                LblC[i] = (Ellipse)FindName(displayname);
                LblC[i].MouseDown += LblEdit_MouseDown;
                LblC[i].MouseLeave += LblEdit_MouseLeave;
                LblC[i].MouseUp += LblEditC_MouseUp;
            }
            for (int i = 0; i < LblH.Length; i++)
            {
                string displayname = "LblEditH" + i;
                LblH[i] = (Ellipse)FindName(displayname);
                LblH[i].MouseDown += LblEdit_MouseDown;
                LblH[i].MouseLeave += LblEdit_MouseLeave;
                LblH[i].MouseUp += LblEditC_MouseUp;
            }

            UComboBox1.SelectionStringChanged += UComboBox1_SelectionStringChanged;
            TbxTemp.GotFocus += TbxTemp_GotFocus;
            TbxTemp.LostFocus += TbxTemp_LostFocus;
            for (int i = 0; i < RTbx.Length; i++)
            {
                RTbx[i].SelectionChanged += Rtx_SelectionChanged;
                RTbx[i].TextChanged += Rtx_TextChanged;
                RTbx[i].GotFocus += Rtx_GotFocus;
                InputMethod.SetPreferredImeState(RTbx[i], InputMethodState.DoNotCare);
                InputLanguageManager.SetInputLanguage(RTbx[i], CultureInfo.GetCultureInfo("zh-CN"));
            }
            FrameEditView.Content = new EditViewPage();
            gridBinding = new Dictionary<TriggerImage, Grid>()
            {
                {ImgEditSettings,GridSettings},
                //{ImgNotice,GridNotice},
                //{ImgView,GridEditView }
            };
            foreach (var item in gridBinding.Keys)
            {
                item.Tapped += ImgSelectedItem_Tapped;
            }
            SoftWareService.ChannelFreshed += SoftWareService_ChannelFreshed;
            SoftWareService.CheckUpdateCompleted += SoftWareService_CheckUpdateCompleted;
            this.EditICs.SaveBrushCallBack += Edit.SaveBrush;
            this.EditICs.SetPropertys(AppData.Local.InkColorIndexProperty, AppData.Local.InkPenWidthProperty);
            this.EditICs.LoadPropertys();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNotice();
            AppData.PageNavigationHelper.PageChanged += PageNavigationHelper_PageChanged;
            AppData.PageNavigationHelper.Add(typeof(SettingsMainPage));
            AppData.Local.Flush();
            Edit.Load(DateTime.Now);
            Edit.GetInfos();
            Edit.SetInfos();
            //((EditViewPage)FrameEditView.Content).Load();
            //EditViewPage.Select();
            WeatherText.Target = Rtx4;
            AutoCheckText.Target = Rtx4;
            AutoCheckText.AutoCheckCollection = AppData.Local.CheckData;
            RegisterTimer();
            OnBackgrondPic(AppData.Local.BackgroundMode, true, false);
            if (AppData.Local.CheckisOpen)
            {
                OnAutoCheck();
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (AppData.Local.IsFullScreen == false)
            {
                AppData.Local.AppLocation = new Point(Left / ScreenSize.Width, Top / ScreenSize.Height);
                AppData.Local.AppSize = new Size(Width / ScreenSize.Width, Height / ScreenSize.Height);
                AppData.Local.IsMaxShow = WindowState == WindowState.Maximized;
            }
        }
        public void Local_PropertyChanged(USettingsProperty key, UPropertyChangedEventArgs e)
        {
            if (key == AppData.Local.EditColorProperty)
            {
                Console.WriteLine("isEditColor => ???");
                Color[] arg = (Color[])e.NewValue;
                int num = arg.Length > LblC.Length ? LblC.Length : arg.Length;
                for (int i = 0; i < LblC.Length; i++)
                {
                    if (i < num)
                    {
                        LblC[i].Fill = new SolidColorBrush(arg[i]);
                        LblC[i].ToolTip = string.Format("{0},{1},{2}", arg[i].R, arg[i].G, arg[i].B);
                    }
                    else
                    {
                        LblC[i].Fill = Brushes.Transparent;
                        LblC[i].ToolTip = null;
                    }
                }
            }
            else if (key == AppData.Local.EditColorHistoryProperty)
            {
                Console.WriteLine("iseditcolorHistory => ???");
                Color[] arg = (Color[])e.NewValue;
                for (int i = 0; i < LblH.Length; i++)
                {
                    if (i < arg.Length)
                    {
                        LblH[i].Fill = new SolidColorBrush(arg[i]);
                        LblH[i].ToolTip = string.Format("{0},{1},{2}", arg[i].R, arg[i].G, arg[i].B);
                    }
                    else
                    {
                        LblH[i].Fill = Brushes.Transparent;
                        LblH[i].ToolTip = null;
                    }
                }
            }
            else if (key == AppData.Local.IsFullScreenProperty)
            {
                FullScreenChanged((bool)e.NewValue);
            }
            else if (key == AppData.Local.EditBackgroundColorProperty)
            {
                Color arg = (Color)e.NewValue;
                //BdrEditBackground.Background = new SolidColorBrush(arg);
                //BdrEditBackground.ToolTip = string.Format("{0},{1},{2},{3}", arg.A, arg.R, arg.G, arg.B);
                BdrRtxBack.Background = new SolidColorBrush((Color)e.NewValue);
                for (int i = 0; i < RTbx.Length; i++)
                {
                    RTbx[i].Background = new SolidColorBrush((Color)e.NewValue);
                }
            }
            else if (key == AppData.Local.IsEditBrushOpenProperty)
            {
                if (AppData.Local.IsFullScreen || _AllowsTransprency)//why is || ?
                {
                    this.ImgEditBrush.IsChecked = (bool)e.NewValue;
                    if ((bool)e.NewValue)
                    {
                        this.EditICs.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.EditICs.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    this.EditICs.Visibility = Visibility.Hidden;
                }
                this.QBBrush.IsChecked = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    this.QBBrush.Description = "显示";
                }
                else
                {
                    this.QBBrush.Description = "隐藏";
                }
            }
            else if (key == AppData.Local.IsRtxHiddenProperty)
            {
                if ((bool)e.NewValue)
                {
                    this.BdrRtxBack.Visibility = Visibility.Visible;
                    this.GridEditRtx.Visibility = Visibility.Hidden;
                    QBHideText.Description = "隐藏";
                    QBHideText.IsChecked = true;
                }
                else
                {
                    this.BdrRtxBack.Visibility = Visibility.Hidden;
                    this.GridEditRtx.Visibility = Visibility.Visible;
                    QBHideText.Description = "显示";
                    QBHideText.IsChecked = false;
                }
            }
            else if (key == AppData.Local.BackgroundModeProperty)
            {
                int mode = (int)e.NewValue;
                if (mode == 0)
                {
                    QBBackgroundMode.IsChecked = false;
                    QBBackgroundMode.Description = "无";
                    QBBackgroundNext.Visibility = Visibility.Collapsed;
                }
                else if (mode == 1)
                {
                    QBBackgroundMode.IsChecked = true;
                    QBBackgroundMode.Description = "图片";
                    QBBackgroundNext.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QBBackgroundMode.IsChecked = true;
                    QBBackgroundMode.Description = "幻灯片";
                    QBBackgroundNext.Visibility = Visibility.Visible;
                }
            }
            else if (key == AppData.Local.WeathercityProperty)
            {
                WeatherText.City = (string)e.NewValue;
            }
            else if (key == AppData.Local.WeatherisOpenProperty)
            {
                if ((bool)e.NewValue)
                {
                    if (AppData.Edit == null || AppData.Edit.CreateTime.Date == DateTime.Today)
                    {
                        QBWeather.Description = "开";
                        QBWeather.ThemeBrush = UserBrushes.ThemeBrushDefault;
                    }
                    else
                    {
                        QBWeather.Description = ">今天";
                        QBWeather.ThemeBrush = new SolidColorBrush( Color.FromRgb(235, 149, 20));
                    }
                }
                else
                {
                    QBWeather.Description = "关";
                }
                QBWeather.IsChecked = (bool)e.NewValue;
            }
            else if (key == AppData.Local.CheckisOpenProperty)
            {
                if ((bool)e.NewValue)
                {
                    if (AppData.Edit == null || AppData.Edit.CreateTime.Date == DateTime.Today)
                    {
                        QBAutoCheck.Description = "开";
                        QBAutoCheck.ThemeBrush = UserBrushes.ThemeBrushDefault;
                    }
                    else
                    {
                        QBAutoCheck.Description = ">今天";
                        QBAutoCheck.ThemeBrush = UserBrushes.WarningBrush;
                    }
                }
                else
                {
                    QBAutoCheck.Description = "关";
                }
                QBAutoCheck.IsChecked = (bool)e.NewValue;
            }
            else if (key == AppData.Local.IsAutoUpdateProperty)
            {
                QBAutoUpdate.IsChecked = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    if (!UpdateDownloaded)
                    {
                        QBAutoUpdate.Description = "开";
                        QBAutoUpdate.ThemeBrush = UserBrushes.ThemeBrushDefault;
                    }
                    else
                    {
                        QBAutoUpdate.Description = "需重启";
                        QBAutoUpdate.ThemeBrush = UserBrushes.WarningBrush;
                    }
                }
                else
                {
                    QBAutoUpdate.Description = "关";
                }
            }
        }
        public void Edit_PropertyChanged(USettingsProperty key, UPropertyChangedEventArgs e)
        {
            if (key == AppData.Edit.ColumnDefiProperty)
            {
                double[] arg = (double[])e.NewValue;
                ColumnDefi0.Width = new GridLength(arg[0], GridUnitType.Star);
                ColumnDefi1.Width = new GridLength(arg[1], GridUnitType.Star);
                ColumnDefi2.Width = new GridLength(1 - arg[0] - arg[1], GridUnitType.Star);
                SetElpLocationAll();
            }
            else if (key == AppData.Edit.RowDefi0Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU0.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD0.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == AppData.Edit.RowDefi1Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU1.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD1.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == AppData.Edit.RowDefi2Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU2.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD2.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == AppData.Edit.ColumnElp0Property)
            {
                SetElpLocation(0, (double)e.NewValue);
            }
            else if (key == AppData.Edit.ColumnElp1Property)
            {
                SetElpLocation(1, (double)e.NewValue);
            }
            else if (key == AppData.Edit.RowElo0Property)
            {
                SetElpLocation(2, (double)e.NewValue);
            }
            else if (key == AppData.Edit.RowElp1Property)
            {
                SetElpLocation(3, (double)e.NewValue);
            }
            else if (key == AppData.Edit.RowElp2Property)
            {
                SetElpLocation(4, (double)e.NewValue);
            }
        }
        #endregion
        #region 全局布局
        bool isMenuShow = false;
        public bool IsMenuShow
        {
            get => isMenuShow;
            set
            {
                if (isMenuShow != value)
                {
                    ImgMenu.IsChecked = value;
                    if (value == false)
                    {
                        GridMenu.Visibility = Visibility.Hidden;
                        isgridmenumoreshow = GridMenuMore.Visibility == Visibility.Visible;
                        GridMenuMore.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        GridMenu.Visibility = Visibility.Visible;
                        if (isgridmenumoreshow)
                        {
                            GridMenuMore.Visibility = Visibility.Visible;
                        }
                    }
                }
                isMenuShow = value;
            }
        }
        Dictionary<TriggerImage, Grid> gridBinding = new Dictionary<TriggerImage, Grid>();
        /// <summary>
        /// 全屏状态改变.
        /// </summary>
        public void FullScreenChanged(bool isfullscreen)
        {
            if (!_AllowsTransprency)
            {
                ImgFullScreen.IsChecked = isfullscreen;
                if (isfullscreen)
                {
                    if (IsWindowLoaded && WindowState == WindowState.Normal)//记录位置和大小.
                    {
                        AppData.Local.AppSize = new Size(Width / ScreenSize.Width, Height / ScreenSize.Height);
                        AppData.Local.AppLocation = new Point(Left / ScreenSize.Width, Top / ScreenSize.Height);
                    }
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    AppData.Local.IsMaxShow = WindowState == WindowState.Maximized;
                    WindowState = WindowState.Normal;
                    Left = 0;
                    Top = 0;
                    Width = ScreenSize.Width;
                    Height = ScreenSize.Height;
                    ImgEditBrush.Visibility = Visibility.Visible;
                    QBBrush.Visibility = Visibility.Visible;
                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    ResizeMode = ResizeMode.CanResize;
                    if (AppData.Local.IsMaxShow)
                    {
                        WindowState = WindowState.Maximized;
                    }
                    Left = AppData.Local.AppLocation.X * ScreenSize.Width;
                    Top = AppData.Local.AppLocation.Y * ScreenSize.Height;
                    Width = AppData.Local.AppSize.Width * ScreenSize.Width;
                    Height = AppData.Local.AppSize.Height * ScreenSize.Height;
                    ImgEditBrush.Visibility = Visibility.Collapsed;
                    QBBrush.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ImgFullScreen.IsChecked = true;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Left = 0;
                Top = 0;
                Width = ScreenSize.Width;
                Height = ScreenSize.Height;
                ImgEditBrush.Visibility = Visibility.Visible;
                QBBrush.Visibility = Visibility.Visible;
            }


        }
        private void ImgEditMove_Tapped(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(ImgBefore))
            {
                Edit.Move(false);
            }
            else
            {
                Edit.Move(true);
            }
            Edit.SetInfos();
            FreshQBWeather();
            FreshQBAutoCheck();
        }
        private void ImgMenu_Tapped(object sender, RoutedEventArgs e)
        {
            IsMenuShow = !IsMenuShow;
        }
        private void ImgFullScreen_Tapped(object sender, RoutedEventArgs e)
        {
            if (!_AllowsTransprency)
            {
                AppData.Local.IsFullScreen = !AppData.Local.IsFullScreen;
                AppData.Local.IsEditBrushOpen = AppData.Local.IsEditBrushOpen;
            }
            else
            {
                WindowState = WindowState.Minimized;
            }
        }
        private void ImgEditBrush_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.IsEditBrushOpen = !AppData.Local.IsEditBrushOpen;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetElpLocationAll();
            if (IsWindowLoaded)
            {
                AppData.DialogInventory.Move();
            }
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
            {
                ShowRtxEditBox();
                Point point = e.GetPosition(GridMain);
                if (point.X < 0.2 * ActualWidth || point.X > 0.8 * ActualWidth)
                {
                    AppData.TimerInventory[TimerDisplayName.HideImg].TickTime = 1;
                    ShowImg();
                }
                ShowMouse();
            }
            else if (mouseElpOnobjectButton == MouseButton.Left && mouseElpDownOnobject != null)
            {
                Elp_SetValue(e.GetPosition(GridMain));
                Console.WriteLine("=>(Down)Window MouseMove");
            }
        }

        private void ImgSelectedItem_Tapped(object sender, RoutedEventArgs e)
        {
            foreach (var item in gridBinding)
            {
                if (item.Key != sender)
                {
                    item.Key.IsChecked = false;
                    item.Value.Visibility = Visibility.Collapsed;
                }
            }
            User.UI.TriggerImage triggerImage = (User.UI.TriggerImage)sender;
            if (gridBinding.TryGetValue(triggerImage, out Grid grid))
            {
                if (triggerImage.IsChecked == false)
                {
                    grid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid.Visibility = Visibility.Visible;
                }
            }
            SetEditMoreVisibility();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                AppData.Local.IsFullScreen = !AppData.Local.IsFullScreen;
            }
        }
        private void ShowImg()
        {
            if (ImgMenu.IsChecked)
            {
                GridMenu.Visibility = Visibility.Visible;
            }
            ImgFullScreen.Visibility = Visibility.Visible;
            ImgMenu.Visibility = Visibility.Visible;
            SetEditMoreVisibility();
        }
        private void HideImg()
        {
            ImgFullScreen.Visibility = Visibility.Collapsed;
            ImgMenu.Visibility = Visibility.Collapsed;
            GridMenu.Visibility = Visibility.Collapsed;
        }
        private void InEdit()
        {
            for (int i = 0; i < RTbx.Length; i++)
            {
                RTbx[i].BorderThickness = new Thickness(1);
            }
            if (AppData.Edit.EditType != EditItemType.Reserved)
            {
                ElpC1.Visibility = Visibility.Visible;
                ElpC2.Visibility = Visibility.Visible;
                ElpR1.Visibility = Visibility.Visible;
                ElpR2.Visibility = Visibility.Visible;
                ElpR3.Visibility = Visibility.Visible;
            }
        }
        private void ExitEdit()
        {
            if (RtxFocusIndex != -1)
            {
                RtxFocusIndex = -1;
                LastRtxFocusIndex = -1;
                InputLanguageManager.SetInputLanguage(TbxTemp, CultureInfo.GetCultureInfo("zh-cn"));
                TbxTemp.Focus();
            }


            AppData.DialogInventory.Hide(DialogDisplayName.GridEditBox);
            for (int i = 0; i < RTbx.Length; i++)
            {
                RTbx[i].BorderThickness = new Thickness(0);
            }
            ElpC1.Visibility = Visibility.Hidden;
            ElpC2.Visibility = Visibility.Hidden;
            ElpR1.Visibility = Visibility.Hidden;
            ElpR2.Visibility = Visibility.Hidden;
            ElpR3.Visibility = Visibility.Hidden;
        }
        private void HideMouse()
        {
            AppData.TimerInventory[TimerDisplayName.HideMouse].IsStarted = false;
            //
            Mouse.OverrideCursor = Cursors.None;
        }
        private void ShowMouse()
        {
            AppData.TimerInventory[TimerDisplayName.HideMouse].IsStarted = true;
            AppData.TimerInventory[TimerDisplayName.HideMouse].TickTime = 1;
            //
            Mouse.OverrideCursor = null;
        }
        #endregion
        #region 设置(更多)布局
        bool isMenuMouseLeftDown = false;
        private void SetEditMoreVisibility()
        {
            bool isSelected = false;
            foreach (var item in gridBinding.Keys)
            {
                if (item.IsChecked == true)
                {
                    isSelected = true;
                }
            }
            if (isSelected)
            {
                GridMenuMore.Visibility = Visibility.Visible;
                GridMenuMoreLeft.Visibility = Visibility.Visible;
            }
            else
            {
                GridMenuMore.Visibility = Visibility.Collapsed;
                GridMenuMoreLeft.Visibility = Visibility.Collapsed;
            }
        }
        private void ImgSettingsBack_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.PageNavigationHelper.Back();
        }
        private void PageNavigationHelper_PageChanged(object sender, PageNavigationEventargs e)
        {
            if (e.IsFirstPage)
            {
                ColumnDefiSettings.Width = new GridLength(0);
            }
            else
            {
                ColumnDefiSettings.Width = new GridLength(60);
            }
            Page page = (Page)Activator.CreateInstance(e.Page);
            FrameSettings.Content = page;
            LblSettingsTitle.Content = page.Title;
        }
        internal void QBEditMod_Tapped(object sender, RoutedEventArgs e)
        {
            if (QBEditMod.IsChecked)
            {
                QBEditMod.Description = "开";
                Edit.LoadMod();
            }
            else
            {
                QBEditMod.Description = "关";
                Edit.ExitMod();
            }
            FreshQBWeather();
            if (FrameSettings.Content is ExtensionPage page)
            {
                page.SetEditMode();
            }
        }
        internal void QBHideText_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.IsRtxHidden = !AppData.Local.IsRtxHidden;
            if (FrameSettings.Content is ExtensionPage page)
            {
                page.SetText();
            }
        }
        private void QBBackgroundMode_Tapped(object sender, RoutedEventArgs e)
        {
            int mode = AppData.Local.BackgroundMode;
            mode++;
            if (mode > 2) mode = 0;
            AppData.Local.BackgroundMode = mode;
            if (FrameSettings.Content is ThemePage page)
            {
                page.ComboBox1.SelectedIndex = mode;
            }
            else
            {
                OnBackgrondPic(mode, true, false);
            }
        }
        private void QBBrush_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.IsEditBrushOpen = !AppData.Local.IsEditBrushOpen;
        }
        internal void QBWeather_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.WeatherisOpen = !AppData.Local.WeatherisOpen;
            if (AppData.Local.WeatherisOpen)
            {
                OnWeatherAsync(true);
            }
            if (FrameSettings.Content is ExtensionPage page)
            {
                page.SetWeather();
            }
        }
        internal void QBAutoCheck_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.CheckisOpen = !AppData.Local.CheckisOpen;
            if (AppData.Local.CheckisOpen)
            {
                OnAutoCheck();
            }
            if (FrameSettings.Content is ExtensionPage page)
            {
                page.SetAutoCheck();
            }
        }
        private void QBAutoUpdate_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.IsAutoUpdate = !AppData.Local.IsAutoUpdate;
        }
        private void QBBackgroundNext_Tapped(object sender, RoutedEventArgs e)
        {
            OnBackgrondPic(AppData.Local.BackgroundMode, true, true);
        }
        private void GridMenuMoreLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isMenuMouseLeftDown = true;
            }
        }
        private void GridMenuMoreLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            isMenuMouseLeftDown = false;
        }
        private void GridMenuMoreLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isMenuMouseLeftDown)
            {
                foreach (var item in gridBinding.Keys)
                {
                    item.IsChecked = false;
                }
                SetEditMoreVisibility();
            }
        }
        #endregion
        #region 背景布局
        public void OnBackgrondPic(int mode, bool firstload = false, bool isnext = true)
        {
            QBBackgroundMode.ThemeBrush = UserBrushes.ThemeBrushDefault;
            if (mode == 0)
            {
                OnBackgroundPicLoad(null);
            }
            else if (mode == 1)
            {
                try
                {
                    OnBackgroundPicLoad(new BitmapImage(new Uri(AppData.Local.BackgroundPicPath)));
                }
                catch (Exception)
                {
                    QBBackgroundMode.ThemeBrush = new SolidColorBrush(Colors.Orange);
                    Console.WriteLine("BackgroundFailed:1");
                }
            }
            else if (mode == 2)
            {
                bool isOk = true;
                TimeSpan defaultTimeSpan = TimeSpan.FromMinutes(AppData.Local.BackgroundPicTimestamp);
                TimeSpan currentTimeSpan = DateTime.Now - AppData.Local.BackgroundPicLastTime;
                double percent = currentTimeSpan.TotalMinutes / defaultTimeSpan.TotalMinutes;
                QBBackgroundNext.ThemeBrush = UserBrushes.GetLinearGradiantBrush(UserBrushes.ThemeColorDefault, Color.FromArgb(204, 51, 51, 51), percent);
                if (!firstload)
                {
                    if (currentTimeSpan < defaultTimeSpan)
                    {
                        isOk = false;
                    }
                }
                if (isOk)
                {
                    try
                    {
                        DirectoryInfo Folder = new DirectoryInfo(AppData.Local.BackgroundPicFolder);
                        List<FileInfo> infos = new List<FileInfo>();
                        foreach (FileInfo file in Folder.GetFiles())
                        {
                            if (file.Extension.ToLower() == ".png" || file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".jpg")
                            {
                                infos.Add(file);
                            }
                        }
                        if (isnext)
                        {
                            AppData.Local.BackgroundPicCurrentindex++;
                            AppData.Local.BackgroundPicLastTime = DateTime.Now;
                        }
                        if (AppData.Local.BackgroundPicCurrentindex >= infos.Count)
                        {
                            AppData.Local.BackgroundPicCurrentindex = 0;
                        }
                        QBBackgroundNext.Description = string.Format("{0}/{1}", AppData.Local.BackgroundPicCurrentindex + 1, infos.Count);
                        OnBackgroundPicLoad(new BitmapImage(new Uri(infos[AppData.Local.BackgroundPicCurrentindex].FullName)));
                    }
                    catch (Exception)
                    {
                        QBBackgroundMode.ThemeBrush = new SolidColorBrush(Colors.Orange);
                        Console.WriteLine("BackgroundFailed:2");
                    }
                }
            }
            if (FrameSettings.Content is ThemePage page)
            {
                page.CheckPic();
                page.SlideTimeLoad(true);
            }
        }
        void OnBackgroundPicLoad(BitmapImage image)
        {
            if (image != (BitmapImage)ImageBackNew.Source)
            {
                ImageBack.Source = ImageBackNew.Source;
                ImageBackNew.Source = image;
                DoubleAnimation th1 = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(3)),
                    From = 1,
                    To = 0
                };
                DoubleAnimation th2 = new DoubleAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(3)),
                    From = 0,
                    To = 1
                };
                Storyboard storyboard = new Storyboard()
                {
                    Children = { th1, th2 }
                };
                Storyboard.SetTarget(th1, ImageBack);
                Storyboard.SetTargetProperty(th1, new PropertyPath(nameof(ImageBack.Opacity)));
                Storyboard.SetTarget(th2, ImageBackNew);
                Storyboard.SetTargetProperty(th2, new PropertyPath(nameof(ImageBackNew.Opacity)));
                storyboard.Begin();
            }
        }
        #endregion
        #region Rtx布局
        object mouseElpDownOnobject = null;
        MouseButton mouseElpOnobjectButton = MouseButton.Left;
        private void Elp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseElpDownOnobject = sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Elp_SetValue(e.GetPosition(GridMain));
            }
            mouseElpOnobjectButton = e.ChangedButton;
        }
        private void Elp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseElpOnobjectButton == MouseButton.Right && mouseElpDownOnobject != null)
            {
                ExitEdit();
            }
            mouseElpDownOnobject = null;
        }
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Elp_MouseUp(null, null);
        }
        private void Elp_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouseElpOnobjectButton == MouseButton.Right)
            {
                mouseElpDownOnobject = null;
            }
        }
        private void Elp_SetValue(Point mousepoint)
        {
            double[] v = new double[2];
            double[] colmundefi = AppData.Edit.ColumnDefi;
            double num = colmundefi[0] + colmundefi[1];
            if (mouseElpDownOnobject == ElpC1 || mouseElpDownOnobject == ElpC2)
            {
                v[1] = Tools.Checkdouble(mousepoint.Y / this.GridMain.ActualHeight, elpdefimin, 1 - elpdefimin);
                if (mouseElpDownOnobject == ElpC1)
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                         , AppData.Local.ColumnDefiMin
                         , num - AppData.Local.ColumnDefiMin);
                    AppData.Edit.ColumnDefi = new double[] { v[0], num - v[0] };
                    AppData.Edit.ColumnElp0 = v[1];
                    //ElpC1.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
                else
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                        , colmundefi[0] + AppData.Local.ColumnDefiMin
                        , 1 - AppData.Local.ColumnDefiMin);
                    AppData.Edit.ColumnDefi = new double[] { colmundefi[0], v[0] - colmundefi[0] };
                    AppData.Edit.ColumnElp1 = v[1];
                    //ElpC2.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
            }
            else if (mouseElpDownOnobject == ElpR1 || mouseElpDownOnobject == ElpR2 || mouseElpDownOnobject == ElpR3)
            {
                v[1] = Tools.Checkdouble(mousepoint.Y / GridMain.ActualHeight, AppData.Local.RowDefiMin, 1 - AppData.Local.RowDefiMin);
                if (mouseElpDownOnobject == ElpR1)
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / (GridMain.ActualWidth * colmundefi[0]), elpdefimin, 1 - elpdefimin);
                    //ElpR1.Margin =  new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[0], -elpWidth, 0, 0);
                    AppData.Edit.RowDefi0 = v[1];
                    AppData.Edit.RowElp0 = v[0];
                }
                else if (mouseElpDownOnobject == ElpR2)
                {
                    v[0] = Tools.Checkdouble((mousepoint.X - GridMain.ActualWidth * colmundefi[0]) / (GridMain.ActualWidth * colmundefi[1]), elpdefimin, 1 - elpdefimin);
                    //ElpR2.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[1], -elpWidth, 0, 0);
                    AppData.Edit.RowDefi1 = v[1];
                    AppData.Edit.RowElp1 = v[0];
                }
                else
                {
                    v[0] = Tools.Checkdouble((mousepoint.X - GridMain.ActualWidth * num) / (GridMain.ActualWidth * (1 - num)), elpdefimin, 1 - elpdefimin);
                    //ElpR3.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * (1- num), -elpWidth, 0, 0);
                    AppData.Edit.RowDefi2 = v[1];
                    AppData.Edit.RowElp2 = v[0];
                }
            }
        }
        private void SetElpLocationAll()
        {
            if (IsWindowLoaded)
            {
                SetElpLocation(0, AppData.Edit.ColumnElp0);
                SetElpLocation(1, AppData.Edit.ColumnElp1);
                SetElpLocation(2, AppData.Edit.RowElp0);
                SetElpLocation(3, AppData.Edit.RowElp1);
                SetElpLocation(4, AppData.Edit.RowElp2);
            }
        }
        private void Rtx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ShowRtxEditBox();
        }
        private void ShowRtxEditBox()
        {
            if (IsWindowLoaded)
            {
                if (Rtx_GetSectionIndex() != -1 && Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
                {
                    if (!AppData.DialogInventory.Exists(DialogDisplayName.GridEditBox))
                    {
                        Rtx_GetSectionValue();
                        AppData.DialogInventory.Show(DialogDisplayName.GridEditBox, new DialogAutoInfo(GridMain, GridEditBox, DialogAuto.Star, Mouse.GetPosition(GridMain), DialogType.Shadow, 30));
                        AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
                        AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
                    }
                }
                else if (Rtx_GetSectionIndex() == -1 || Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed)
                {
                    AppData.DialogInventory.Hide(DialogDisplayName.GridEditBox);
                }
            }
        }
        private void SetElpLocation(int index, double value)
        {
            if (index == 0)
            {
                ElpC1.Margin = new Thickness(-elpWidth, -elpWidth + value * GridMain.ActualHeight, 0, 0);
            }
            else if (index == 1)
            {
                ElpC2.Margin = new Thickness(-elpWidth, -elpWidth + value * GridMain.ActualHeight, 0, 0);
            }
            else if (index == 2)
            {
                ElpR1.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * AppData.Edit.ColumnDefi[0], -elpWidth, 0, 0);
            }
            else if (index == 3)
            {
                ElpR2.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * AppData.Edit.ColumnDefi[1], -elpWidth, 0, 0);
            }
            else if (index == 4)
            {
                ElpR3.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * (1 - AppData.Edit.ColumnDefi[0] - AppData.Edit.ColumnDefi[1]), -elpWidth, 0, 0);
            }
        }
        #endregion
        #region RichTextBox操作
        private void Rtx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEditLoaded)
            {
                AppData.TimerInventory[TimerDisplayName.ExitEdit].IsStarted = true;
                AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
            }
            for (int i = 0; i < RTbx.Length; i++)
            {
                if (sender.Equals(RTbx[i]))
                {
                    Edit.SaveRtfFile(i);
                    return;
                }
            }
        }
        private void Rtx_GotFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Rtx_GotFocus");
            InEdit();
            AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
            AppData.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
            for (int i = 0; i < RTbx.Length; i++)
            {
                if (sender.Equals(RTbx[i]))
                {
                    RtxFocusIndex = i;
                    break;
                }
            }
            if (RtxFocusIndex != LastRtxFocusIndex)
            {
                InputLanguageManager.SetInputLanguage(TbxTemp, CultureInfo.GetCultureInfo("en-gb"));
                LastRtxFocusIndex = RtxFocusIndex;
                NeedReturnFocus = true;
                TbxTemp.Focus();
            }
        }
        private void TbxTemp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RtxFocusIndex != -1)
            {
                InputLanguageManager.SetInputLanguage(RTbx[RtxFocusIndex], CultureInfo.GetCultureInfo("zh-cn"));
                Console.WriteLine("TbxTemp_LostFocus");
            }
        }
        private void TbxTemp_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NeedReturnFocus)
            {
                Console.WriteLine("TbxTemp_GotFocus");
                NeedReturnFocus = false;
                RTbx[RtxFocusIndex].Focus();
            }
        }
        private int Rtx_GetSectionIndex()
        {
            for (int i = 0; i < RTbx.Length; i++)
            {
                if (!RTbx[i].Selection.IsEmpty && RTbx[i].IsFocused)
                {
                    return i;
                }
            }
            return -1;
        }
        private void Rtx_GetSectionValue()
        {
            int index = Rtx_GetSectionIndex();
            if (index != -1)
            {
                TextSelection selection = RTbx[index].Selection;
                int fontsize = 0;
                string fontfamily = "";
                bool fontbold = false;
                bool fontitalic = false;
                if (selection.GetPropertyValue(TextElement.FontSizeProperty) != DependencyProperty.UnsetValue)
                {
                    try
                    {
                        fontsize = int.Parse(selection.GetPropertyValue(TextElement.FontSizeProperty).ToString());
                    }
                    catch (Exception)
                    {
                        fontsize = 0;
                    }
                }
                if (selection.GetPropertyValue(TextElement.FontFamilyProperty) != DependencyProperty.UnsetValue)
                {
                    fontfamily = selection.GetPropertyValue(TextElement.FontFamilyProperty).ToString();
                }
                if (selection.GetPropertyValue(TextElement.FontWeightProperty) != DependencyProperty.UnsetValue)
                {
                    if ((FontWeight)selection.GetPropertyValue(TextElement.FontWeightProperty) == FontWeights.Bold)
                    {
                        fontbold = true;
                    }
                }
                if (selection.GetPropertyValue(TextElement.FontStyleProperty) != DependencyProperty.UnsetValue)
                {
                    if ((FontStyle)selection.GetPropertyValue(TextElement.FontStyleProperty) == FontStyles.Italic)
                    {
                        fontitalic = true;
                    }
                }
                if (selection.GetPropertyValue(TextElement.ForegroundProperty) != DependencyProperty.UnsetValue)
                {
                    SolidColorBrush arg = (SolidColorBrush)selection.GetPropertyValue(TextElement.ForegroundProperty);
                    ApplyEditColorHistory(arg.Color);
                }
                rtxProperty = new RtxProperty(fontsize, fontfamily, fontbold, fontitalic);
                rtxProperty.Print();
            }
        }
        public void Rtx_SetSelctionValue(string method, object value)
        {
            int index = Rtx_GetSectionIndex();
            if (index != -1)
            {
                if (method == "fontsize")
                {
                    RTbx[index].Selection.ApplyPropertyValue(TextElement.FontSizeProperty, string.Concat((int)value));
                }
                else if (method == "fontbold")
                {
                    if ((bool)value == false)
                    {
                        RTbx[index].Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }
                    else
                    {
                        RTbx[index].Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    }
                }
                else if (method == "fontitalic")
                {
                    if ((bool)value == false)
                    {
                        RTbx[index].Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    }
                    else
                    {
                        RTbx[index].Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    }
                }
                else if (method == "foreground")
                {
                    RTbx[index].Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush((Color)value));
                }
                else if (method == "fontfamily")
                {
                    RTbx[index].Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily((string)value));
                }
            }

            //}
            //catch (Exception)
            //{
            //}
        }
        #endregion
        #region GridEditBox
        private void SlideFontSize_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SlideFontSize_SetValue(SlideFontSize.SlideValue, true);
        }
        private void LblEditF_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(LblEditMouseDownobj) && Ismouseleftdown)
            {
                if (sender.Equals(LblEditF0))
                {
                    rtxProperty.Fontbold = !rtxProperty.Fontbold;
                }
                else if (sender.Equals(LblEditF1))
                {
                    rtxProperty.Fontitalic = !rtxProperty.Fontitalic;
                }
            }
        }
        private void LblEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Ismouseleftdown = true;
            }
            else
            {
                Ismouseleftdown = false;
            }
            LblEditMouseDownobj = sender;
        }
        private void LblEdit_MouseLeave(object sender, MouseEventArgs e)
        {
            LblEditMouseDownobj = null;
        }
        private void LblEditC_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(LblEditMouseDownobj))
            {
                if (Ismouseleftdown)
                {
                    for (int i = 0; i < LblC.Length; i++)
                    {
                        if (sender.Equals(LblC[i]) && i < AppData.Local.Editcolor.Length)
                        {
                            Rtx_SetSelctionValue("foreground", AppData.Local.Editcolor[i]);
                            return;
                        }
                    }
                    for (int i = 0; i < LblH.Length; i++)
                    {
                        if (sender.Equals(LblH[i]) && i < AppData.Local.EditcolorHistory.Length)
                        {
                            Rtx_SetSelctionValue("foreground", AppData.Local.EditcolorHistory[i]);
                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < LblC.Length; i++)
                    {
                        if (sender.Equals(LblC[i]))
                        {
                            if (i < AppData.Local.Editcolor.Length)
                            {
                                ShowColorPicker(e, AppData.Local.Editcolor[i], ColorPickTask.GridEditBox, i);
                            }
                            else
                            {
                                ShowColorPicker(e, Colors.White, ColorPickTask.GridEditBox, i);
                            }
                        }
                        //else
                        //{
                        //    Area.DialogInventory.Show(DialogDisplayName.Dialog,
                        //        new DialogAutoInfo(GridMain, new ColorPicker(new ColorP(Colors.White), new UPropertyChangedEventHander(ColorPick_Ok_Click), new EventHandler(ColorPick_Cancel_Click)),
                        //        DialogAuto.Star, e.GetPosition(GridMain), DialogType.Dialog, 30
                        //        ));
                        //}
                    }
                }
            }
        }
        private void UComboBox1_SelectionStringChanged(object sender, string e)
        {
            rtxProperty.Fontfamily = e;
        }
        class RtxProperty
        {
            int fontSize;
            string fontFamily;
            bool fontBold;
            bool fontItalic;

            public RtxProperty(int fontSize, string fontFamily, bool fontBold, bool fontItalic)
            {
                this.fontSize = fontSize;
                this.fontFamily = fontFamily;
                this.fontBold = fontBold;
                this.fontItalic = fontItalic;
                if (fontSize != 0)
                {
                    AppData.MainWindow.SlideFontSize_SetValue(fontSize);
                }
                else
                {
                    AppData.MainWindow.SlideFontSize_SetValue(-1);
                }
                if (fontBold == false)
                {
                    AppData.MainWindow.LblEditF0.Background = Brushes.Transparent;
                }
                else
                {
                    AppData.MainWindow.LblEditF0.Background = AppData.CheckedBrush;
                }
                if (fontItalic == false)
                {
                    AppData.MainWindow.LblEditF1.Background = Brushes.Transparent;
                }
                else
                {
                    AppData.MainWindow.LblEditF1.Background = AppData.CheckedBrush;
                }
                AppData.MainWindow.UComboBox1.IsDropDownOpen = false;
                if (fontFamily == "" || fontFamily == null)
                {
                    AppData.MainWindow.UComboBox1.Text = "[未识别]";
                }
                else
                {
                    AppData.MainWindow.UComboBox1.Text = fontFamily;
                }
            }

            public int FontSize
            {
                get => fontSize;
                set
                {
                    if (value >= fontsizemin && value <= fontsizemax)
                    {
                        AppData.MainWindow.Rtx_SetSelctionValue("fontsize", value);
                        fontSize = value;
                    }
                }
            }
            public string Fontfamily
            {
                get => fontFamily; set
                {
                    fontFamily = value;
                    AppData.MainWindow.Rtx_SetSelctionValue("fontfamily", value);
                }
            }
            public bool Fontbold
            {
                get => fontBold; set
                {
                    fontBold = value;
                    if (value == false)
                    {
                        AppData.MainWindow.LblEditF0.Background = Brushes.Transparent;
                    }
                    else
                    {
                        AppData.MainWindow.LblEditF0.Background = AppData.CheckedBrush;
                    }
                    AppData.MainWindow.Rtx_SetSelctionValue("fontbold", value);
                }
            }
            public bool Fontitalic
            {
                get => fontItalic; set
                {
                    fontItalic = value;
                    if (value == false)
                    {
                        AppData.MainWindow.LblEditF1.Background = Brushes.Transparent;
                    }
                    else
                    {
                        AppData.MainWindow.LblEditF1.Background = AppData.CheckedBrush;
                    }
                    AppData.MainWindow.Rtx_SetSelctionValue("fontitalic", value);
                }
            }
            public void Print()
            {
                Console.WriteLine(FontSize);
                Console.WriteLine(Fontfamily);
                Console.WriteLine(Fontbold);
                Console.WriteLine(Fontitalic);
            }
        }
        RtxProperty rtxProperty;
        public void SlideFontSize_SetValue(double v, bool isedit = false)
        {
            if (v < 0)
            {
                LblEditFS.Content = "X";
            }
            else
            {
                int fontsize = (int)v;
                LblEditFS.Content = fontsize;
                SlideFontSize.SlideValue = fontsize;
                Rtx_SetSelctionValue("fontsize", fontsize);
            }
        }
        private void ApplyEditColorHistory(Color color)
        {
            for (int i = 0; i < AppData.Local.Editcolor.Length; i++)
            {
                if (AppData.Local.Editcolor[i] == color)
                {
                    return;
                }
            }
            List<Color> colorlist = new List<Color>() { color };
            for (int i = 0; i < AppData.Local.EditcolorHistory.Length; i++)
            {
                if (AppData.Local.EditcolorHistory[i] != color)
                {
                    colorlist.Add(AppData.Local.EditcolorHistory[i]);
                }
            }
            int num = colorlist.Count > 4 ? 4 : colorlist.Count;
            AppData.Local.EditcolorHistory = colorlist.Take(num).ToArray();
        }
        #endregion
        #region ColorDialog交互
        enum ColorPickTask
        {
            None,
            GridEditBox,
            EditBackColor
        }
        ColorPickTask colorPickTask;
        int colorpicktaskindex;
        private void GridDialogBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("GridDialogBack -> MouseUp");
            if (IsGridbackMousedown)
            {
                HideDialog();
            }
        }
        private void GridDialogBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("GridDialogBack -> MouseDown");
            IsGridbackMousedown = true;
        }
        private void ShowColorPicker(MouseButtonEventArgs e, Color color, ColorPickTask colorPickTask, int colorpicktaskindex = 0, DialogAuto dialogAuto = DialogAuto.Star, bool caneditalpha = false)
        {
            ColorPicker1.Value = new ColorP(color);
            AppData.DialogInventory.Show(DialogDisplayName.Dialog,
                   new DialogAutoInfo(GridDialog, ColorPicker1,
                   dialogAuto, e.GetPosition(GridMain), DialogType.Shadow, 30
                   , new Size(200, 230)));
            IsGridbackMousedown = false;
            GridDialogBack.Visibility = Visibility.Visible;
            this.colorPickTask = colorPickTask;
            this.colorpicktaskindex = colorpicktaskindex;
        }
        private void ColorPick_OkOrCancel_Click(object sender, UPropertyChangedEventArgs<ColorP> e)
        {
            if (colorPickTask == ColorPickTask.GridEditBox)
            {
                Color[] arg = AppData.Local.Editcolor;
                if (colorpicktaskindex < arg.Length)
                {
                    arg[colorpicktaskindex] = e.NewValue.GetColor();
                }
                else
                {
                    Color[] argclone = new Color[colorpicktaskindex + 1];
                    for (int i = 0; i < arg.Length; i++)
                    {
                        argclone[i] = arg[i];
                    }
                    argclone[colorpicktaskindex] = e.NewValue.GetColor();
                    arg = argclone;
                }


                AppData.Local.Editcolor = arg;
            }
            HideDialog();
        }
        private void HideDialog()
        {
            AppData.DialogInventory.Hide(DialogDisplayName.Dialog);
            GridDialogBack.Visibility = Visibility.Hidden;
        }
        #endregion
        #region 天气
        public WeatherText WeatherText = new WeatherText("杭州");
        public async void OnWeatherAsync(bool isFirst = false)
        {
            if ((isFirst || DateTime.Now - AppData.Local.WeatherLastTime > TimeSpan.FromMinutes(AppData.Local.WeatherTimestamp)) && AppData.Edit.CreateTime.Date == DateTime.Today)
            {
                try
                {
                    await WeatherText.LoadWeatherAsync();
                    WeatherText.Next();
                   
                }
                catch (Exception)
                {
                    Console.WriteLine("Weather.Error");
                }
                AppData.Local.WeatherLastTime = DateTime.Now;
            }
        }
        void FreshQBWeather()
        {
            //附加.
            if (AppData.Local.WeatherisOpen)
            {
                if (AppData.Edit == null || AppData.Edit.CreateTime.Date == DateTime.Today)
                {
                    QBWeather.Description = "开";
                    QBWeather.ThemeBrush = UserBrushes.ThemeBrushDefault;
                }
                else
                {
                    QBWeather.Description = ">今天";
                    QBWeather.ThemeBrush = UserBrushes.WarningBrush;
                }
            }
        }
        #endregion
        #region AutoCheck
        public AutoCheckText AutoCheckText = new AutoCheckText();
        public void OnAutoCheck()
        {
            try
            {
                AutoCheckText.Next();
            }
            catch (Exception)
            {
                Console.WriteLine("AutoCheck.Error");
            }
        }
        void FreshQBAutoCheck()
        {
            if (AppData.Local.CheckisOpen)
            {
                if (AppData.Edit == null || AppData.Edit.CreateTime.Date == DateTime.Today)
                {
                    QBAutoCheck.Description = "开";
                    QBAutoCheck.ThemeBrush = UserBrushes.ThemeBrushDefault;
                }
                else
                {
                    QBAutoCheck.Description = ">今天";
                    QBAutoCheck.ThemeBrush = UserBrushes.WarningBrush;
                }
            }
        }
        #endregion
        #region 更新
        bool UpdateDownloaded { get; set; }
        string Version { get; set; }
        public SoftWareService SoftWareService { get; } = new SoftWareService(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "Edit_Community");
        double DownloadPercent { get; set; }
        private void SoftWareService_CheckUpdateCompleted(object sender, CheckUpdateEventArgs e)
        {
            if (e.ChannelState == ChannelState.Failed || e.ChannelState == ChannelState.FileFailed)
            {
                Dispatcher.Invoke(() =>
                {
                    QBAutoUpdate.IsOpened = true;
                    QBAutoUpdate.Description = "错误";
                    QBAutoUpdate.ThemeBrush = new SolidColorBrush(Colors.Orange);
                });
            }
            else
            {
                if (e.UpdateType == UpdateType.Download)
                {
                    SoftWareService.DownloadUpdate();
                    Version = e.Version;
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        QBAutoUpdate.IsOpened = true;
                        QBAutoUpdate.Description = "最新";
                        QBAutoUpdate.ThemeBrush = UserBrushes.ThemeBrushDefault;
                    });
                }
            }
        }
        private void SoftWareService_ChannelFreshed(object sender, ChannelFreshEventArgs e)
        {
            if (e.ChannelState == ChannelState.Completed)
            {
                UpdateDownloaded = true;
            }
            else
            {
                UpdateDownloaded = false;
            }
            if (e.ChannelState == ChannelState.Failed || e.ChannelState == ChannelState.FileFailed)
            {
                Dispatcher.Invoke(() =>
                {
                    QBAutoUpdate.IsOpened = true;
                    QBAutoUpdate.Description = "错误";
                    QBAutoUpdate.ThemeBrush = new SolidColorBrush(Colors.Orange);
                });

            }
            else if (e.ChannelState == ChannelState.Doing)
            {
                Dispatcher.Invoke(() =>
                {
                    QBAutoUpdate.IsOpened = false;
                    QBAutoUpdate.Description = string.Format("{0}%", e.Percent);
                    QBAutoUpdate.ThemeBrush = UserBrushes.ThemeBrushDefault;
                });

            }
            else if (e.ChannelState == ChannelState.Completed)
            {
                Dispatcher.Invoke(() =>
                {
                    QBAutoUpdate.IsOpened = true;
                    QBAutoUpdate.Description = "需重启";
                    QBAutoUpdate.ThemeBrush = new SolidColorBrush( Color.FromRgb(235, 149, 20));
                    ShowUpdateDialog();
                });

            }
        }
        public async void OnUpdateAsync(bool isFirst = false)
        {
            if ((isFirst) || DateTime.Now - AppData.Local.UpdateLastTime > TimeSpan.FromMinutes(AppData.Local.UpdateTiemstamp))
            {
                QBAutoUpdate.IsOpened = false;
                QBAutoUpdate.Description = "检查中";
                QBAutoUpdate.ThemeBrush = UserBrushes.ThemeBrushDefault;
                await Task.Run(() => SoftWareService.CheckUpdate());
                AppData.Local.UpdateLastTime = DateTime.Now;
            }
        }
        void ShowUpdateDialog()
        {
            Dispatcher.Invoke(() =>
            {
                AppData.NoticeHelper.Add(new NotificationInfo
                {
                    DateTime = DateTime.Now,
                    Title = "已下载更新",
                    Description = $"更新以下载完毕.点击[更新]启动.\n版本:{Version}",
                    Button = "更新",
                    ButtonEvent = "Update"
                });
            });
        }
        void ToUpdate()
        {
            string path = SoftWareService.ServiceCache + @"Updater\Edit_CommunityUpdater.exe";
            if (File.Exists(path))
            {
                Process.Start(path);
                Close();
            }
        }
        #endregion
        #region 通知
        public void InvokeNotice(NotificationInfo info)
        {
            NoticeDialog1.NotificationInfo = info;
            NoticeDialog1.Visibility = Visibility.Visible;
        }
        private void NoticeDialog_Choose(object sender, NotificationInfo e)
        {
            if (e.ButtonEvent == "Update")
            {
                ToUpdate();
            }
            NoticeDialog1.Visibility = Visibility.Collapsed;
        }
        private void NoticeDialog_ChooseToNotification(object sender, NotificationInfo e)
        {
            ImgNotice.IsChecked = true;
            ImgSelectedItem_Tapped(ImgNotice, null);
            NoticeDialog1.Visibility = Visibility.Collapsed;
        }
        private void NoticeDialog_Closed(object sender, NotificationInfo e)
        {
            NoticeDialog1.Visibility = Visibility.Collapsed;
        }
        public void LoadNotice()
        {
            StackPanelNotice.Children.Clear();
            foreach (var item in AppData.NoticeHelper.Notification.Reverse())
            {
                var noticedialog = new NoticeDialog() { Margin = new Thickness(10, 5, 10, 5), NotificationInfo = item };
                noticedialog.Closed += NoticeDialog_Closed_1;
                noticedialog.Choose += NoticeDialog_Choose_1;
                StackPanelNotice.Children.Add(noticedialog);
            }
        }
        private void NoticeDialog_Choose_1(object sender, NotificationInfo e)
        {
            NoticeDialog_Choose(sender, e);
            AppData.NoticeHelper.Remove(e);
        }
        private void NoticeDialog_Closed_1(object sender, NotificationInfo e)
        {
            AppData.NoticeHelper.Remove(e);
        }
        private void TriggerButtonNoticeRemoveAll_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.NoticeHelper.Clear();
        }
        public void OnDownloadNotice(int count)
        {
            this.NoticeDialog1.NotificationInfo = new NotificationInfo { DateTime = DateTime.Now, Title = string.Format("{0}个推送通知", count), Button = "", ButtonEvent = "" };
        }
        private async void TriggerButtonSenderNotice_Tapped(object sender, RoutedEventArgs e)
        {
            if (TbxNotice1.Text != "")
            {
                NotificationInfo notificationInfo = new NotificationInfo
                {
                    Title = TbxNotice1.Text,
                    Button = TbxNotice2.Text,
                    ButtonEvent = TbxNotice3.Text,
                    Description = TbxNotice4.Text,
                    DateTime = DateTime.Now,
                    DateTimeSpecified = true
                };
                TriggerButtonSenderNotice.IsOpened = false;
                bool result = await AppData.NoticeHelper.SendNoticeAsync(notificationInfo);
                TriggerButtonSenderNotice.IsOpened = true;
                if (result)
                {
                    TbxNotice1.Text = ""; TbxNotice2.Text = ""; TbxNotice3.Text = ""; TbxNotice4.Text = "";
                }
            }
        }
        private void ApplyNotice_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (GridNoticeSend.Visibility == Visibility.Collapsed)
            {
                GridNoticeSend.Visibility = Visibility.Visible;
                TriggerButtonSenderNotice.Visibility = Visibility.Visible;
            }
            else
            {
                GridNoticeSend.Visibility = Visibility.Collapsed;
                TriggerButtonSenderNotice.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
