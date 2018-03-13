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
using Library;
using User.SoftWare;

namespace Edit_Community
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 公用参数
        enum MouseOnobject
        {
            None,
            ElpC1,
            ElpC2,
            ElpR1,
            ElpR2,
            ElpR3,
            SliderFontSize,
            ImgEditBrush,
            ImgEditWeather,
            ImgEditSettings,
            ImgEditSearch,
            ImgEditOut,
        }
        bool isWindowLoaded = false;
        bool isEditLoaded = false;
        int isEditChanged = 0;
        bool isMenuShow = false;
        internal RichTextBox[] RTbx = new RichTextBox[6];
        internal Label[] LblF = new Label[2];
        internal Ellipse[] LblC =  new Ellipse [9];
        internal Ellipse[] LblH = new Ellipse[4];
        internal Border[] BdrBackgroundColorDefault = new Border[4];
        internal Border[] BdrBackgroundColorHistory = new Border[4];
        const double elpWidth = 8;
        MouseOnobject mouseOnobject = MouseOnobject.None;
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

        public bool IsWindowLoaded { get => isWindowLoaded; set => isWindowLoaded = value; }
        public bool IsEditLoaded { get => isEditLoaded; set => isEditLoaded = value; }

        public bool IsMenuShow
        {
            get => isMenuShow;
            set
            {
                if (isMenuShow != value)
                {
                    Imgmenu.IsChecked = value;
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
        //private void Timer_SaveBitmap(object sender, EventArgs e)
        //{
        //    if (IsEditChanged < 2)
        //    {
        //        RtxCopyToVisual();
        //        //Edit.SaveRtfBitmap();
        //        IsEditChanged++;
        //        Console.WriteLine("Edit=>SaveBitmap");
        //    }
        //}
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
            for (int i = 0; i < BdrBackgroundColorDefault.Length; i++)
            {
                string displayname = "BdrBackgroundColorDefault" + i;
                BdrBackgroundColorDefault[i] = (Border)FindName(displayname);
                Color arg = Area.Local.EditBackgroundColorDefault[i];
                BdrBackgroundColorDefault[i].Background = new SolidColorBrush(arg);
                BdrBackgroundColorDefault[i].ToolTip = string.Format("{0},{1},{2},{3}", arg.A, arg.R, arg.G, arg.B);
                BdrBackgroundColorDefault[i].MouseUp += BdrEditBackgrounds_MouseUp;
            }
            for (int i = 0; i < BdrBackgroundColorHistory.Length; i++)
            {
                string displayname = "BdrBackgroundColorHistory" + i;
                BdrBackgroundColorHistory[i] = (Border)FindName(displayname);
                BdrBackgroundColorHistory[i].MouseUp += BdrEditBackgrounds_MouseUp;
            }

            UComboBox1.SelectionStringChanged += UComboBox1_SelectionStringChanged;
            TbxTemp.GotFocus += TbxTemp_GotFocus;
            TbxTemp.LostFocus += TbxTemp_LostFocus;
            ImgBefore.MouseUp += ImgMove_MouseUp;
            ImgNext.MouseUp += ImgMove_MouseUp;
            for (int i = 0; i < RTbx.Length; i++)
            {
                RTbx[i].SelectionChanged += Rtx_SelectionChanged;
                RTbx[i].TextChanged += Rtx_TextChanged;
                RTbx[i].GotFocus += Rtx_GotFocus;
                InputMethod.SetPreferredImeState(RTbx[i], InputMethodState.DoNotCare);
                InputLanguageManager.SetInputLanguage(RTbx[i], CultureInfo.GetCultureInfo("zh-CN"));
            }

            Area.LoadBackgroundImage();
            this.EditICs.SaveBrushCallBack += Edit.SaveBrush;
            this.EditICs.SetPropertys(Area.Local.inkColorIndex, Area.Local.inkPenwidth);

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
        private void ImgMove_MouseUp(object sender, MouseButtonEventArgs e)
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Area.Local.IsFullScreen)
            {
                Area.Local.AppLocation = new Point(Left / Area.ScreenSize.Width, Top / Area.ScreenSize.Height);
                Area.Local.AppSize = new Size(Width / Area.ScreenSize.Width, Height / Area.ScreenSize.Height);
                Area.Local.IsMaxShow = WindowState == WindowState.Maximized;
            }
        }
        private void ImgfullScreen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Area.Local.IsFullScreen = !Area.Local.IsFullScreen;
            Area.Local.IsEditBrushOpen = Area.Local.IsEditBrushOpen;
        }
        private void Imgmenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMenuShow = !IsMenuShow;
        }
        /// <summary>
        /// 与事件的交互
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件信息.</param>
        public void PropertyChanged(object sender, UPropertyChangedEventargs e)
        {
            if (sender.Equals(Area.Local.editcolor))
            {
                Console.WriteLine("isEditColor => ???");
                Color[] arg = (Color[])e.Newvalue;
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
            else if (sender.Equals(Area.Local.editBackgroundColorHistory))
            {
                Console.WriteLine("iseditBackgroundColorHistory => ???");
                Color[] arg = (Color[])e.Newvalue;
                for (int i = 0; i < BdrBackgroundColorHistory.Length; i++)
                {
                    if (i < arg.Length)
                    {
                        BdrBackgroundColorHistory[i].Background = new SolidColorBrush(arg[i]);
                        BdrBackgroundColorHistory[i].ToolTip = string.Format("{0},{1},{2},{3}", arg[i].A, arg[i].R, arg[i].G, arg[i].B);
                    }
                    else
                    {
                        BdrBackgroundColorHistory[i].Background = Brushes.Transparent;
                        BdrBackgroundColorHistory[i].ToolTip = null;
                    }
                }
            }
            else if (sender.Equals(Area.Local.editcolorHistory))
            {
                Console.WriteLine("iseditcolorHistory => ???");
                Color[] arg = (Color[])e.Newvalue;
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
            if (Area.Edit != null)
            {
                if (sender.Equals(Area.Edit.columnDefi))
                {
                    double[] arg = (double[])e.Newvalue;
                    ColumnDefi0.Width = new GridLength(arg[0], GridUnitType.Star);
                    ColumnDefi1.Width = new GridLength(arg[1], GridUnitType.Star);
                    ColumnDefi2.Width = new GridLength(1 - arg[0] - arg[1], GridUnitType.Star);
                    SetElpLocation();
                }
            }
            if (e.IsChanged)
            {
                if (sender.Equals(Area.Local.isFullScreen))
                {
                    ImgfullScreen.IsChecked = (bool)e.Newvalue;
                    Area.FullScreenChanged((bool)e.Newvalue);
                }
                if (sender.Equals(Area.Local.editBackgroundColor))
                {
                    Color arg = (Color)e.Newvalue;
                    BdrEditBackground.Background = new SolidColorBrush(arg);
                    BdrEditBackground.ToolTip = string.Format("{0},{1},{2},{3}", arg.A, arg.R, arg.G, arg.B);
                    BdrRtxBack.Background = new SolidColorBrush((Color)e.Newvalue);
                    for (int i = 0; i < RTbx.Length; i++)
                    {
                        RTbx[i].Background = new SolidColorBrush((Color)e.Newvalue);
                    }
                }
                if (sender.Equals(Area.Local.isEditBrushOpen))
                {
                    if (Area.Local.IsFullScreen)
                    {
                        this.ImgEditBrush.IsChecked = (bool)e.Newvalue;
                        if ((bool)e.Newvalue)
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
                if (sender.Equals(Area.Local.isRtxHidden))
                {
                    this.ImgMenuEditHideRtx.IsChecked = (bool)e.Newvalue;
                    if ((bool)e.Newvalue)
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

                if (Area.Edit != null)
                {
                    if (sender.Equals(Area.Edit.rowDefi0))
                    {
                        double arg = (double)e.Newvalue;
                        RowDefiU0.Height = new GridLength(arg, GridUnitType.Star);
                        RowDefiD0.Height = new GridLength(1 - arg, GridUnitType.Star);
                    }
                    else if (sender.Equals(Area.Edit.rowDefi1))
                    {
                        double arg = (double)e.Newvalue;
                        RowDefiU1.Height = new GridLength(arg, GridUnitType.Star);
                        RowDefiD1.Height = new GridLength(1 - arg, GridUnitType.Star);
                    }
                    else if (sender.Equals(Area.Edit.rowDefi2))
                    {
                        double arg = (double)e.Newvalue;
                        RowDefiU2.Height = new GridLength(arg, GridUnitType.Star);
                        RowDefiD2.Height = new GridLength(1 - arg, GridUnitType.Star);
                    }
                    else if (sender.Equals(Area.Edit.columnElp0))
                    {
                        SetElpLocation(0, (double)e.Newvalue);
                    }
                    else if (sender.Equals(Area.Edit.columnElp1))
                    {
                        SetElpLocation(1, (double)e.Newvalue);
                    }
                    else if (sender.Equals(Area.Edit.rowElp0))
                    {
                        SetElpLocation(2, (double)e.Newvalue);
                    }
                    else if (sender.Equals(Area.Edit.rowElp1))
                    {
                        SetElpLocation(3, (double)e.Newvalue);
                    }
                    else if (sender.Equals(Area.Edit.rowElp2))
                    {
                        SetElpLocation(4, (double)e.Newvalue);
                    }
                }
            }


            Console.WriteLine("PropertyChaned");
        }
        #endregion
        #region 菜单交互操作

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
            SetMouseOnobject(sender);
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
        #region 菜单项-Edit
        enum _EditMenuName
        {
            Settings,
        }
        _EditMenuName editMenuName;
        _EditMenuName EditMenuName { get => editMenuName; set => editMenuName = value; }
        private void ImgAll_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("ImgAll -> MouseUp");
            if (GetMouseOnobject(sender) == mouseOnobject)
            {
                if (mouseOnobject == MouseOnobject.ImgEditSettings || mouseOnobject == MouseOnobject.ImgEditOut || mouseOnobject == MouseOnobject.ImgEditSearch)
                {
                    SetImgEditChecked(sender);
                }
                else if (mouseOnobject == MouseOnobject.ImgEditBrush)
                {
                    Area.Local.IsEditBrushOpen = !Area.Local.IsEditBrushOpen;
                }
                else if (mouseOnobject == MouseOnobject.ImgEditWeather)
                {
                    Area.Local.IsWeatherOpen = !Area.Local.IsWeatherOpen;
                }
            }
        }
        private void ImgAll_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("ImgAll -> MouseDown");
            SetMouseOnobject(sender);
            Console.WriteLine(mouseOnobject);
        }
        private void SetImgEditChecked(object sender)
        {
            ImgMenu[] obj = new ImgMenu[] { ImgEditSettings };
            Grid[] linkgrid = new Grid[] { GridMenuEditSettings };
            for (int i = 0; i < obj.Length; i++)
            {
                if (sender.Equals(obj[i]))
                {
                    obj[i].IsChecked = !obj[i].IsChecked;
                    if (obj[i].IsChecked)
                    {
                        GridMenuMore.Visibility = Visibility.Visible;
                        linkgrid[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        GridMenuMore.Visibility = Visibility.Hidden;
                        linkgrid[i].Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    obj[i].IsChecked = false;
                    linkgrid[i].Visibility = Visibility.Hidden;
                }
            }
        }
        #region Settings分项
        private void BdrEditBackground_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowColorPicker(e, Area.Local.EditBackgroundColor, ColorPickTask.EditBackColor, 0, DialogAuto.Absolute, true);
        }
        private void BdrEditBackgrounds_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < BdrBackgroundColorDefault.Length; i++)
            {
                if (sender.Equals(BdrBackgroundColorDefault[i]))
                {
                    Area.Local.EditBackgroundColor = Area.Local.EditBackgroundColorDefault[i];
                    return;
                }
            }
            for (int i = 0; i < BdrBackgroundColorHistory.Length; i++)
            {
                if (i < Area.Local.EditBackgroundColorHistory.Length && sender.Equals(BdrBackgroundColorHistory[i]))
                {
                    Area.Local.EditBackgroundColor = Area.Local.EditBackgroundColorHistory[i];
                    return;
                }
            }
        }

        private void ApplyBackgroundColorHistory(Color color)
        {
            for (int i = 0; i < Area.Local.EditBackgroundColorDefault.Length; i++)
            {
                if (Area.Local.EditBackgroundColorDefault[i] == color)
                {
                    return;
                }
            }
            List<Color> colorlist = new List<Color>() { color };
            for (int i = 0; i < Area.Local.EditBackgroundColorHistory.Length; i++)
            {
                if (Area.Local.EditBackgroundColorHistory[i] != color)
                {
                    colorlist.Add(Area.Local.EditBackgroundColorHistory[i]);
                }
            }
            int num = colorlist.Count > 4 ? 4 : colorlist.Count;
            Area.Local.EditBackgroundColorHistory = colorlist.Take(num).ToArray();
        }
        private void ImgMenuEditMod_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //ImgMenuEditMod.IsChecked = !ImgMenuEditMod.IsChecked;
            if (ImgMenuEditMod.IsChecked)
            {
                Edit.LoadMod();
            }
            else
            {
                Edit.ExitMod();
            }
        }
        private void ImgMenuHideRtx_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Area.Local.IsRtxHidden = !Area.Local.IsRtxHidden;
        }
        #endregion
        #endregion
        #region 布局
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetElpLocation();
            Area.Local.GridWeatherLocation = Area.Local.GridWeatherLocation;
            if (IsWindowLoaded)
            {
                Area.DialogInventory.Move();
            }
        }
        private void Elp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetMouseOnobject(sender);
            Ismouseleftdown = e.LeftButton == MouseButtonState.Pressed;
            Elp_SetValue(e.GetPosition(GridMain));
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
            else
            {
                Elp_SetValue(e.GetPosition(GridMain));
                Console.WriteLine("=>(Down)Window MouseMove");
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Ismouseleftdown && (mouseOnobject != MouseOnobject.None && mouseOnobject != MouseOnobject.SliderFontSize))
            {
                ExitEdit();
            }
            mouseOnobject = MouseOnobject.None;
            Console.WriteLine("=>WindowMouseUp");
        }
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseOnobject = MouseOnobject.None;
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
        private MouseOnobject GetMouseOnobject(object obj)
        {
            if (obj.Equals(ElpC1))
                return MouseOnobject.ElpC1;
            else if (obj.Equals(ElpC2))
                return MouseOnobject.ElpC2;
            else if (obj.Equals(ElpR1))
                return MouseOnobject.ElpR1;
            else if (obj.Equals(ElpR2))
                return MouseOnobject.ElpR2;
            else if (obj.Equals(ElpR3))
                return MouseOnobject.ElpR3;
            else if (obj.Equals(SlideFontSize))
                return MouseOnobject.SliderFontSize;
            else if (obj.Equals(ImgEditBrush))
                return MouseOnobject.ImgEditBrush;
            else if (obj.Equals(ImgEditSettings))
                return MouseOnobject.ImgEditSettings;
            else
                return MouseOnobject.None;
        }
        private void SetMouseOnobject(object obj)
        {
            mouseOnobject = GetMouseOnobject(obj);
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
        private void SetElpLocation()
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
        private void Elp_SetValue(Point mousepoint)
        {
            double[] v = new double[2];
            double[] colmundefi = Area.Edit.ColumnDefi;
            double num = colmundefi[0] + colmundefi[1];
            if (mouseOnobject == MouseOnobject.ElpC1 || mouseOnobject == MouseOnobject.ElpC2)
            {
                v[1] = Switcher.Checkdouble(mousepoint.Y / this.GridMain.ActualHeight, elpdefimin, 1 - elpdefimin);
                if (mouseOnobject == MouseOnobject.ElpC1)
                {
                    v[0] = Switcher.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                         , Area.Local.ColumnDefiMin
                         , num - Area.Local.ColumnDefiMin);
                    Area.Edit.ColumnDefi = new double[] { v[0], num - v[0] };
                    Area.Edit.ColumnElp0 = v[1];
                    //ElpC1.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
                else
                {
                    v[0] = Switcher.Checkdouble(mousepoint.X / this.GridMain.ActualWidth
                        , colmundefi[0] + Area.Local.ColumnDefiMin
                        , 1 - Area.Local.ColumnDefiMin);
                    Area.Edit.ColumnDefi = new double[] { colmundefi[0], v[0] - colmundefi[0] };
                    Area.Edit.ColumnElp1 = v[1];
                    //ElpC2.Margin = new Thickness(-elpWidth, -elpWidth + v[1] * GridMain.ActualHeight, 0, 0);
                }
            }
            else if (mouseOnobject == MouseOnobject.ElpR1 || mouseOnobject == MouseOnobject.ElpR2 || mouseOnobject == MouseOnobject.ElpR3)
            {
                v[1] = Switcher.Checkdouble(mousepoint.Y / GridMain.ActualHeight, Area.Local.RowDefiMin, 1 - Area.Local.RowDefiMin);
                if (mouseOnobject == MouseOnobject.ElpR1)
                {
                    v[0] = Switcher.Checkdouble(mousepoint.X / (GridMain.ActualWidth * colmundefi[0]), elpdefimin, 1 - elpdefimin);
                    //ElpR1.Margin =  new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[0], -elpWidth, 0, 0);
                    Area.Edit.RowDefi0 = v[1];
                    Area.Edit.RowElp0 = v[0];
                }
                else if (mouseOnobject == MouseOnobject.ElpR2)
                {
                    v[0] = Switcher.Checkdouble((mousepoint.X - GridMain.ActualWidth * colmundefi[0]) / (GridMain.ActualWidth * colmundefi[1]), elpdefimin, 1 - elpdefimin);
                    //ElpR2.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * colmundefi[1], -elpWidth, 0, 0);
                    Area.Edit.RowDefi1 = v[1];
                    Area.Edit.RowElp1 = v[0];
                }
                else
                {
                    v[0] = Switcher.Checkdouble((mousepoint.X - GridMain.ActualWidth * num) / (GridMain.ActualWidth * (1 - num)), elpdefimin, 1 - elpdefimin);
                    //ElpR3.Margin = new Thickness(-elpWidth + v[0] * GridMain.ActualWidth * (1- num), -elpWidth, 0, 0);
                    Area.Edit.RowDefi2 = v[1];
                    Area.Edit.RowElp2 = v[0];
                }
            }
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
        private void ShowImg()
        {
            ImgfullScreen.Visibility = Visibility.Visible;
            Imgmenu.Visibility = Visibility.Visible;
            if (Imgmenu.IsChecked)
            {
                GridMenu.Visibility = Visibility.Visible;
            }
            //GridMenuMore.Visibility = Visibility.Visible;
        }
        private void HideImg()
        {
            ImgfullScreen.Visibility = Visibility.Hidden;
            Imgmenu.Visibility = Visibility.Hidden;
            GridMenu.Visibility = Visibility.Hidden;
            //GridMenuMore.Visibility = Visibility.Hidden;
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

        #endregion
        #region 交互
        enum ColorPickTask
        {
            None,
            GridEditBox,
            EditBackColor
        }
        ColorPickTask colorPickTask;
        int colorpicktaskindex;
        private void ColorPick_Ok_Click(object sender, UPropertyChangedEventargs e)
        {
            if (colorPickTask == ColorPickTask.GridEditBox)
            {
                Color[] arg = Area.Local.Editcolor;
                if (colorpicktaskindex < arg.Length)
                {
                    arg[colorpicktaskindex] = ((ColorP)e.Newvalue).GetColor();
                }
                else
                {
                    Color[] argclone = new Color[colorpicktaskindex + 1];
                    for (int i = 0; i < arg.Length; i++)
                    {
                        argclone[i] = arg[i];
                    }
                    argclone[colorpicktaskindex] = ((ColorP)e.Newvalue).GetColor();
                    arg = argclone;
                }


                Area.Local.Editcolor = arg;
            }
            else if (colorPickTask == ColorPickTask.EditBackColor)
            {
                Area.Local.EditBackgroundColor = ((ColorP)e.Newvalue).GetColor();
                ApplyBackgroundColorHistory(((ColorP)e.Newvalue).GetColor());
            }
            HideDialog();
        }
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
            Area.DialogInventory.Show(DialogDisplayName.Dialog,
                   new DialogAutoInfo(GridDialog, new ColorPicker(new ColorP(color), new UPropertyChangedEventHander(ColorPick_Ok_Click), caneditalpha),
                   dialogAuto, e.GetPosition(GridMain), DialogType.Dialog, 30
                   , new Size(200, 230)));
            IsGridbackMousedown = false;
            GridDialogBack.Visibility = Visibility.Visible;
            this.colorPickTask = colorPickTask;
            this.colorpicktaskindex = colorpicktaskindex;
        }
        private void HideDialog()
        {
            Area.DialogInventory.Hide(DialogDisplayName.Dialog);
            GridDialog.Visibility = Visibility.Hidden;
            GridDialogBack.Visibility = Visibility.Hidden;
        }
        private void MessageBox1_MouseUp(object sender, EventArgs e)
        {
            Area.Local.IsFullScreen = false;
            Area.Local.IsEditBrushOpen = false;
            HideDialog();
        }
        private void MessageBox2_MouseUp(object sender, EventArgs e)
        {
            Area.Local.IsFullScreen = true;
            Area.Local.IsEditBrushOpen = true;
            HideDialog();
        }



        #endregion
        private void PageNavigationHelper_PageChanged(object sender, User.UI. PageNavigationEventargs e)
        {
            if (e.IsFirstPage)
            {
                ColumnDefiSettings.Width = new GridLength(0);
            }
            else
            {
                ColumnDefiSettings.Width = new GridLength(60);
            }
            FrameSettings.Content = e.Page;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                Area.Local.IsFullScreen = !Area.Local.IsFullScreen;
            }
        }

        private void ImgMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Area.PageNavigationHelper.Back();
        }
    }
}
