using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StrategyManagerSolution.Adorners
{
    internal class MoveAdorner : Adorner
    {
        VisualCollection AdornerVisuals { get; }
        public Thumb thumb;
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public event Action? Drag;
        public MoveAdorner(UIElement adornedElement, int width, int height) : base(adornedElement)
        {
            ThumbWidth = width;
            ThumbHeight = height;
            AdornerVisuals = new VisualCollection(this);
            thumb = new Thumb() { Background = Brushes.Transparent, Height = ThumbHeight, Width = ThumbWidth };
            thumb.DragDelta += Thumb_DragDelta;
            AdornerVisuals.Add(thumb);
            AdornerStyle adornerStyle = new AdornerStyle();
            thumb.Style = adornerStyle.FindResource("Transparent") as Style;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
            Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
            Drag?.Invoke();
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            thumb.Arrange(new Rect(0, 0, ThumbWidth, ThumbHeight));
            return base.ArrangeOverride(finalSize);
        }
        protected override int VisualChildrenCount => AdornerVisuals.Count;
    }
}

