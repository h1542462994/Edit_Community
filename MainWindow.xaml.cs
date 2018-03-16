using System;
using System.Collections.Generic;
using System.Globalization;
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
using User.SoftWare;
using User.UI;

namespace Edit_Community
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 公用参数
        //enum MouseOnobject
        //{
        //    None,
        //    ElpC1,
        //    ElpC2,
        //    ElpR1,
        //    ElpR2,
        //    ElpR3,
        //    SliderFontSize,
        //    ImgEditBrush,
        //    ImgEditWeather,
        //    ImgEditSettings,
        //    ImgEditSearch,
        //    ImgEditOut,
        //}
        bool isWindowLoaded = false;
        bool isEditLoaded = false;
        int isEditChanged = 0;

        internal RichTextBox[] RTbx = new RichTextBox[6];
        internal Label[] LblF = new Label[2];
        internal Ellipse[] LblC = new Ellipse[9];
        internal Ellipse[] LblH = new Ellipse[4];
        //internal Border[] BdrBackgroundColorDefault = new Border[4];
        //internal Border[] BdrBackgroundColorHistory = new Border[4];
        const double elpWidth = 8;
        //MouseOnobject mouseOnobject = MouseOnobject.None;
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
        #endregion
        #region 计时器模块
        private void RegisterTimer()
        {
            Area.TimerInventory.Register(TimerDisplayName.ExitEdit, new TimerQueueInfo(Area.Local.ExitEditInterval, new EventHandler(Timer_ExitEdit), false, true));
            Area.TimerInventory.Register(TimerDisplayName.HideImg, new TimerQueueInfo(6, new EventHandler(Timer_HideImg), false, true));
            //Area.TimerInventory.Register(TimerDisplayName.SaveBitmap, new TimerQueueInfo(40, new EventHandler(Timer_SaveBitmap), false, true));
            Area.TimerInventory.Register(TimerDisplayName.HideMouse, new TimerQueueInfo(6, new EventHandler(Timer_HideMouse), false, true));
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
            Console.WriteLine("=> Timer_ExitEdit" + Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime);

            ExitEdit();
        }
        #endregion
        #region 全局
        public MainWindow()
        {
            InitializeComponent();
            Area.MainWindow = this;
            //Area.WhiteBoardWindow = new WhiteBoardWindow();
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
            //for (int i = 0; i < BdrBackgroundColorDefault.Length; i++)
            //{
            //    string displayname = "BdrBackgroundColorDefault" + i;
            //    BdrBackgroundColorDefault[i] = (Border)FindName(displayname);
            //    Color arg = Area.Local.EditBackgroundColorDefault[i];
            //    BdrBackgroundColorDefault[i].Background = new SolidColorBrush(arg);
            //    BdrBackgroundColorDefault[i].ToolTip = string.Format("{0},{1},{2},{3}", arg.A, arg.R, arg.G, arg.B);
            //    BdrBackgroundColorDefault[i].MouseUp += BdrEditBackgrounds_MouseUp;
            //}
            //for (int i = 0; i < BdrBackgroundColorHistory.Length; i++)
            //{
            //    string displayname = "BdrBackgroundColorHistory" + i;
            //    BdrBackgroundColorHistory[i] = (Border)FindName(displayname);
            //    BdrBackgroundColorHistory[i].MouseUp += BdrEditBackgrounds_MouseUp;
            //}

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

            gridBinding = new Dictionary<User.UI.TriggerImage, Grid>()
            {
                { ImgEditSettings,GridSettings}
                ,
            };

            Area.LoadBackgroundImage();

            this.EditICs.SaveBrushCallBack += Edit.SaveBrush;
            this.EditICs.SetPropertys(Area.Local.InkColorIndexProperty, Area.Local.InkPenWidthProperty);

            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Area.PageNavigationHelper.PageChanged += PageNavigationHelper_PageChanged;
            Area.PageNavigationHelper.Add(new SettingsMainPage());
            Area.Local.Flush();
            Edit.Load(DateTime.Now);
            Edit.GetInfos();
            Edit.SetInfos();
            RegisterTimer();
        }
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;

        //    Application.Current.Shutdown();
        //}
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Area.Local.IsFullScreen == false)
            {
                Area.Local.AppLocation = new Point(Left / ScreenSize.Width, Top / ScreenSize.Height);
                Area.Local.AppSize = new Size(Width / ScreenSize.Width, Height / ScreenSize.Height);
                Area.Local.IsMaxShow = WindowState == WindowState.Maximized;
            }
        }
        public void Local_PropertyChanged(USettingsProperty key, PropertyChangedEventargs e)
        {
            if (key == Area.Local.EditColorProperty)
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
            else if (key == Area.Local.EditColorHistoryProperty)
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
            else if (key == Area.Local.IsFullScreenProperty)
            {
                ImgFullScreen.IsChecked = (bool)e.NewValue;
                FullScreenChanged((bool)e.NewValue);
            }
            else if (key == Area.Local.EditBackgroundColorProperty)
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
            else if (key == Area.Local.IsEditBrushOpenProperty)
            {
                if (Area.Local.IsFullScreen)
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
            }
            else if (key == Area.Local.IsRtxHiddenProperty)
            {
                //this.ImgMenuEditHideRtx.IsChecked = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    this.BdrRtxBack.Visibility = Visibility.Visible;
                    this.GridEditRtx.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.BdrRtxBack.Visibility = Visibility.Hidden;
                    this.GridEditRtx.Visibility = Visibility.Visible;
                }
            }
        }
        public void Edit_PropertyChanged(USettingsProperty key, PropertyChangedEventargs e)
        {
            if (key == Area.Edit.ColumnDefiProperty)
            {
                double[] arg = (double[])e.NewValue;
                ColumnDefi0.Width = new GridLength(arg[0], GridUnitType.Star);
                ColumnDefi1.Width = new GridLength(arg[1], GridUnitType.Star);
                ColumnDefi2.Width = new GridLength(1 - arg[0] - arg[1], GridUnitType.Star);
                SetElpLocationAll();
            }
            else if (key == Area.Edit.RowDefi0Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU0.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD0.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == Area.Edit.RowDefi1Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU1.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD1.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == Area.Edit.RowDefi2Property)
            {
                double arg = (double)e.NewValue;
                RowDefiU2.Height = new GridLength(arg, GridUnitType.Star);
                RowDefiD2.Height = new GridLength(1 - arg, GridUnitType.Star);
            }
            else if (key == Area.Edit.ColumnElp0Property)
            {
                SetElpLocation(0, (double)e.NewValue);
            }
            else if (key == Area.Edit.ColumnElp1Property)
            {
                SetElpLocation(1, (double)e.NewValue);
            }
            else if (key == Area.Edit.RowElo0Property)
            {
                SetElpLocation(2, (double)e.NewValue);
            }
            else if (key == Area.Edit.RowElp1Property)
            {
                SetElpLocation(3, (double)e.NewValue);
            }
            else if (key == Area.Edit.RowElp2Property)
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
        Dictionary<User.UI.TriggerImage, Grid> gridBinding = new Dictionary<User.UI.TriggerImage, Grid>();
        /// <summary>
        /// 全屏状态改变.
        /// </summary>
        public void FullScreenChanged(bool isfullscreen)
        {
            if (isfullscreen)
            {
                if (IsWindowLoaded && WindowState == WindowState.Normal)//记录位置和大小.
                {
                    Area.Local.AppSize = new Size(Width / ScreenSize.Width, Height / ScreenSize.Height);
                    Area.Local.AppLocation = new Point(Left / ScreenSize.Width, Top / ScreenSize.Height);
                }
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Area.Local.IsMaxShow = WindowState == WindowState.Maximized;
                WindowState = WindowState.Normal;
                Left = 0;
                Top = 0;
                Width = ScreenSize.Width;
                Height = ScreenSize.Height;
                ImgEditBrush.Visibility = Visibility.Visible;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                if (Area.Local.IsMaxShow)
                {
                    WindowState = WindowState.Maximized;
                }
                Left = Area.Local.AppLocation.X * ScreenSize.Width;
                Top = Area.Local.AppLocation.Y * ScreenSize.Height;
                Width = Area.Local.AppSize.Width * ScreenSize.Width;
                Height = Area.Local.AppSize.Height * ScreenSize.Height;
                ImgEditBrush.Visibility = Visibility.Hidden;
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
        }
        private void ImgMenu_Tapped(object sender, RoutedEventArgs e)
        {
            IsMenuShow = !IsMenuShow;

        }
        private void ImgFullScreen_Tapped(object sender, RoutedEventArgs e)
        {
            Area.Local.IsFullScreen = !Area.Local.IsFullScreen;
            Area.Local.IsEditBrushOpen = Area.Local.IsEditBrushOpen;
        }
        private void ImgEditBrush_Tapped(object sender, RoutedEventArgs e)
        {
            Area.Local.IsEditBrushOpen = !Area.Local.IsEditBrushOpen;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetElpLocationAll();
            if (IsWindowLoaded)
            {
                Area.DialogInventory.Move();
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
                    Area.TimerInventory[TimerDisplayName.HideImg].TickTime = 1;
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
                Area.Local.IsFullScreen = !Area.Local.IsFullScreen;
            }
        }
        private void ImgSettingsBack_Tapped(object sender, RoutedEventArgs e)
        {
            Area.PageNavigationHelper.Back();
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
            if (Area.Edit.EditType != EditType.Mod)
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


            Area.DialogInventory.Hide(DialogDisplayName.GridEditBox);
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
            Area.TimerInventory[TimerDisplayName.HideMouse].IsStarted = false;
            //
            Mouse.OverrideCursor = Cursors.None;
        }
        private void ShowMouse()
        {
            Area.TimerInventory[TimerDisplayName.HideMouse].IsStarted = true;
            Area.TimerInventory[TimerDisplayName.HideMouse].TickTime = 1;
            //
            Mouse.OverrideCursor = null;
        }
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
            }
            else
            {
                GridMenuMore.Visibility = Visibility.Collapsed;
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
            double[] colmundefi = Area.Edit.ColumnDefi;
            double num = colmundefi[0] + colmundefi[1];
            if (mouseElpDownOnobject == ElpC1 || mouseElpDownOnobject == ElpC2)
            {
                v[1] = Tools.Checkdouble(mousepoint.Y / this.GridMain.ActualHeight, elpdefimin, 1 - elpdefimin);
                if (mouseElpDownOnobject == ElpC1)
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                         , Area.Local.ColumnDefiMin
                         , num - Area.Local.ColumnDefiMin);
                    Area.Edit.ColumnDefi = new double[] { v[0], num - v[0] };
                    Area.Edit.ColumnElp0 = v[1];
                    //ElpC1.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
                else
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                        , colmundefi[0] + Area.Local.ColumnDefiMin
                        , 1 - Area.Local.ColumnDefiMin);
                    Area.Edit.ColumnDefi = new double[] { colmundefi[0], v[0] - colmundefi[0] };
                    Area.Edit.ColumnElp1 = v[1];
                    //ElpC2.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
            }
            else if (mouseElpDownOnobject == ElpR1 || mouseElpDownOnobject == ElpR2 || mouseElpDownOnobject == ElpR3)
            {
                v[1] = Tools.Checkdouble(mousepoint.Y / GridMain.ActualHeight, Area.Local.RowDefiMin, 1 - Area.Local.RowDefiMin);
                if (mouseElpDownOnobject == ElpR1)
                {
                    v[0] = Tools.Checkdouble(mousepoint.X / (GridMain.ActualWidth * colmundefi[0]), elpdefimin, 1 - elpdefimin);
                    //ElpR1.Margin =  new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[0], -elpWidth, 0, 0);
                    Area.Edit.RowDefi0 = v[1];
                    Area.Edit.RowElp0 = v[0];
                }
                else if (mouseElpDownOnobject == ElpR2)
                {
                    v[0] = Tools.Checkdouble((mousepoint.X - GridMain.ActualWidth * colmundefi[0]) / (GridMain.ActualWidth * colmundefi[1]), elpdefimin, 1 - elpdefimin);
                    //ElpR2.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[1], -elpWidth, 0, 0);
                    Area.Edit.RowDefi1 = v[1];
                    Area.Edit.RowElp1 = v[0];
                }
                else
                {
                    v[0] = Tools.Checkdouble((mousepoint.X - GridMain.ActualWidth * num) / (GridMain.ActualWidth * (1 - num)), elpdefimin, 1 - elpdefimin);
                    //ElpR3.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * (1- num), -elpWidth, 0, 0);
                    Area.Edit.RowDefi2 = v[1];
                    Area.Edit.RowElp2 = v[0];
                }
            }
        }
        private void SetElpLocationAll()
        {
            if (IsWindowLoaded)
            {
                SetElpLocation(0, Area.Edit.ColumnElp0);
                SetElpLocation(1, Area.Edit.ColumnElp1);
                SetElpLocation(2, Area.Edit.RowElp0);
                SetElpLocation(3, Area.Edit.RowElp1);
                SetElpLocation(4, Area.Edit.RowElp2);
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
                    if (!Area.DialogInventory.Exists(DialogDisplayName.GridEditBox))
                    {
                        Rtx_GetSectionValue();
                        Area.DialogInventory.Show(DialogDisplayName.GridEditBox, new DialogAutoInfo(GridMain, GridEditBox, DialogAuto.Star, Mouse.GetPosition(GridMain), DialogType.Shadow, 30));
                        Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
                        Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
                    }
                }
                else if (Rtx_GetSectionIndex() == -1 || Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed)
                {
                    Area.DialogInventory.Hide(DialogDisplayName.GridEditBox);
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
                ElpR1.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * Area.Edit.ColumnDefi[0], -elpWidth, 0, 0);
            }
            else if (index == 3)
            {
                ElpR2.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * Area.Edit.ColumnDefi[1], -elpWidth, 0, 0);
            }
            else if (index == 4)
            {
                ElpR3.Margin = new Thickness(-elpWidth + value * GridMain.ActualWidth * (1 - Area.Edit.ColumnDefi[0] - Area.Edit.ColumnDefi[1]), -elpWidth, 0, 0);
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        #endregion
        #region RichTextBox操作
        private void Rtx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEditLoaded)
            {
                Area.TimerInventory[TimerDisplayName.ExitEdit].IsStarted = true;
                Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
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
            Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
            Area.TimerInventory[TimerDisplayName.ExitEdit].TickTime = 1;
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
                        if (sender.Equals(LblC[i]) && i < Area.Local.Editcolor.Length)
                        {
                            Rtx_SetSelctionValue("foreground", Area.Local.Editcolor[i]);
                            return;
                        }
                    }
                    for (int i = 0; i < LblH.Length; i++)
                    {
                        if (sender.Equals(LblH[i]) && i < Area.Local.EditcolorHistory.Length)
                        {
                            Rtx_SetSelctionValue("foreground", Area.Local.EditcolorHistory[i]);
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
                            if (i < Area.Local.Editcolor.Length)
                            {
                                ShowColorPicker(e, Area.Local.Editcolor[i], ColorPickTask.GridEditBox, i);
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
                    Area.MainWindow.SlideFontSize_SetValue(fontSize);
                }
                else
                {
                    Area.MainWindow.SlideFontSize_SetValue(-1);
                }
                if (fontBold == false)
                {
                    Area.MainWindow.LblEditF0.Background = Brushes.Transparent;
                }
                else
                {
                    Area.MainWindow.LblEditF0.Background = Area.CheckedBrush;
                }
                if (fontItalic == false)
                {
                    Area.MainWindow.LblEditF1.Background = Brushes.Transparent;
                }
                else
                {
                    Area.MainWindow.LblEditF1.Background = Area.CheckedBrush;
                }
                Area.MainWindow.UComboBox1.IsDropDownOpen = false;
                if (fontFamily == "" || fontFamily == null)
                {
                    Area.MainWindow.UComboBox1.Text = "[未识别]";
                }
                else
                {
                    Area.MainWindow.UComboBox1.Text = fontFamily;
                }
            }

            public int FontSize
            {
                get => fontSize;
                set
                {
                    if (value >= fontsizemin && value <= fontsizemax)
                    {
                        Area.MainWindow.Rtx_SetSelctionValue("fontsize", value);
                        fontSize = value;
                    }
                }
            }
            public string Fontfamily
            {
                get => fontFamily; set
                {
                    fontFamily = value;
                    Area.MainWindow.Rtx_SetSelctionValue("fontfamily", value);
                }
            }
            public bool Fontbold
            {
                get => fontBold; set
                {
                    fontBold = value;
                    if (value == false)
                    {
                        Area.MainWindow.LblEditF0.Background = Brushes.Transparent;
                    }
                    else
                    {
                        Area.MainWindow.LblEditF0.Background = Area.CheckedBrush;
                    }
                    Area.MainWindow.Rtx_SetSelctionValue("fontbold", value);
                }
            }
            public bool Fontitalic
            {
                get => fontItalic; set
                {
                    fontItalic = value;
                    if (value == false)
                    {
                        Area.MainWindow.LblEditF1.Background = Brushes.Transparent;
                    }
                    else
                    {
                        Area.MainWindow.LblEditF1.Background = Area.CheckedBrush;
                    }
                    Area.MainWindow.Rtx_SetSelctionValue("fontitalic", value);
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
            for (int i = 0; i < Area.Local.Editcolor.Length; i++)
            {
                if (Area.Local.Editcolor[i] == color)
                {
                    return;
                }
            }
            List<Color> colorlist = new List<Color>() { color };
            for (int i = 0; i < Area.Local.EditcolorHistory.Length; i++)
            {
                if (Area.Local.EditcolorHistory[i] != color)
                {
                    colorlist.Add(Area.Local.EditcolorHistory[i]);
                }
            }
            int num = colorlist.Count > 4 ? 4 : colorlist.Count;
            Area.Local.EditcolorHistory = colorlist.Take(num).ToArray();
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
            Area.DialogInventory.Show(DialogDisplayName.Dialog,
                   new DialogAutoInfo(GridDialog,ColorPicker1 ,
                   dialogAuto, e.GetPosition(GridMain), DialogType.Shadow, 30
                   , new Size(200, 230)));
            IsGridbackMousedown = false;
            GridDialogBack.Visibility = Visibility.Visible;
            this.colorPickTask = colorPickTask;
            this.colorpicktaskindex = colorpicktaskindex;
        }
        private void ColorPick_OkOrCancel_Click(object sender, PropertyChangedEventargs<User.UI.ColorP> e)
        {
            if (colorPickTask == ColorPickTask.GridEditBox)
            {
                Color[] arg = Area.Local.Editcolor;
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


                Area.Local.Editcolor = arg;
            }
            HideDialog();
        }
        private void HideDialog()
        {
            Area.DialogInventory.Hide(DialogDisplayName.Dialog);
            GridDialogBack.Visibility = Visibility.Hidden;
        }
        #endregion
        private void PageNavigationHelper_PageChanged(object sender, User.UI.PageNavigationEventargs e)
        {
            if (e.IsFirstPage)
            {
                ColumnDefiSettings.Width = new GridLength(0);
            }
            else
            {
                ColumnDefiSettings.Width = new GridLength(60);
            }
            FrameSettings.Content = Activator.CreateInstance(e.Page.GetType());
        }
    }
}
