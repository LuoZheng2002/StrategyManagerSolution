using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Utils;
using StrategyManagerSolution.ViewModels.Form;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
		private readonly NodeAdorner _nodeAdorner;
		private readonly PosAdorner _posAdorner;
		public StrategyView View { get;}
		private StrategyModel _strategyModel;
		public StrategyModel StrategyModel=>_strategyModel;
		public bool IsSelected { get; set; } = false;
		public string Text
		{
			get { return StrategyModel.StrategyName; }
			set { StrategyModel.StrategyName = value; }
		}
		public Brush Background { get; set; } = Brushes.AliceBlue;
		public Command DropCommand { get; }
		public Command MouseDownCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		
		public TextBlock TextBlock => View.TextBlock;
		public FrameworkElement DragSourceView => View;
		public ConnectionLine? LineLeaving { get; set; }
		public IDragDestination? LinkingTo { get; set; }
		public DiagramElementModel? ModelLinkingTo
		{
			get { return _strategyModel.LinkingTo; }
			set { _strategyModel.LinkingTo = value; }
		}
		public Point Offset { get; } = new Point(200,15);

		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<ViewModelBase>? PositionChanged;
		public StrategyViewModel(StrategyView view, StrategyModel strategyModel)
		{
			View = view;
			_strategyModel = strategyModel;
			DropCommand = new Command(OnDrop);
			MouseDownCommand = new Command(OnMouseDown);
			MouseEnterCommand= new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			_nodeAdorner = new NodeAdorner(TextBlock);
			_nodeAdorner.MouseLeave += OnMouseLeaveAdorner;
			_nodeAdorner.DragStarted += OnDragStarted;
			_posAdorner = new PosAdorner(TextBlock);
			_posAdorner.MouseLeave += OnMouseLeaveAdorner;
			_posAdorner.UpperClicked += OnUpperClicked;
			_posAdorner.LowerClicked += OnLowerClicked;
		}

		

		void OnDragStarted(FrameworkElement dragSource)
		{
			DragStarted?.Invoke(this);
		}
		private void OnMouseLeaveAdorner(object sender, MouseEventArgs e)
		{
			if (!MouseInTextBlock(e))
			{
				HideAdorners();
			}
		}

		public event Action<StrategyViewModel, object>? Dropped;//second arg: dropeventargs
		public event Action<StrategyViewModel>? Destroy;
		public event Action<StrategyViewModel>? UpperClicked;
		public event Action<StrategyViewModel>? LowerClicked;
		public event Action<ViewModelBase>? OpenScript;
		public void OnDrop(object? obj)
		{
			Dropped?.Invoke(this, obj!);
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
			Background = Brushes.AliceBlue;
			OnPropertyChanged(nameof(Background));
		}
		private void OnDoubleClick()
		{
			PopupWindow popupWindow = new PopupWindow();
			StrategyConfigViewModel strategyConfigViewModel = new StrategyConfigViewModel();
			strategyConfigViewModel.StrategyName = _strategyModel.StrategyName;
			strategyConfigViewModel.StrategyModelClassName = _strategyModel.StrategyClassName;
			strategyConfigViewModel.OpenScript += OnOpenScript;
			popupWindow.DataContext = new PopupViewModel(popupWindow, strategyConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}
			_strategyModel.StrategyName = strategyConfigViewModel.StrategyName;
			_strategyModel.StrategyClassName = strategyConfigViewModel.StrategyModelClassName;
			OnPropertyChanged(nameof(Text));
		}
		private void OnOpenScript()
		{
			OpenScript?.Invoke(this);
		}
		public void OnMouseDown(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			if (e.ClickCount == 2)
			{
				OnDoubleClick();
				return;
			}
			e.Handled = true;
			IsSelected = true;
			Background = Brushes.LightGreen;
			OnPropertyChanged(nameof(Background));
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
				Destroy?.Invoke(this);
				IsSelected = false;
			}
		}
		void OnMouseEnter(object? obj)
		{
			ShowAdorners();
		}
		void OnMouseLeave(object? obj)
		{
			MouseEventArgs e = (obj as MouseEventArgs)!;
			if (!MouseInTextBlock(e))
			{
				HideAdorners();
			}
		}
		bool MouseInTextBlock(MouseEventArgs e)
		{
			Point pos = e.GetPosition(TextBlock);
			return (pos.X >= 0 && pos.Y >= 0 && pos.X <= TextBlock.ActualWidth && pos.Y <= TextBlock.ActualHeight);
		}
		void ShowAdorners()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextBlock);
			Adorner[] adorners = adornerLayer.GetAdorners(TextBlock);
			if (adorners == null || !adorners.Contains(_nodeAdorner))
			{
				adornerLayer.Add(_nodeAdorner);
				adornerLayer.Add(_posAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to add another node adorner.");
			}
		}
		void HideAdorners()
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
				adornerLayer.Remove(_posAdorner);
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

		public void OnLineLeavingDestroyed(ConnectionLine line)
		{
			LineLeaving = null;
			LinkingTo = null;
		}
		public void OnPositionChanged(ViewModelBase viewModelBase)
		{
			PositionChanged?.Invoke(this);
		}
		private void OnUpperClicked()
		{
			UpperClicked?.Invoke(this);
		}
		private void OnLowerClicked() 
		{
			LowerClicked?.Invoke(this);
		}
	}
}
