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
    /// UComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class UComboBox : UserControl
    {
        public UComboBox()
        {
            InitializeComponent();
            for (int i = 0; i < Grids.Length; i++)
            {
                Grid grid = (Grid)FindName("Grid" + i);
                Grids[i] = grid;
                grid.MouseUp += GridMenu_MouseUp;
                grid.MouseMove += GridMenu_MouseMove;
                grid.MouseLeave += GridMeun_MouseLeave;
            }
        }
        Grid[] Grids = new Grid[5];
        string[] fontFamilies = new string[]
        {
            "Consolas","微软雅黑","黑体","等线","幼圆"
        };

        private void GridMeun_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid)sender).Background = Brushes.LightGray;
        }
        private void GridMenu_MouseMove(object sender, MouseEventArgs e)
        {
            ((Grid)sender).Background = Brushes.Gray;
        }
        private void GridMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton== MouseButton.Left)
            {
                for (int i = 0; i < Grids.Length; i++)
                {
                    if (Grids[i] == sender)
                    {
                        SelectionStringChanged?.Invoke(this, fontFamilies[i]);
                        IsDropDownOpen = false;
                        Text = fontFamilies[i];
                        return;
                    }
                }
            }
        }
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsDropDownOpen = true;
            }
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            GridTitle.Background = Brushes.Gray;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            GridTitle.Background = Brushes.Transparent;
        }
        public event EventHandler<string> SelectionStringChanged;
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(UComboBox), new PropertyMetadata(false, new PropertyChangedCallback(IsDropDownOpen_Changed)));
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UComboBox), new PropertyMetadata("显示文本", new PropertyChangedCallback(Text_Changed)));

        void OnIsDropDownOpenChanged()
        {
            if (IsDropDownOpen == false)
            {
                GridMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                GridMenu.Visibility = Visibility.Visible;
            }
        }
        void OnTextChanged()
        {
            TbkTitle.Text = Text;
            try
            {
                FontFamily fontFamily = new FontFamily(Text);
                TbkTitle.FontFamily = fontFamily;
            }
            catch (Exception)
            {

            }
        }

        private static void IsDropDownOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UComboBox)d).OnIsDropDownOpenChanged();
        }
        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UComboBox)d).OnTextChanged();
        }
    }
}
