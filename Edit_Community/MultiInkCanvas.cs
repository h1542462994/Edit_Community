using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Edit_Community
{
    /// <summary>
    /// 支持多点触摸的InkCanvas
    /// </summary
    public class MultiTouchCanvas : FrameworkElement
    {
        internal InkCanvasProxy InkCanvas = new InkCanvasProxy();
        private Grid transparentOverlay = new Grid();
        private Border eraserbdr = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Width = 40,
            Height = 60,
            Background = Brushes.White,
            BorderThickness = new Thickness(2),
            BorderBrush = Brushes.Black

        };
        private StrokeType _strokeType = StrokeType.Stroke;
        private Dictionary<object, StrokeCollection> _strokes = new Dictionary<object, StrokeCollection>();
        private Dictionary<object, Stroke> _currentStroke = new Dictionary<object, Stroke>();

        private Color _penColor = Colors.Green;
        public Color PenColor
        {
            get
            {
                return this.InkCanvas.DefaultDrawingAttributes.Color;
            }
            set
            {
                this._penColor = value;
                Color color = value;
                if (this.PenType == StrokeType.Stroke)
                {
                    this.InkCanvas.DefaultDrawingAttributes.Color = color; 
                }
                else if (this.PenType == StrokeType.HighlighterStroke)
                {
                    color.ScA = 0.5f;
                    this.InkCanvas.DefaultDrawingAttributes.Color = color;
                }
            }
        }

        private double _penWidth = 4.0;
        public double PenWidth
        {
            get { return this._penWidth; }
            set
            {
                this._penWidth = value;
                if(this._strokeType == StrokeType.HighlighterStroke)
                {
                    this.InkCanvas.DefaultDrawingAttributes.Width = value;
                    this.InkCanvas.DefaultDrawingAttributes.Height = 5 * value;
                }
                else if(this._strokeType == StrokeType.Stroke)
                {
                    this.InkCanvas.DefaultDrawingAttributes.Width = value;
                    this.InkCanvas.DefaultDrawingAttributes.Height = value;
                }
            }
        }

        private double _eraseWidth = 30.0;
        public double EraseWidth
        {
            get
            {
                return this._eraseWidth;
            }
            set
            {
                this._eraseWidth = value;
                if (this._strokeType == StrokeType.EraseByPoint)
                {
                    this.InkCanvas.DefaultDrawingAttributes.Height = value;
                    this.eraserbdr.Width = value;
                    this.eraserbdr.Height = value;
                   this.InkCanvas.DefaultDrawingAttributes.Width = value;
                }
            }
        }

        public StrokeType PenType
        {
            get { return this._strokeType; }
            set
            {
                this._strokeType = value;
                if (this._strokeType == StrokeType.Stroke || this._strokeType == StrokeType.HighlighterStroke)
                {
                    this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    PenWidth = _penWidth;
                }
                else if (this._strokeType == StrokeType.EraseByPoint)
                {
                    this.InkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    EraseWidth = _eraseWidth;
                }
                this.PenColor = _penColor;
            }
        }

        public Brush Background
        {
            get { return this.InkCanvas.Background; }
            set
            {
                this.InkCanvas.Background = value;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 3; //grid + inkcanvas
            }
        }

        public MultiTouchCanvas()
        {
            base.IsManipulationEnabled = true;
            this.transparentOverlay.Background = Brushes.Transparent;
            base.IsEnabled = true;

            this.InitInkCanvasPropertys();
            this.PenType = StrokeType.Stroke;
        }

        public void ClearStrokes(object device)
        {
            if (this._strokes.ContainsKey(device) && this.InkCanvas.Strokes != null && this.InkCanvas.Strokes.Count > 0)
            {
                StrokeCollection sc = this._strokes[device];
                this.InkCanvas.Strokes.Remove(sc);
                this._strokes.Remove(device);
            }
        }

        public StrokeCollection GetStrokes(object device)
        {
            return this._strokes.ContainsKey(device) ? this._strokes[device] : null;
        }

        private void ShowEraserBdr(Point point)
        {
            this.eraserbdr.Margin = new Thickness(point.X - EraseWidth / 2, point.Y - EraseWidth / 2, 0, 0);
            this.eraserbdr.Visibility = Visibility.Visible;
        }
        private void HideEraserBdr()
        {
            this.eraserbdr.Visibility = Visibility.Hidden;
        }

        #region Event handle
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            TouchPoint tp = e.GetTouchPoint(this);
            if (this.InkCanvas.EditingMode == InkCanvasEditingMode.Ink)
            {
                this._startStroke(e.Device, tp.Position);
            }
            else
            {
                if (this.InkCanvas.EditingMode == InkCanvasEditingMode.EraseByPoint)
                {
                    ShowEraserBdr(tp.Position);

                    this._removeStroke(e.Device, tp.Position);
                }
            }

            e.Handled = true;
            base.Focusable = true;
            base.Focus();
            base.Focusable = false;
            e.TouchDevice.Capture(this);
        }

        protected override void OnPreviewTouchMove(TouchEventArgs e)
        {
            _handleTouchMove(e);
            if (this.InkCanvas.EditingMode == InkCanvasEditingMode.EraseByPoint)
            {
                ShowEraserBdr(e.GetTouchPoint(this).Position);
            }
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            e.TouchDevice.Capture(null); //
            HideEraserBdr();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (base.Visibility == System.Windows.Visibility.Visible)
            {
                if (this.InkCanvas.EditingMode == InkCanvasEditingMode.Ink)
                {
                    this._startStroke(e.Device, e.GetPosition(this));
                }
                else
                {
                    if (this.InkCanvas.EditingMode == InkCanvasEditingMode.EraseByPoint)
                    {
                        this._removeStroke(e.Device, e.GetPosition(this));
                    }
                }

                e.MouseDevice.Capture(this);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.InkCanvas.EditingMode == InkCanvasEditingMode.EraseByPoint)
                {
                    this._removeStroke(e.Device, e.GetPosition(this));
                    return;
                }
                if (this._strokes.ContainsKey(e.Device) && this._currentStroke.ContainsKey(e.Device))
                {
                    this._addPointToStroke(e.Device, e.GetPosition(this));
                }
                else
                {
                    this._startStroke(e.Device, e.GetPosition(this));
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            e.MouseDevice.Capture(null);
        }
        #endregion

        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0: return this.InkCanvas;
                case 1: return this.transparentOverlay;
                case 2:return this.eraserbdr;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.InkCanvas.Measure(availableSize);
            this.transparentOverlay.Measure(availableSize);
            return this.InkCanvas.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.InkCanvas.Arrange(new Rect(finalSize));
            this.transparentOverlay.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            base.AddVisualChild(this.InkCanvas);
            base.AddVisualChild(this.transparentOverlay);
        }
        private void _handleTouchMove(TouchEventArgs e)
        {
            if (this.InkCanvas.EditingMode == InkCanvasEditingMode.EraseByPoint)
            {
                this._removeStroke(e.Device, e.GetTouchPoint(this).Position);
                e.Handled = true;
                return;
            }

            if (this._strokes.ContainsKey(e.Device) && this._currentStroke.ContainsKey(e.Device))
            {
                Stroke stroke = this._currentStroke[e.Device];
                StylusPointCollection sps = stroke.StylusPoints;
                if (sps != null)
                {
                    TouchPoint tp = e.GetTouchPoint(this);
                    Point p = tp.Position;
                    this._addPointToStroke(e.Device, p);
                }
            }
            else
            {
                TouchPoint tp = e.GetTouchPoint(this);
                this._startStroke(e.Device, tp.Position);
            }
            e.Handled = true;
        }

        private void _addPointToStroke(object device, Point position)
        {
            Stroke stroke = this._currentStroke[device];
            if (stroke != null)
            {
                StylusPointCollection spc = stroke.StylusPoints;
                if (spc != null)
                {
                    spc.Add(new StylusPoint(position.X, position.Y, 0.5f));
                }
            }
        }

        private void _removeStroke(object device, Point position)
        {
            for (int i = 0; i < this.InkCanvas.Strokes.Count; i++)
            {
                Stroke stroke = this.InkCanvas.Strokes[i];
                Size strokesize = new Size(this.InkCanvas.DefaultDrawingAttributes.Width, this.InkCanvas.DefaultDrawingAttributes.Height);
                StrokeCollection sc = stroke.GetEraseResult(new Rect(position.X - 20, position.Y - 30, 40, 60));
                this.InkCanvas.Strokes.Replace(stroke, sc);
            }
        }

        private void _startStroke(object device, Point inputPosition)
        {
            StylusPointCollection stylusPointCollection = new StylusPointCollection();
            stylusPointCollection.Add(new StylusPoint(inputPosition.X, inputPosition.Y, 0.5f));

            if (stylusPointCollection.Count > 0)
            {
                Stroke stroke = new Stroke(stylusPointCollection);
                stroke.DrawingAttributes.Width = this.InkCanvas.DefaultDrawingAttributes.Width;
                stroke.DrawingAttributes.Height = this.InkCanvas.DefaultDrawingAttributes.Height;
                stroke.DrawingAttributes.Color = this.InkCanvas.DefaultDrawingAttributes.Color;
                this.InkCanvas.Strokes.Add(stroke);//添加到canvas
                if (this._currentStroke.ContainsKey(device))
                {
                    this._currentStroke.Remove(device);
                }
                this._currentStroke.Add(device, stroke);

                if (this._strokes.ContainsKey(device))
                {
                    this._strokes[device].Add(this._currentStroke[device]);
                    return;
                }

                this._strokes.Add(device, new StrokeCollection { this._currentStroke[device] });
            }
        }

        private void InitInkCanvasPropertys()
        {
            this.InkCanvas.Focusable = base.Focusable;
            this.InkCanvas.Background = Brushes.Transparent;
            this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            this.InkCanvas.UseCustomCursor = true;
        }

        public void ExitEditMode()
        {
            this.InkCanvas.Background = null;
            this.transparentOverlay.Background = null;
        }
        public void InEditMode()
        {
            this.InkCanvas.Background = Brushes.Transparent;
            this.transparentOverlay.Background = Brushes.Transparent;
        }
    }
    public enum StrokeType
    {
        Stroke,
        HighlighterStroke,
        EraseByPoint
    }
    public class InkCanvasProxy : InkCanvas
    {
        public InkCanvasProxy()
            : base()
        {
            // base.IsHitTestVisible = false;
            //  base.StylusPlugIns.Remove(base.DynamicRenderer);
        }
    }
}
