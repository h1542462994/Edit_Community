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
            TSText.IsChecked = Area.MainWindow.QBHideText.IsChecked;
            LblText.Content = Area.MainWindow.QBHideText.Description;
            TSEditMode.IsChecked = Area.MainWindow.QBEditMod.IsChecked;
            LblEditMode.Content = Area.MainWindow.QBEditMod.Description;
            TSWeather.IsChecked = Area.MainWindow.QBWeather.IsChecked;
            LblWeather.Content = Area.MainWindow.QBWeather.Description;
        }
        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {
            Area.PageNavigationHelper.Add(typeof(SettingsMainPage));
        }
        #region 交互
        private void TSText_Tapped(object sender, RoutedEventArgs e)
        {
            Area.MainWindow.QBHideText.IsChecked = TSText.IsChecked;
            Area.MainWindow.QBHideText_Tapped(null, new RoutedEventArgs());
        }
        public void SetText()
        {
            TSText.InvokeCheck(Area.MainWindow.QBHideText.IsChecked);
            LblText.Content = Area.MainWindow.QBHideText.Description;
        }
        private void TSEditMode_Tapped(object sender, RoutedEventArgs e)
        {
            Area.MainWindow.QBEditMod.IsChecked = TSEditMode.IsChecked;
            Area.MainWindow.QBEditMod_Tapped(null, new RoutedEventArgs());
        }
        public void SetEditMode()
        {
            TSEditMode.InvokeCheck(Area.MainWindow.QBEditMod.IsChecked);
            LblEditMode.Content = Area.MainWindow.QBEditMod.Description;
        }
        private void TSWeather_Tapped(object sender, RoutedEventArgs e)
        {
            Area.MainWindow.QBWeather.IsChecked = TSWeather.IsChecked;
            Area.MainWindow.QBWeather_Tapped(null, new RoutedEventArgs());
        }
        public void SetWeather()
        {
            TSWeather.InvokeCheck(Area.MainWindow.QBWeather.IsChecked);
            LblWeather.Content = Area.MainWindow.QBWeather.Description;
        }
        private void TSAutoCheck_Tapped(object sender, RoutedEventArgs e)
        {
            Area.MainWindow.QBAutoCheck.IsChecked = TSAutoCheck.IsChecked;
            Area.MainWindow.QBAutoCheck_Tapped(null, new RoutedEventArgs());
        }
        public void SetAutoCheck()
        {
            TSAutoCheck.InvokeCheck(Area.MainWindow.QBAutoCheck.IsChecked);
        }
        #endregion
    }
}
