using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using System.Windows.Forms;
using System.Windows.Documents;
using StrategyManagerSolution.DiagramMisc;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class StartViewModel:ViewModelBase, IDragSource, IHasPosition, ISelectable
	{
		public StartView View { get;set; }
		private StartModel _startModel;
		public MoveAdorner MoveAdorner { get; set; }
		public Command LoadedCommand { get; }
		public event Action<ViewModelBase>? PositionChanged;
		public Point CanvasPos
		{
			get { return _startModel.CanvasPos; }
			set { _startModel.CanvasPos = value; }
		}

		public FrameworkElement DragSourceView => View;

		public ConnectionLine? LineLeaving { get; set; }
		public IDragDestination? LinkingTo { get; set; }

		public Point Offset { get; } = new Point(0, 0);

		public FrameworkElement PositionView => View;
		public ImageSource ImageSource { get; set; }
		public Brush BackgroundColor { get; set; } = Brushes.LightBlue;
		public bool IsSelected { get; set; } = false;
		public Command ClickCommand { get; }
		
		public StartViewModel(StartView startView, StartModel startModel)
		{
			View = startView;
			_startModel = startModel;
			MoveAdorner = new MoveAdorner(View,70,70);
			MoveAdorner.Drag += OnDrag;
			LoadedCommand = new(OnLoaded);
			ImageSource = new BitmapImage(new Uri("../../../Images/Start.jpg", UriKind.Relative));
			double a = ImageSource.Width;
			ClickCommand = new Command(OnSelect);
		}

		private void OnSelect(object? obj)
		{
			BackgroundColor = Brushes.Green;
			OnPropertyChanged(nameof(BackgroundColor));
			MouseButtonEventArgs e = (MouseButtonEventArgs)obj!;
			e.Handled = true;
			IsSelected = true;
		}

		private void OnDrag()
		{
			PositionChanged?.Invoke(this);
		}

		private void OnLoaded(object? obj)
		{
			SetUpAdorner();
		}
		public void OnDeselect()
		{
			IsSelected = false;
			BackgroundColor = Brushes.LightBlue;
			OnPropertyChanged(nameof(BackgroundColor));
		}
		public void SetUpAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(View);
			if (adornerLayer == null)
			{
				Console.WriteLine("Failed to get adorner layer!");
			}
			else
			{
				adornerLayer.Add(MoveAdorner);
			}
		}

		public void OnLineLeavingDestroyed(ConnectionLine line)
		{
			LineLeaving = null;
			LinkingTo = null;
		}
	}
}
