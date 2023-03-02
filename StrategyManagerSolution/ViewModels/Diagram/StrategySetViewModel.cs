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
using Contracts.Enums;

namespace StrategyManagerSolution.ViewModels
{
	internal class StrategySetViewModel : ViewModelBase, ISelectable, IDragDestination, IDiagramItem
	{
		public MoveAdorner MoveAdorner { get; set; }
		public StrategySetView View { get; set; }
		private StrategySetModel _strategySetModel;
		//接口
		public UserControl ViewRef => View;
		public FrameworkElement DragDestinationView => View;
		public DiagramElementModel ModelRef => _strategySetModel;
		public DiagramElementModel DestinationModel => _strategySetModel;
		public ConnectionLine? Line { get; set; }
		public bool DraggingLine { get; set; }
		public bool IsSelected { get; set; }
		private int number = 0;
		public ImageSource Image { get; set; }
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;

		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set; }
		public string Text
		{
			get { return _strategySetModel.StrategySetName; }
			set { _strategySetModel.StrategySetName = value; }
		}
		public Point Offset { get; } = new Point(0, 20);
		public Point CanvasPos
		{
			get { return _strategySetModel.CanvasPos; }
			set { _strategySetModel.CanvasPos = value; }
		}
		public ObservableCollection<StrategyView> StrategyViews { get; } = new();
		public List<StrategyViewModel> StrategyViewModels { get; } = new();
		// Commands
		public Command DropCommand { get; }
		public Command SelectCommand { get; }
		public Command MouseLeftButtonUpCommand { get; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }

		

		
		// Events
		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<IDragDestination>? DragEnded;
		public event Action<ViewModelBase>? PositionChanged;
		public event Action<ViewModelBase>? Destroy;
		public event Action<ViewModelBase>? OpenScript;
		public StrategySetViewModel(StrategySetView view, StrategySetModel strategySetModel)
		{
			View = view;
			_strategySetModel = strategySetModel;
			string imageName = _strategySetModel.Type == StrategySetType.Hierarchical ? "../../../Images/hierarchy.jpg" : "../../../Images/liberty.jpg";
			Image = new BitmapImage(new Uri(imageName, UriKind.Relative));
			//activate image using bug
			double a = Image.Width;

			DropCommand = new Command(OnDrop);
			SelectCommand = new Command(OnSelect);
			MouseLeftButtonUpCommand = new Command(OnMouseLeftButtonUp);
			LoadedCommand = new Command(OnLoaded);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			
			MoveAdorner = new MoveAdorner(view,0, 0, 30, 30);
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
		public void OnDragStarted(IDragSource dragSource)
		{
			DragStarted?.Invoke(dragSource);
		}
		private void AddChild(int index)
		{
			StrategyModel strategyModel = new StrategyModel("strategyName", "className");
			StrategyView strategyView = new StrategyView();
			StrategyViewModel strategyViewModel = new StrategyViewModel(strategyView, strategyModel);
			//连接上下文
			strategyView.DataContext = strategyViewModel;
			//属性注册
			strategyViewModel.Text = "Strategy " + number.ToString();
			//内部事件
			strategyViewModel.Dropped += OnChildDrop;
			strategyViewModel.DragStarted += OnDragStarted;
			strategyViewModel.Destroy += OnDeleteChild;
			strategyViewModel.OpenScript += OnOpenScript;
			//外部事件
			PositionChanged += strategyViewModel.OnPositionChanged;
			KeyDown += strategyViewModel.OnKeyDown;
			CanvasClicked += strategyViewModel.OnCanvasClicked;
			//三重添加
			StrategyViews.Insert(index, strategyView);
			StrategyViewModels.Insert(index, strategyViewModel);
			_strategySetModel.Strategies.Insert(index, strategyModel);
			
			number++;
		}

		public void OnOpenScript(ViewModelBase viewModelBase)
		{
			OpenScript?.Invoke(viewModelBase);
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
			TextColor = Brushes.LightGreen;
			OnPropertyChanged(nameof(TextColor));
		}
		public void OnDeselect(object? obj)
		{
			IsSelected = false;
			TextColor = Brushes.AliceBlue;
			OnPropertyChanged(nameof(TextColor));
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
				foreach(var strategyViewModel in StrategyViewModels)
				{
					Destroy?.Invoke(strategyViewModel);
				}
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
				Destroy?.Invoke(strategyViewModel);
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
