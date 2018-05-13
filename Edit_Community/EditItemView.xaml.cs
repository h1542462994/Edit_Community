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
using User.UI;

namespace Edit_Community
{
    /// <summary>
    /// EditItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditItemView : UControl
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
                TbxTitle.IsReadOnly = true;
            }
            else
            {
                TbxTitle.IsReadOnly = false;
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
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(EditItemView), new PropertyMetadata(0,new PropertyChangedCallback(Index_Changed)));
        private static void Index_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditItemView)d).TbxIndex.Text = e.NewValue.ToString();
        }
        public bool IsContentOpened
        {
            get { return (bool)GetValue(IsContentOpenedProperty); }
            set { SetValue(IsContentOpenedProperty, value); }
        }
        public static readonly DependencyProperty IsContentOpenedProperty =
            DependencyProperty.Register("IsContentOpened", typeof(bool), typeof(EditItemView), new PropertyMetadata(false,new PropertyChangedCallback(IsContentOpened_Changed)));
        private static void IsContentOpened_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditItemView view = (EditItemView)d;
            if ((bool)e.NewValue)
            {
                view.Height = double.NaN;
            }
            else
            {
                view.Height = 100;
            }
            view.TriggerImageSlide.IsChecked = (bool)e.NewValue;
        }

        public async void LoadContent()
        {
            string result = "";
            for (int i = 0; i < 6; i++)
            {
                string path = EditInfo.Folder + i + ".rtf";
                result += i + ":" + await LoadRtfFile(path);
            }
            TbxContent.Text = result;
        }

        public async Task<string> LoadRtfFile(string path)
        {
            string result = null;
            await  Dispatcher.InvokeAsync(() =>
            {
                RichTextBox rtx = new RichTextBox();
                TextRange tr = new TextRange(rtx.Document.ContentStart, rtx.Document.ContentEnd);
                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    tr.Load(stream, DataFormats.Rtf);
                }
                result = tr.Text;
            });
            return result;
        }

        public event RoutedEventHandler SlideTapped;

        private void TriggerImageSlide_Tapped(object sender, RoutedEventArgs e)
        {
            SlideTapped?.Invoke(this, e);
        }
    }

    public enum EditItemType
    {
        Reserved = -1,
        Daily = 0,
        Note = 1,
        Holiday = 2,
        Summervacation = 3,
        Wintervacation = 4,
        Hugeevent = 5
    }
}
