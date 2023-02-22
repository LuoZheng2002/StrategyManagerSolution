using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class StrategyViewModel : ViewModelBase, ISelectable, IDragSource
	{
		public StrategyView View { get; set; }
		private StrategyModel _strategyModel;
		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (_isSelected)
					{
						Background = Brushes.LightGreen;
					}
					else
					{
						Background = Brushes.AliceBlue;
					}
					OnPropertyChanged(nameof(Background));
				}
			}
		}
		private string _text = "";
		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					OnPropertyChanged(nameof(Text));
				}
			}
		}
		private Brush _background = Brushes.AliceBlue;
		public Brush Background
		{
			get { return _background; }
			set
			{
				_background = value;
				OnPropertyChanged(nameof(Background));
			}
		}
		public Command DropCommand { get; }
		public Command SelectCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		public Command LoadedCommand { get; }
		private readonly NodeAdorner _nodeAdorner;
		public TextBlock TextBlock { get; }
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
					_strategyModel.LinkingTo = null;
				}
				else
				{
					_linkingTo = value;
					_strategyModel.LinkingTo = _linkingTo.DestinationModel;
				}
			}
		}
		public Point Offset { get; } = new Point(0,0);

		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<ViewModelBase>? PositionChanged;
		public StrategyViewModel(StrategyView view, StrategyModel strategyModel)
		{
			View = view;
			_strategyModel = strategyModel;
			TextBlock = View.TextBlock;
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			MouseEnterCommand= new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			LoadedCommand = new Command(OnLoaded);
			_nodeAdorner = new NodeAdorner(TextBlock);
			_nodeAdorner.MouseLeave += OnMouseLeaveNodeAdorner;
			_nodeAdorner.DragStarted += OnDragStarted;
		}

		

		void OnDragStarted(FrameworkElement dragSource)
		{
			DragStarted?.Invoke(this);
		}
		private void OnMouseLeaveNodeAdorner(object sender, MouseEventArgs e)
		{
			if (!MouseInTextBlock(e))
			{
				HideNodeAdorner();
			}
		}

		public event Action<StrategyViewModel, object>? Dropped;//second arg: dropeventargs
		public event Action<object?>? DeleteChild;
		public void OnDrop(object? obj)
		{
			Dropped?.Invoke(this, obj);
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
		}
		public void OnSelect(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			IsSelected = true;
		}
		public void OnCanvasClicked()
		{
			CanvasClicked?.Invoke();
			OnDeselect(null);
		}
		public void OnKeyDown(KeyEventArgs e)
		{
			KeyDown?.Invoke(e);
			if (e.Key == Key.Delete && IsSelected)
			{
				DeleteChild?.Invoke(this);
			}
		}
		void OnMouseEnter(object? obj)
		{
			ShowNodeAdorner();
		}
		void OnMouseLeave(object? obj)
		{
			MouseEventArgs e = (obj as MouseEventArgs)!;
			if (!MouseInTextBlock(e))
			{
				HideNodeAdorner();
			}
		}
		bool MouseInTextBlock(MouseEventArgs e)
		{
			Point pos = e.GetPosition(TextBlock);
			return (pos.X >= 0 && pos.Y >= 0 && pos.X <= TextBlock.Width && pos.Y <= TextBlock.Height);
		}
		void ShowNodeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			Adorner[] adorners = adornerLayer.GetAdorners(TextBlock);
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
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			Adorner[] adorners = adornerLayer.GetAdorners(TextBlock);
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
		~StrategyViewModel()
		{
			Console.WriteLine("Strategy Destructed!");
		}
		public void OnLoaded(object? obj)
		{
			// historical issues
		}

		public void OnLineLeavingDestroyed(ConnectionLine line)
		{
			LineLeaving = null;
			LinkingTo = null;
		}
		public void OnPositionChanged(ViewModelBase viewModelBase)
		{
			PositionChanged?.Invoke(this);
		}
	}
}
