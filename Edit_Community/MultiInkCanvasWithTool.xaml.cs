using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using User;
using User.SoftWare;
using User.UI;

namespace Edit_Community
{
    /// <summary>
    /// MultiInkCanvasWithTool.xaml 的交互逻辑
    /// </summary>
    public partial class MultiInkCanvasWithTool : UserControl
    {
        const double penwidthmin = 2;
        const double penwidthmax = 12;
        bool isPenwidthSlideMouseDown;
        bool isEraseSlideMouseDown;
        internal TriggerImage[] ImgInkMenu = new TriggerImage[4];
        internal Ellipse[] ElpInkColor = new Ellipse[25];
        public SaveBrushEventHander SaveBrushCallBack;
        private int inkColorIndex = 0;
        private USettingsProperty<int> inkColorIndexProperty;
        private double penwidth = 4;
        private USettingsProperty<double> penwidthProperty;
        private int inkMenuSelectindex = 1;
        public bool IsTransparentStyle
        {
            get { return (bool)GetValue(IsTransparentStyleProperty); }
            set { SetValue(IsTransparentStyleProperty, value); }
        }
        public static readonly DependencyProperty IsTransparentStyleProperty =
            DependencyProperty.Register("IaTransparentStyle", typeof(bool), typeof(MultiInkCanvasWithTool), new PropertyMetadata(true, new PropertyChangedCallback(IsTransparentChanged)));
        private static void IsTransparentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiInkCanvasWithTool arg = (MultiInkCanvasWithTool)d;
            if ((bool)e.NewValue)
            {
                arg.GridBack.Background = Brushes.Transparent;
                arg.GridVisual.Background = Brushes.Transparent;
                arg.BdrBack0.Visibility = Visibility.Hidden;
                arg.BdrBack1.Visibility = Visibility.Hidden;
                arg.BdrBack2.Visibility = Visibility.Hidden;
                arg.BdrBack3.Visibility = Visibility.Hidden;
                arg.GridEditInkMenuBox1.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
                arg.GridEditInkMenuBox3.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
                arg.ElpPenwidth.Fill = Brushes.Black;
                arg.BdISM.Background = new SolidColorBrush(Color.FromArgb(255, 75, 75, 75));
            }
            else
            {
                arg.GridBack.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
                arg.GridVisual.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
                arg.BdrBack0.Visibility = Visibility.Visible;
                arg.BdrBack1.Visibility = Visibility.Visible;
                arg.BdrBack2.Visibility = Visibility.Visible;
                arg.BdrBack3.Visibility = Visibility.Visible;
                arg.GridEditInkMenuBox1.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
                arg.GridEditInkMenuBox3.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
                arg.ElpPenwidth.Fill = Brushes.White;
                arg.BdISM.Background = Brushes.White;
            }
        }

        private readonly Color[] InkColorDefault = new Color[]
{
            Colors.Black,Colors.DarkGray,Colors.Gray,Colors.LightGray,Colors.White,
            Colors.Red,Colors.OrangeRed,Colors.Orange,Colors.Gold,Colors.Yellow,
            Colors.LawnGreen,Colors.Green,Colors.SeaGreen,Colors.DeepSkyBlue,Colors.Blue,
            Colors.Tomato,Colors.Violet,Colors.LightYellow,Colors.LightGreen,Colors.LightBlue,
            Colors.Pink,Colors.RosyBrown,Colors.Chocolate,Colors.Brown,Colors.Purple,
};
        public int InkMenuSelectIndex
        {
            get => inkMenuSelectindex;
            set
            {
                inkMenuSelectindex = value;
                for (int i = 0; i < ImgInkMenu.Length; i++)
                {
                    if (i == value)
                    {
                        ImgInkMenu[i].IsChecked = true;
                    }
                    else
                    {
                        ImgInkMenu[i].IsChecked = false;
                    }
                }
                if (value == 0)
                {
                    EditICs.Visibility = Visibility.Hidden;
                    GridVisual.Visibility = Visibility.Hidden;
                }
                else
                {
                    EditICs.Visibility = Visibility.Visible;
                    GridVisual.Visibility = Visibility.Visible;
                }
                if (value == 1)
                {
                    EditICs.PenType = StrokeType.Stroke;
                }
                else if (value == 2)
                {
                    EditICs.PenType = StrokeType.HighlighterStroke;
                }
                else if (value == 3)
                {
                    EditICs.PenType = StrokeType.EraseByPoint;
                }
            }
        }
        public int InkColorIndex
        {
            get
            {
                if (inkColorIndexProperty == null)
                {
                    return inkColorIndex;
                }
                else
                {
                    return inkColorIndexProperty.Value;
                }
            }
            set
            {
                if (inkColorIndexProperty == null)
                {
                    inkColorIndex = value;
                }
                else
                {
                    inkColorIndexProperty.Value = value;
                }
                int arg = value;
                int row = arg / 5;
                int column = arg % 5;
                ElpInkColorSelect.Margin = new Thickness(40 * column + 5, 30 * row, 0, 0);
                Color color = InkColorDefault[arg];
                EditICs.PenColor = InkColorDefault[arg];
            }
        }
        public double Penwidth
        {
            get
            {
                if (penwidthProperty == null)
                {
                    return penwidth;
                }
                else
                {
                    return penwidthProperty.Value;
                }
            }
            set
            {
                if (penwidthProperty == null)
                {
                    penwidth = value;
                }
                else
                {
                    penwidthProperty.Value = value;
                }
                double bi = (value - penwidthmin) / (penwidthmax - penwidthmin);
                this.BdISM.Margin = new Thickness(7.5 + bi * 120, 2.5, 0, 2.5);
                this.ElpPenwidth.Width = (int)value;
                this.ElpPenwidth.Height = (int)value;
                this.LblPenwidth.Content = (int)value;
                this.EditICs.PenWidth = (int)value;
            }
        }

        public MultiInkCanvasWithTool()
        {
            InitializeComponent();
            GridInkColor_InitilizeComponent();
            for (int i = 0; i < ImgInkMenu.Length; i++)
            {
                string displayname = "ImgInkMenu" + i;
                ImgInkMenu[i] = (TriggerImage)FindName(displayname);
                ImgInkMenu[i].MouseUp += ImgInkMenu_MouseUp;
            }
            InkColorIndex = 0;
            Penwidth = 4;
            InkMenuSelectIndex = 1;
        }
        private void GridInkColor_InitilizeComponent()
        {
            int column = 0;
            int row = 0;
            for (row = 0; row < 5; row++)
            {
                for (column = 0; column < 5; column++)
                {
                    int index = 5 * row + column;
                    Ellipse ellipse = new Ellipse()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(40 * column + 5, 30 * row, 0, 0),
                        Width = 30,
                        Height = 30
                    };
                    ElpInkColor[index] = ellipse;
                    ElpInkColor[index].MouseUp += BdrInkColor_MouseUp;
                    GridInkColorBox.Children.Add(ellipse);
                }
            }
            for (int i = 0; i < ElpInkColor.Length; i++)
            {
                ElpInkColor[i].Fill = new SolidColorBrush(InkColorDefault[i]);
            }
        }
        private void ImgInkMenu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < ImgInkMenu.Length; i++)
            {
                if (sender.Equals(ImgInkMenu[i]))
                {

                    if (i == 1 || i == 2)
                    {
                        if (i == InkMenuSelectIndex)
                        {
                            if (GridEditInkMenuBox1.Visibility == Visibility.Visible)
                                GridEditInkMenuBox1.Visibility = Visibility.Hidden;
                            else
                                GridEditInkMenuBox1.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            GridEditInkMenuBox1.Visibility = Visibility.Hidden;
                        }
                        GridEditInkMenuBox3.Visibility = Visibility.Hidden;
                    }
                    else if (i == 0)
                    {
                        GridEditInkMenuBox1.Visibility = Visibility.Hidden;
                        GridEditInkMenuBox3.Visibility = Visibility.Hidden;
                    }
                    else if (i == 3)
                    {
                        if (i == InkMenuSelectIndex)
                        {
                            if (GridEditInkMenuBox3.Visibility == Visibility.Visible)
                                GridEditInkMenuBox3.Visibility = Visibility.Hidden;
                            else
                                GridEditInkMenuBox3.Visibility = Visibility.Visible;
                        }
                        GridEditInkMenuBox1.Visibility = Visibility.Hidden;
                    }
                    InkMenuSelectIndex = i;
                }
            }
        }
        private void BdrInkColor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < ElpInkColor.Length; i++)
            {
                if (sender.Equals(ElpInkColor[i]))
                {
                    InkColorIndex = i;
                    break;
                }
            }
        }

        private void BorderInkSize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPenwidthSlideMouseDown)
            {
                double v = Tools.GetSlideValue(this.BorderInkSize, 10);

                this.Penwidth = penwidthmin + v * (penwidthmax - penwidthmin);
            }
        }
        private void BorderInkSize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isPenwidthSlideMouseDown = true;
            double v = Tools.GetSlideValue(this.BorderInkSize, 10);
            this.Penwidth = penwidthmin + v * (penwidthmax - penwidthmin);
            this.GridBack.Visibility = Visibility.Visible;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isPenwidthSlideMouseDown = false;
            isEraseSlideMouseDown = false;
            BdEM.Margin = new Thickness(-15, 0, 0, 0);
            GridBack.Visibility = Visibility.Hidden;
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isPenwidthSlideMouseDown = false;
            if (isEraseSlideMouseDown)
            {
                if (e.GetPosition(BdrEraser).X >= 180)
                {
                    EditICs.InkCanvas.Strokes = new StrokeCollection();
                }
                BdEM.Margin = new Thickness(-15, 0, 0, 0);
            }
            isEraseSlideMouseDown = false;
            GridBack.Visibility = Visibility.Hidden;
        }
        public void Load(StrokeCollection value)
        {
            this.EditICs.InkCanvas.Strokes = value;
        }
        public void SetPropertys(USettingsProperty<int> inkColorIndexProperty, USettingsProperty<double> penwidthProperty)
        {
            this.inkColorIndexProperty = inkColorIndexProperty;
            this.penwidthProperty = penwidthProperty;
        }
        public void LoadPropertys()
        {
            if (inkColorIndexProperty != null)
                InkColorIndex = inkColorIndexProperty.Value;
            if (penwidthProperty != null)
                Penwidth = penwidthProperty.Value;
        }
        private void BdrEraser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(BdrEraser);
            if (point.X >= 5 && point.X <= 35)
            {
                if (point.X >= 20)
                {
                    BdEM.Margin = new Thickness(point.X - 35, 0, 0, 0);
                }
                isEraseSlideMouseDown = true;
                GridBack.Visibility = Visibility.Visible;
            }
        }
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isEraseSlideMouseDown)
            {
                Point point = e.GetPosition(BdrEraser);
                if (point.X < 20)
                {
                    point.X = 20;
                }
                else if (point.X > 180)
                {
                    point.X = 180;
                }
                BdEM.Margin = new Thickness(point.X - 35, 0, 0, 0);
            }
        }

        private void EditICs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Stroke_MouseDown();
        }
        private void EditICs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Stroke_MouseUp();
        }
        private void EditICs_TouchUp(object sender, TouchEventArgs e)
        {
            Stroke_MouseUp();
        }
        private void EditICs_TouchEnter(object sender, TouchEventArgs e)
        {
            Stroke_MouseDown();
        }
        private void EditICs_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (InkMenuSelectIndex == 3 && e.LeftButton == MouseButtonState.Pressed)
            {
                Stroke_MouseMove(e.GetPosition(EditICs));
            }
        }

        void Stroke_MouseDown()
        {
            GridEditInkMenuBox1.Visibility = Visibility.Hidden;
            GridEditInkMenuBox3.Visibility = Visibility.Hidden;
        }
        void Stroke_MouseMove(Point point)
        {
            ImgEraser.Visibility = Visibility.Visible;
            ImgEraser.Margin = new Thickness(point.X - 20, point.Y -30, 0, 0);
        }
        void Stroke_MouseUp()
        {
            SaveBrushCallBack?.Invoke(this.EditICs.InkCanvas.Strokes);
            ImgEraser.Visibility = Visibility.Collapsed;
        }

        private void EditICs_TouchMove(object sender, TouchEventArgs e)
        {
            if (InkMenuSelectIndex == 3)
            {
                Stroke_MouseMove(e.GetTouchPoint(EditICs).Position);
            }
        }
    }
    public delegate void SaveBrushEventHander(StrokeCollection value);
}
