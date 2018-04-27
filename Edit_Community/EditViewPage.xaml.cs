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
    /// EditViewPage.xaml 的交互逻辑
    /// </summary>
    public partial class EditViewPage : Page
    {
        public EditViewPage()
        {
            InitializeComponent();
        }
        public void Load()
        {
            StackPanel1.Children.Clear();
            for (int i = AppData.EditInfos.Length; i >= 0; i--)
            {
                EditItemView view = null;
                if (i == AppData.EditInfos.Length)
                {
                    view = new EditItemView() { EditInfo = EditTemp.GetModEditInfo(), VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 0, 0, 5) };
                }
                else
                {
                    view = new EditItemView() { EditInfo = AppData.EditInfos[i], VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 5, 0, 5) };
                }
                StackPanel1.Children.Add(view);
            }
        }
    }
}
