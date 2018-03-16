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
            }
        }
        Ellipse[] Elps = new Ellipse[5];
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ElpBackground.Fill = new SolidColorBrush(Area.Local.EditBackgroundColor);
            ColorPicker1.ValueOld = new ColorP(Area.Local.EditBackgroundColorOld);
            ColorPicker1.Value = new ColorP(Area.Local.EditBackgroundColor);
            int length = Area.Local.EditBackgroundColorHistory.Length;
            if (length > 5) length = 5;
            for (int i = 0; i < 5; i++)
            {
                if (i<length)
                {
                    Elps[i].Fill = new SolidColorBrush(Area.Local.EditBackgroundColorHistory[i]);
                }
                else
                {
                    Elps[i].Fill = null;
                }
            }
        }
        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {
            Area.PageNavigationHelper.Add(new SettingsMainPage());
        }
        #region 背景颜色
        bool isElpBackgroundLeftMouseDown = false;
        private void ColorPicker1_ChooseOkOrCancel(object sender, User.SoftWare.PropertyChangedEventargs<ColorP> e)
        {
            Area.Local.EditBackgroundColorOld = e.OldValue.GetColor();
            ApplyEditBackgroundHistoryColor(e.NewValue.GetColor());
        }
        private void ColorPicker1_ValueChanged(object sender, User.SoftWare.PropertyChangedEventargs<ColorP> e)
        {
            ElpBackground.Fill = new SolidColorBrush(e.NewValue.GetColor());
            Area.Local.EditBackgroundColor = e.NewValue.GetColor();
        }
        void ApplyEditBackgroundHistoryColor(Color color)
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
    }
}
