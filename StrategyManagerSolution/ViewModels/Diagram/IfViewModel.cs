using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class IfViewModel:ViewModelBase, IDiagramItem, IDragDestination, ISelectable
	{
		public MoveAdorner MoveAdorner { get; }
		public IfView View { get; }
		private IfModel _ifModel;

	
		public bool DraggingLine { get; set; }

		public Point CanvasPos
		{
			get { return _ifModel.CanvasPos; }
			set { _ifModel.CanvasPos = value;}
		}
		// 接口
		public UserControl ViewRef => View;
		public FrameworkElement DragDestinationView => View;
		public DiagramElementModel ModelRef => _ifModel;
		public DiagramElementModel DestinationModel => _ifModel;
		
		public string IfStatementText
		{
			get { return _ifModel.IfStatementText; }
			set { _ifModel.IfStatementText = value; }
		}
		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set; }

		public Point Offset { get; } = new Point(0, 20);

		public CaseViewModel TrueCaseViewModel { get; }
		public CaseViewModel FalseCaseViewModel { get; }
		public ImageSource ImageSource { get; } = new BitmapImage(new Uri("../../../Images/if.jpg", UriKind.Relative));
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;
		public bool IsSelected { get; set; }
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
		public IfViewModel(IfView ifView, IfModel ifModel)
		{
			View = ifView;
			_ifModel = ifModel;
			MoveAdorner = new MoveAdorner(View, 0,0,30,30);
			MoveAdorner.Drag += OnDrag;
			
			TrueCaseViewModel = new CaseViewModel(View.TrueCase, _ifModel.TrueCaseModel);
			FalseCaseViewModel = new CaseViewModel(View.FalseCase, _ifModel.FalseCaseModel);
			//内部事件
			TrueCaseViewModel.DragStarted += OnDragStarted;
			FalseCaseViewModel.DragStarted += OnDragStarted;
			//外部事件
			PositionChanged += TrueCaseViewModel.OnPositionChanged;
			CanvasClicked += TrueCaseViewModel.OnCanvasClicked; 
			PositionChanged += FalseCaseViewModel.OnPositionChanged;
			CanvasClicked += FalseCaseViewModel.OnCanvasClicked;
			// activate image using bug
			double a = ImageSource.Width;

			SelectCommand = new Command(OnSelect);
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
				Destroy?.Invoke(TrueCaseViewModel);
				Destroy?.Invoke(FalseCaseViewModel);
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
