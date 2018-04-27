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
    /// EditItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditItemView : UserControl
    {
        public EditItemView()
        {
            InitializeComponent();
        }

        public EditInfo EditInfo
        {
            get { return (EditInfo)GetValue(EditInfoProperty); }
            set { SetValue(EditInfoProperty, value); }
        }
        public static readonly DependencyProperty EditInfoProperty =
            DependencyProperty.Register("EditInfo", typeof(EditInfo), typeof(EditItemView), new PropertyMetadata(new EditInfo(), new PropertyChangedCallback(EditInfo_Changed)));
        void OnEditInfoChanged()
        {

            #region EditItemType
            EditItemType editItemType = EditInfo.EditType;
            if (editItemType == EditItemType.Reserved)
            {
                ElpMarkup.Fill = Brushes.Gray;
                TbxMark.Text = "保留";
            }
            else if (editItemType == EditItemType.Note)
            {
                ElpMarkup.Fill = Brushes.Violet;
                TbxMark.Text = "笔记";
            }
            else if (editItemType == EditItemType.Daily)
            {
                ElpMarkup.Fill = Brushes.White;
                TbxMark.Text = "日常";
            }
            else if (editItemType == EditItemType.Holiday)
            {
                ElpMarkup.Fill = Brushes.Orange;
                TbxMark.Text = "假期";
            }
            else if (editItemType == EditItemType.Summervacation)
            {
                ElpMarkup.Fill = Brushes.SeaGreen;
                TbxMark.Text = "暑假";
            }
            else if (editItemType == EditItemType.Wintervacation)
            {
                ElpMarkup.Fill = Brushes.SkyBlue;
                TbxMark.Text = "寒假";
            }
            else if (editItemType == EditItemType.Hugeevent)
            {
                ElpMarkup.Fill = Brushes.OrangeRed;
                TbxMark.Text = "重要";
            }

            if (editItemType == EditItemType.Reserved)
            {
                TbxTitle.IsEnabled = false;
            }
            else
            {
                TbxTitle.IsEnabled = true;
            }
            #endregion
            if (EditInfo.Title == null)
            {
                TbxTitle.Text = "";
            }
            else
            {
                TbxTitle.Text = EditInfo.Title;
            }
            TbxTime.Text = EditInfo.CreateTime.ToString();
        }
        private static void EditInfo_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditItemView)d).OnEditInfoChanged();
        }

        public static async void LoadContent()
        {
          
        }
    }

    public enum EditItemType
    {
        Reserved = -1,
        Note = 2,
        Daily = 0,
        Holiday = 5,
        Summervacation = 6,
        Wintervacation = 7,
        Hugeevent = 8
    }
}
