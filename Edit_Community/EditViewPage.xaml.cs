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
                int j = AppData.EditInfos.Length - i;
                EditItemView view = null;
                if (i == AppData.EditInfos.Length)
                {
                    view = new EditItemView() { EditInfo = EditTemp.GetModEditInfo(), VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 0, 0, 5) };
                }
                else
                {
                    view = new EditItemView() { EditInfo = AppData.EditInfos[i], VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 5, 0, 5) };
                }
                view.Index = j;
                view.LoadContent();
                view.SlideTapped += View_SlideTapped;
                
                StackPanel1.Children.Add(view);
            }
        }
        private void View_SlideTapped(object sender, RoutedEventArgs e)
        {
            foreach (var item in StackPanel1.Children)
            {
                if (item is EditItemView view)
                {
                    if (sender.Equals(item))
                    {
                        view.IsContentOpened = !view.IsContentOpened;
                    }
                    else
                    {
                        view.IsContentOpened = false;
                    }
                }
            }
        }

        void Select(EditInfo info)
        {
            foreach (var item in StackPanel1.Children)
            {
                if (item is EditItemView view)
                {
                    if (view.EditInfo == info)
                    {
                        view.BorderThickness = new Thickness(2);
                        view.BorderBrush = Brushes.Orange;

                    }
                    else
                    {
                        view.BorderThickness = new Thickness(0);
                    }
                }
            }
        }
        public static void Select()
        {
            if (AppData.MainWindow.FrameEditView.Content is EditViewPage page)
            {
                EditInfo info = new EditInfo() { Folder = AppData.EditBranchFolder };
                page.Select(info);
            }
       
        }
    }
}
