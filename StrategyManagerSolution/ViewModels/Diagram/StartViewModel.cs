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
// using System.Windows.Forms;
using System.Windows.Documents;
using StrategyManagerSolution.DiagramMisc;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class StartViewModel:ViewModelBase, IDragSource, IDiagramItem, ISelectable
	{
		public StartView View { get;set; }
		private StartModel _startModel;
		//接口
		public UserControl ViewRef => View;

		public DiagramElementModel ModelRef => _startModel;

		private NodeAdorner _nodeAdorner;
		public MoveAdorner MoveAdorner { get; set; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		public event Action<ViewModelBase>? PositionChanged;
		public Point CanvasPos
		{
			get { return _startModel.CanvasPos; }
			set { _startModel.CanvasPos = value; }
		}
		public DiagramElementModel? ModelLinkingTo
		{
			get { return _startModel.LinkingTo; }
			set { _startModel.LinkingTo = value; }
		}
		public FrameworkElement DragSourceView => View;

		public ConnectionLine? LineLeaving { get; set; }
		private IDragDestination? _linkingTo;
		public IDragDestination? LinkingTo
		{
			get { return _linkingTo; }
			set
			{
				if (value == null)
				{
					_linkingTo = null;
					_startModel.LinkingTo = null;
				}
				else
				{
					_linkingTo = value;
					_startModel.LinkingTo = _linkingTo.DestinationModel;
				}
			}
		}

		public Point Offset { get; } = new Point(160, 35);

		public FrameworkElement PositionView => View;
		public Grid Grid => View.Grid;
		public ImageSource ImageSource { get; set; }
		public Brush BackgroundColor { get; set; } = Brushes.LightBlue;
		public bool IsSelected { get; set; } = false;
		public Command ClickCommand { get; }

		public event Action<ViewModelBase>? Destroy;
		public event Action<IDragSource>? DragStarted;
		public StartViewModel(StartView startView, StartModel startModel)
		{
			View = startView;
			_startModel = startModel;
			MoveAdorner = new MoveAdorner(View, 10, 10, 50,50);
			MoveAdorner.Drag += OnDrag;
			
			ImageSource = new BitmapImage(new Uri("../../../Images/Start.jpg", UriKind.Relative));
			double a = ImageSource.Width;
			ClickCommand = new Command(OnSelect);
			LoadedCommand = new(OnLoaded);
			MouseEnterCommand = new(OnMouseEnter);
			MouseLeaveCommand = new(OnMouseLeave);
			_nodeAdorner = new NodeAdorner(Grid);
			_nodeAdorner.MouseLeave += OnMouseLeaveNodeAdorner;
			_nodeAdorner.DragStarted += OnDragStarted;
		}

		private void OnDragStarted(FrameworkElement obj)
		{
			DragStarted?.Invoke(this);
		}

		private void OnMouseLeaveNodeAdorner(object sender, MouseEventArgs e)
		{
			if (!MouseInView(e))
			{
				HideNodeAdorner();
			}
		}

		private bool MouseInView(MouseEventArgs e)
		{
			Point pos = e.GetPosition(Grid);
			return (pos.X >= 0 && pos.Y >= 0 && pos.X <= Grid.ActualWidth && pos.Y <= Grid.ActualHeight);
		}

		void ShowNodeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Grid);
			Adorner[] adorners = adornerLayer.GetAdorners(Grid);
			if (adorners == null || !adorners.Contains(_nodeAdorner))
			{
				adornerLayer.Add(_nodeAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to add another node adorner.");
			}
		}
		void HideNodeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Grid);
			Adorner[] adorners = adornerLayer.GetAdorners(Grid);
			if (adorners == null)
			{
				Console.WriteLine("There is no adorners.");
			}
			else if (adorners.Contains(_nodeAdorner))
			{
				adornerLayer.Remove(_nodeAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to remove node adorner twice.");
			}
		}
		private void OnSelect(object? obj)
		{
			BackgroundColor = Brushes.Green;
			OnPropertyChanged(nameof(BackgroundColor));
			MouseButtonEventArgs e = (MouseButtonEventArgs)obj!;
			e.Handled = true;
			IsSelected = true;
		}
		public void OnKeyDown(KeyEventArgs e)
		{
			if (IsSelected && e.Key == Key.Delete)
			{
				Destroy?.Invoke(this);
				IsSelected = false;
			}
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

		void OnMouseEnter(object? obj)
		{
			ShowNodeAdorner();
		}
		void OnMouseLeave(object? obj)
		{
			MouseEventArgs e = (obj as MouseEventArgs)!;
			if (!MouseInView(e))
			{
				HideNodeAdorner();
			}
		}
	}
}
