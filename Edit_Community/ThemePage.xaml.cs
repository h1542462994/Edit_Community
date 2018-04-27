using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using User;
using User.UI;

namespace Edit_Community
{
    /// <summary>
    /// ThemePage.xaml 的交互逻辑
    /// </summary>
    public partial class ThemePage : Page
    {
        public ThemePage()
        {
            InitializeComponent();
            this.Loaded += Page_Loaded;
            for (int i = 0; i < Elps.Length; i++)
            {
                Elps[i] = (Ellipse)FindName("Elp" + i);
                Elps[i].MouseDown += Elps_MouseDown;
                Elps[i].MouseLeave += Elps_MouseLeave;
                Elps[i].MouseUp += Elps_MouseUp;
            }
            BackgroundPicLoad();
        }
        Ellipse[] Elps = new Ellipse[5];
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadElpsColor();
            SlideTimeLoad(true);
            CheckPic();
            ElpBackground.Fill = new SolidColorBrush(AppData.Local.EditBackgroundColor);
            ColorPicker1.ValueOld = new ColorP(AppData.Local.EditBackgroundColorOld);
            ColorPicker1.Value = new ColorP(AppData.Local.EditBackgroundColor);
        }
        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.PageNavigationHelper.Add(typeof(SettingsMainPage));
        }
        #region 背景颜色
        bool isElpLeftMouseDown;
        private void ColorPicker1_ChooseOkOrCancel(object sender, UPropertyChangedEventArgs<ColorP> e)
        {
            AppData.Local.EditBackgroundColorOld = e.OldValue.GetColor();
            ApplyEditBackgroundHistoryColor(e.NewValue.GetColor());
        }
        private void ColorPicker1_ValueChanged(object sender, UPropertyChangedEventArgs<ColorP> e)
        {
            ElpBackground.Fill = new SolidColorBrush(e.NewValue.GetColor());
            AppData.Local.EditBackgroundColor = e.NewValue.GetColor();
        }
        private void Elps_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isElpLeftMouseDown = e.ChangedButton == MouseButton.Left;
        }
        private void Elps_MouseLeave(object sender, MouseEventArgs e)
        {
            isElpLeftMouseDown = false;
        }
        private void Elps_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isElpLeftMouseDown)
            {
                for (int i = 0; i < Elps.Length; i++)
                {
                    if (sender == Elps[i] && i<AppData.Local.EditBackgroundColorHistory.Length)
                    {
                        ColorPicker1.Value = new ColorP(AppData.Local.EditBackgroundColorHistory[i]);
                        break;
                    }
                }
            }
        }
        void ApplyEditBackgroundHistoryColor(Color color)
        {
            for (int i = 0; i < AppData.Local.EditBackgroundColorHistory.Length; i++)
            {
                if (AppData.Local.EditBackgroundColorHistory[i] == color)
                {
                    return;
                }
            }
            List<Color> colorlist = new List<Color>() { color };
            for (int i = 0; i < AppData.Local.EditBackgroundColorHistory.Length; i++)
            {
                if (AppData.Local.EditBackgroundColorHistory[i] != color)
                {
                    colorlist.Add(AppData.Local.EditBackgroundColorHistory[i]);
                }
            }
            int num = colorlist.Count > 5 ? 5 : colorlist.Count;
            AppData.Local.EditBackgroundColorHistory = colorlist.Take(num).ToArray();

            LoadElpsColor();
        }
        void LoadElpsColor()
        {
            int length = AppData.Local.EditBackgroundColorHistory.Length > 5 ? 5 :
                AppData.Local.EditBackgroundColorHistory.Length;
            for (int i = 0; i < 5; i++)
            {
                if (i < length)
                {
                    Elps[i].Fill = new SolidColorBrush(AppData.Local.EditBackgroundColorHistory[i]);
                }
                else
                {
                    Elps[i].Fill = null;
                }
            }
        }
        #endregion
        #region 背景图片
        void BackgroundPicLoad()
        {
            if (AppData.Local.BackgroundMode == 0)
            {
                Grid1.Visibility = Visibility.Collapsed;
                GridSlideTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                Grid1.Visibility = Visibility.Visible;
                if (AppData.Local.BackgroundMode == 1)
                {
                    BtnChoose.InnerContent = "选择图片";
                    BtnSlideNext.Visibility = Visibility.Collapsed;
                    GridSlideTime.Visibility = Visibility.Collapsed;
                }
                else
                {
                    BtnChoose.InnerContent = "选择文件夹";
                    BtnSlideNext.Visibility = Visibility.Visible;
                    GridSlideTime.Visibility = Visibility.Visible;
                }
            }
            if (!IsLoaded)
            {
                ComboBox1.SelectedIndex = AppData.Local.BackgroundMode;
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                AppData.Local.BackgroundMode = ComboBox1.SelectedIndex;
                BackgroundPicLoad();
                AppData.MainWindow.OnBackgrondPic(AppData.Local.BackgroundMode, true, false);
            }
        }
        private void BtnChoose_Tapped(object sender, RoutedEventArgs e)
        {
            if (AppData.Local.BackgroundMode == 1)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "图像文件(*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png"
                };
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    AppData.Local.BackgroundPicPath = openFileDialog.FileName;
                    AppData.MainWindow.OnBackgrondPic(AppData.Local.BackgroundMode);
                }
            }
            else if (AppData.Local.BackgroundMode == 2)
            {
                var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog()
                {

                };
                var result = openFolderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    AppData.Local.BackgroundPicFolder = openFolderDialog.SelectedPath;
                    AppData.MainWindow.OnBackgrondPic(AppData.Local.BackgroundMode,isnext: false);
                }
            }
        }
        private void BtnSlideNext_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.MainWindow.OnBackgrondPic(AppData.Local.BackgroundMode,true);
        }

        public void SlideTimeLoad(bool isFromSetting)
        {
            if (isFromSetting)
            {
                SlideBarTime.SlideValue = AppData.Local.BackgroundPicTimestamp;
            }
            else
            {
                AppData.Local.BackgroundPicTimestamp = SlideBarTime.SlideValue;
            }
            double v = ((int)(AppData.Local.BackgroundPicTimestamp * 10)) / 10.0;
            LblTime.Content = v;
            SlideBarTime.TickValue = (DateTime.Now - AppData.Local.BackgroundPicLastTime).TotalMinutes;
        }
        private void SlideBarTime_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SlideTimeLoad(false);
            TimeSpan defaultTimeSpan = TimeSpan.FromMinutes(AppData.Local.BackgroundPicTimestamp);
            TimeSpan currentTimeSpan = DateTime.Now - AppData.Local.BackgroundPicLastTime;
            double percent = currentTimeSpan.TotalMinutes / defaultTimeSpan.TotalMinutes;
            AppData.MainWindow. QBBackgroundNext.ThemeBrush  = UserBrushes.GetLinearGradiantBrush(UserBrushes.ThemeColorDefault, Color.FromArgb(204, 51, 51, 51), percent);
        }
        public void CheckPic()
        {
            if (AppData.Local.BackgroundMode == 1)
            {
                string path = AppData.Local.BackgroundPicPath;
                if (!File.Exists(path) || !(path.Contains(".jpg" ) || path.Contains(".png")|| path.Contains(".bmp")))
                {
                    PushText("图片路径无效", Colors.OrangeRed);
                }
                else
                {
                    PushText("已加载图片", Colors.SpringGreen);
                }
            }
            if (AppData.Local.BackgroundMode == 2)
            {
                if (AppData.Local.BackgroundPicFolder!="" && Directory.Exists(AppData.Local.BackgroundPicFolder))
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
                    if (infos.Count == 0)
                    {
                        PushText("文件夹为空", Colors.OrangeRed);
                    }
                    else
                    {
                        PushText(string.Format("幻灯片:{0}/{1}", AppData.Local.BackgroundPicCurrentindex + 1, infos.Count), Colors.SpringGreen);
                    }
                }
                else
                {
                    PushText("文件夹路径无效", Colors.OrangeRed);
                }
            }
        }
        void PushText(string arg, Color color)
        {
            LblBackgroundPicFail.Content = arg;
            LblBackgroundPicFail.Foreground = new SolidColorBrush( color);
        }
        #endregion
    }
}
