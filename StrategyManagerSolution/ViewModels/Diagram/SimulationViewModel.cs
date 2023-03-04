using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Documents;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.ViewModels.Form;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class SimulationViewModel: ViewModelBase, IDiagramItem,IDragDestination, ISelectable
	{
		public MoveAdorner MoveAdorner { get;}
		public SimulationView View { get; }
		private SimulationModel _simulationModel;
		public SimulationModel SimulationModel => _simulationModel;
		public bool DraggingLine { get; set; }

		public Point CanvasPos
		{
			get { return _simulationModel.CanvasPos; }
			set { _simulationModel.CanvasPos = value; }
		}
		//接口
		public UserControl ViewRef => View;
		public FrameworkElement DragDestinationView => View;
		public DiagramElementModel ModelRef => _simulationModel;
		public DiagramElementModel DestinationModel => _simulationModel;

		public string SimulationDescription
		{
			get { return _simulationModel.SimulationDescription; }
			set { _simulationModel.SimulationDescription = value; }
		}
		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set;}
		public Point Offset { get; } = new Point(0, 20);
		public CaseViewModel Player1WinsViewModel { get; }
		public CaseViewModel Player2WinsViewModel { get; }
		public ImageSource ImageSource { get; } = new BitmapImage(new Uri("../../../Images/simulation.jpg", UriKind.Relative));
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;
		public bool IsSelected { get; set;}
		public string Player1Name => _simulationModel.Player1Name;
		public string Player2Name => _simulationModel.Player2Name;
		// Commands
		public Command SelectCommand { get; }
		public Command MouseLeftButtonUpCommand { get; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		//Events
		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<IDragDestination>? DragEnded;
		public event Action<ViewModelBase>? PositionChanged;
		public event Action<ViewModelBase>? Destroy;
		public event Action<ViewModelBase>? OpenScript;
        public SimulationViewModel(SimulationView simulationView, SimulationModel simulationModel)
        {
			View = simulationView;
			_simulationModel = simulationModel;
			MoveAdorner = new MoveAdorner(View, 0, 0, 30, 30);
			MoveAdorner.Drag += OnDrag;
			Player1WinsViewModel = new CaseViewModel(View.Player1WinsCase, _simulationModel.Player1WinsCaseModel, false);
			Player2WinsViewModel = new CaseViewModel(View.Player2WinsCase, _simulationModel.Player2WinsCaseModel, false);
			//内部事件
			Player1WinsViewModel.DragStarted += OnDragStarted;
			Player2WinsViewModel.DragStarted += OnDragStarted;
			//外部事件
			PositionChanged += Player1WinsViewModel.OnPositionChanged;
			CanvasClicked += Player1WinsViewModel.OnCanvasClicked;
			PositionChanged += Player2WinsViewModel.OnPositionChanged;
			CanvasClicked += Player2WinsViewModel.OnCanvasClicked;
			// activate image using bug
			double a = ImageSource.Width;

			SelectCommand = new Command(OnMouseDown);
			MouseLeftButtonUpCommand = new Command(OnMouseLeftButtonUp);
			LoadedCommand = new Command(OnLoaded);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
		}

		private void OnMouseEnter(object? obj)
		{
			if (DraggingLine)
			{
				BackgroundColor = Brushes.Magenta;
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}
		private void OnMouseLeave(object? obj)
		{
			if (BackgroundColor == Brushes.Magenta)
			{
				BackgroundColor = Brushes.AliceBlue;
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
		public void OnLineEnteringDestroyed(ConnectionLine line)
		{
			LineEntering = null;
			LinkingFrom = null;
		}
		public void OnOpenScript()
		{
			OpenScript?.Invoke(this);
		}
		private void OnDoubleClick()
		{
			PopupWindow popupWindow = new PopupWindow();
			SimulationConfigViewModel simulationConfigViewModel = new SimulationConfigViewModel(_simulationModel);
			simulationConfigViewModel.OpenScript += () => OpenScript?.Invoke(this);
			popupWindow.DataContext = new PopupViewModel(popupWindow, simulationConfigViewModel); ;
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}
			OnPropertyChanged(nameof(SimulationDescription));
			OnPropertyChanged(nameof(Player1Name)); 
			OnPropertyChanged(nameof(Player2Name));
		}
		public void OnMouseDown(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			if (e.ClickCount == 2)
			{
				OnDoubleClick();
				return;
			}
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
				Destroy?.Invoke(Player1WinsViewModel);
				Destroy?.Invoke(Player2WinsViewModel);
				Destroy?.Invoke(this);
				IsSelected = false;
			}
			else
			{
				KeyDown?.Invoke(e);
			}
		}
		private void OnDrag()
		{
			PositionChanged?.Invoke(this);
		}

		public void OnDragLineStarted()
		{
			DraggingLine = true;
		}
		public void OnDragLineEnded()
		{
			DraggingLine = false;
		}
		public void OnDragStarted(IDragSource dragSource)
		{
			DragStarted?.Invoke(dragSource);
		}
		public void OnMouseLeftButtonUp(object? obj)
		{
			MouseButtonEventArgs e = (obj as MouseButtonEventArgs)!;
			e.Handled = true;
			Console.WriteLine("Mouse left button up triggered in if module.");
			DragEnded?.Invoke(this);
		}
	}
}
