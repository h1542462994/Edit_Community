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

namespace Edit_Community
{
    /// <summary>
    /// ExtensionPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExtensionPage : Page
    {
        public ExtensionPage()
        {
            InitializeComponent();
            this.Loaded += ExtensionPage_Loaded;
        }
        private void ExtensionPage_Loaded(object sender, RoutedEventArgs e)
        {
            TSText.IsChecked = AppData.MainWindow.QBHideText.IsChecked;
            LblText.Content = AppData.MainWindow.QBHideText.Description;
            TSEditMode.IsChecked = AppData.MainWindow.QBEditMod.IsChecked;
            LblEditMode.Content = AppData.MainWindow.QBEditMod.Description;
            TSWeather.IsChecked = AppData.MainWindow.QBWeather.IsChecked;
            LblWeather.Content = AppData.MainWindow.QBWeather.Description;
            TsTransprency.IsChecked = AppData.Local.AllowTranspancy;
            if (TsTransprency.IsChecked)
            {
                LblTranspancy_F.Content = "关";
            }
            else
            {
                LblTranspancy_F.Content = "开";
            }
        }
        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.PageNavigationHelper.Add(typeof(SettingsMainPage));
        }
        #region 交互
        private void TSText_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.MainWindow.QBHideText.IsChecked = TSText.IsChecked;
            AppData.MainWindow.QBHideText_Tapped(null, new RoutedEventArgs());
        }
        public void SetText()
        {
            TSText.InvokeCheck(AppData.MainWindow.QBHideText.IsChecked);
            LblText.Content = AppData.MainWindow.QBHideText.Description;
        }
        private void TSEditMode_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.MainWindow.QBEditMod.IsChecked = TSEditMode.IsChecked;
            AppData.MainWindow.QBEditMod_Tapped(null, new RoutedEventArgs());
        }
        public void SetEditMode()
        {
            TSEditMode.InvokeCheck(AppData.MainWindow.QBEditMod.IsChecked);
            LblEditMode.Content = AppData.MainWindow.QBEditMod.Description;
        }
        private void TSWeather_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.MainWindow.QBWeather.IsChecked = TSWeather.IsChecked;
            AppData.MainWindow.QBWeather_Tapped(null, new RoutedEventArgs());
        }
        public void SetWeather()
        {
            TSWeather.InvokeCheck(AppData.MainWindow.QBWeather.IsChecked);
            LblWeather.Content = AppData.MainWindow.QBWeather.Description;
        }
        private void TsTransprency_Tapped(object sender, RoutedEventArgs e)
        {
            AppData.Local.AllowTranspancy = TsTransprency.IsChecked;
            if (TsTransprency.IsChecked)
            {
                LblTranspancy_F.Content = "关";
            }
            else
            {
                LblTranspancy_F.Content = "开";
            }
            if (TsTransprency.IsChecked == AppData.MainWindow._AllowsTransprency)
            {
                LblTranspancy.Visibility = Visibility.Collapsed;
            }
            else
            {
                LblTranspancy.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
