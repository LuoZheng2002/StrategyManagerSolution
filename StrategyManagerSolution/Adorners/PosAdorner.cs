using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.Adorners
{
    internal class PosAdorner: Adorner
    {
        VisualCollection AdornerVisuals { get; }
        static readonly int WIDTH = 20;
        static readonly int HEIGHT = 10;
        private readonly Image _upperImage;
        private readonly Image _lowerImage;
        public event Action? UpperClicked;
        public event Action? LowerClicked;
        public ImageSource UpperImageSource { get; } = new BitmapImage(new Uri("../../../Images/upper.jpg", UriKind.Relative));
		public ImageSource LowerImageSource { get; } = new BitmapImage(new Uri("../../../Images/lower.jpg", UriKind.Relative));
		public PosAdorner(UIElement adornedElement): base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            double a = UpperImageSource.Width;
            double b = LowerImageSource.Width;
            _upperImage = new Image { Width = WIDTH, Height = HEIGHT, Source = UpperImageSource };
            _lowerImage = new Image { Width = WIDTH, Height = HEIGHT, Source= LowerImageSource };
            _upperImage.MouseDown += (_, _) => UpperClicked?.Invoke();
            _lowerImage.MouseDown += (_, _) => LowerClicked?.Invoke();
            AdornerVisuals.Add(_upperImage);
            AdornerVisuals.Add(_lowerImage);
        }
		protected override Visual GetVisualChild(int index)
		{
            return AdornerVisuals[index];
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
            _upperImage.Arrange(new Rect(AdornedElement.RenderSize.Width - WIDTH - 5, AdornedElement.RenderSize.Height / 2 - 12.5, WIDTH, HEIGHT));
            _lowerImage.Arrange(new Rect(AdornedElement.RenderSize.Width - WIDTH - 5, AdornedElement.RenderSize.Height / 2 + 2.5, WIDTH, HEIGHT));
			return base.ArrangeOverride(finalSize);
		}
		protected override int VisualChildrenCount => AdornerVisuals.Count;
	}
}
