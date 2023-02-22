using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.Views.Diagram;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.ViewModels.Diagram;
using Contracts.MVVMModels;

namespace StrategyManagerSolution.ViewModels
{
	internal class StrategySetViewModel : ViewModelBase, ISelectable, IDragDestination, IDiagramItem
	{
		public MoveAdorner MoveAdorner { get; set; }
		public StrategySetView View { get; set; }
		private StrategySetModel _strategySetModel;
		//接口
		public UserControl ViewRef => View;
		public DiagramElementModel ModelRef => _strategySetModel;
		public DiagramElementModel DestinationModel => _strategySetModel;
		public ConnectionLine? Line { get; set; }
		public bool DraggingLine { get; set; }
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
						TextColor = Brushes.LightGreen;
					}
					else
					{
						TextColor = Brushes.AliceBlue;
					}
					OnPropertyChanged(nameof(TextColor));
				}
			}
		}
		private int number = 0;
		public ImageSource? Image { get; set; } = new BitmapImage(new Uri("../Images/114514.jpeg", UriKind.Relative));
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;
		
		private string _imageName = "";
		public string ImageName
		{
			get { return _imageName; }
			set
			{
				if (_imageName != value)
				{
					_imageName = value;
					Image = new BitmapImage(new Uri(_imageName, UriKind.Relative));
					double a = Image.Width;
					OnPropertyChanged(nameof(Image));
				}
			}
		}
		public string Text { get; set; } = "";
		public ObservableCollection<StrategyView> StrategyViews { get; } = new();
		public List<StrategyViewModel> StrategyViewModels { get; } = new();
		public Command DropCommand { get; }
		public Command SelectCommand { get; }
		public Command MouseLeftButtonUpCommand { get; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }

		public FrameworkElement DragDestinationView => View;

		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set; }
		public Point Offset { get; } = new Point(0, 0);
		public Point CanvasPos
		{
			get { return _strategySetModel.CanvasPos; }
			set { _strategySetModel.CanvasPos = value; }
		}

		public FrameworkElement PositionView => throw new NotImplementedException();

		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<IDragDestination>? DragEnded;
		public event Action<ViewModelBase>? PositionChanged;
		public event Action<ViewModelBase>? Destroy;
		public StrategySetViewModel(StrategySetView view, StrategySetModel strategySetModel)
		{
			View = view;
			_strategySetModel = strategySetModel;
			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			MouseLeftButtonUpCommand = new Command(OnMouseLeftButtonUp);
			LoadedCommand = new Command(OnLoaded);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			
			MoveAdorner = new MoveAdorner(view, 30, 30);
			MoveAdorner.Drag += OnDrag;
		}

		private void OnDrag()
		{
			PositionChanged?.Invoke(this);
		}

		private void OnMouseLeave(object? obj)
		{
			if (BackgroundColor==Brushes.Magenta)
			{
				BackgroundColor = Brushes.AliceBlue;
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		private void OnMouseEnter(object? obj)
		{
			if (DraggingLine)
			{
				BackgroundColor = Brushes.Magenta;
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		private void OnLoaded(object? obj)
		{
			SetUpAdorner();
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
		void OnDragStarted(IDragSource dragSource)
		{
			DragStarted?.Invoke(dragSource);
		}
		private void AddChild(int index)
		{
			StrategyModel strategyModel = new StrategyModel("strategyName", "className");
			StrategyView strategyView = new StrategyView();
			StrategyViewModel strategyViewModel = new StrategyViewModel(strategyView, strategyModel);
			strategyView.DataContext = strategyViewModel;
			strategyViewModel.Dropped += OnChildDrop;
			strategyViewModel.DragStarted += OnDragStarted;
			strategyViewModel.Destroy += OnDeleteChild;
			PositionChanged += strategyViewModel.OnPositionChanged;
			StrategyViews.Insert(index, strategyView);
			StrategyViewModels.Insert(index, strategyViewModel);
			_strategySetModel.Strategies.Insert(index, strategyModel);
			// strategyViewModel.SetUpDummyAdorner();
			strategyViewModel.Text = "Strategy " + number.ToString();
			CanvasClicked += strategyViewModel.OnCanvasClicked;
			KeyDown += strategyViewModel.OnKeyDown;
			number++;
		}

		public void OnDrop(object? obj)
		{
			DragEventArgs e = (obj as DragEventArgs)!;
			AddChild(0);
			e.Handled = true;
		}
		public void OnChildDrop(StrategyViewModel dropped, object? obj)
		{
			int childIndex = StrategyViewModels.FindIndex(x => x == dropped);
			DragEventArgs e = (obj as DragEventArgs)!;
			if (childIndex != -1)
			{
				DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
				if (displayTile != null)
				{
					AddChild(childIndex + 1);
				}
				else
				{
					Console.WriteLine("This is not a display tile!");
				}
			}
			e.Handled = true;
		}
		public void OnSelect(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			IsSelected = true;
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
		}
		public void OnCanvasClicked()
		{
			OnDeselect(null);
			CanvasClicked?.Invoke();
		}
		public void OnKeyDown(KeyEventArgs e)
		{
			if (IsSelected && e.Key == Key.Delete)
			{
				Destroy?.Invoke(this);
				IsSelected = false;
			}
			else
			{
				KeyDown?.Invoke(e);
			}
		}
		public void OnDeleteChild(StrategyViewModel strategyViewModel)
		{
			int index = StrategyViewModels.FindIndex(x => x == strategyViewModel);
			if (index != -1)
			{
				strategyViewModel.IsSelected = false;
				StrategyViewModels.RemoveAt(index);
				StrategyViews.RemoveAt(index);
				_strategySetModel.Strategies.RemoveAt(index);
			}
			else
			{
				Console.WriteLine("Cannot delete strategy!");
			}
		}
		public void OnMouseLeftButtonUp(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			Console.WriteLine("Mouse left button up triggered in strategy set.");
			DragEnded?.Invoke(this);
		}
		public void OnConnectionLineDestroyed(ConnectionLine line)
		{
			if (Line!=line)
			{
				Console.WriteLine("不是一条线？");
			}
			Line = null;
		}
		public void OnDragLineStarted()
		{
			DraggingLine = true;
		}
		public void OnDragLineEnded()
		{
			DraggingLine = false;
		}

		public void OnLineEnteringDestroyed(ConnectionLine line)
		{
			LineEntering = null;
			LinkingFrom = null;
		}
	}
}
