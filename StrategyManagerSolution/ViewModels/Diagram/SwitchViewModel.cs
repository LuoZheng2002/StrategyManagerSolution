using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.ViewModels.Form;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class SwitchViewModel: ViewModelBase, IDiagramItem, IDragDestination, ISelectable
	{
		public MoveAdorner MoveAdorner { get; }
		public SwitchView View { get; }
		private SwitchModel _switchModel;
		
		public bool DraggingLine { get; set; }
		public Point CanvasPos
		{
			get { return _switchModel.CanvasPos; }
			set { _switchModel.CanvasPos = value; }
		}
		//接口
		public UserControl ViewRef => View;
		public FrameworkElement DragDestinationView => View;
		public DiagramElementModel ModelRef => _switchModel;
		public DiagramElementModel DestinationModel => _switchModel;

		public string SwitchTargetText
		{
			get { return _switchModel.SwitchTargetText; }
			set { _switchModel.SwitchTargetText= value; }
		}
		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set; }
		public Point Offset { get; } = new Point(0, 20);
		public List<CaseViewModel> CaseViewModels { get; } = new();
		public ObservableCollection<CaseView> CaseViews { get; } = new();
		public ImageSource ImageSource { get; } = new BitmapImage(new Uri("../../../Images/switch.jpg", UriKind.Relative));
		public Brush BackgroundColor { get; set; } = Brushes.AliceBlue;
		public Brush TextColor { get; set; } = Brushes.AliceBlue;
		public bool IsSelected { get; set; }
		// Commands
		public Command SelectCommand { get; }
		public Command MouseLeftButtonUpCommand { get; }
		public Command LoadedCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }
		public Command AddCaseCommand { get; }

		//Events
		public event Action? CanvasClicked;
		public event Action<KeyEventArgs>? KeyDown;
		public event Action<IDragSource>? DragStarted;
		public event Action<IDragDestination>? DragEnded;
		public event Action<ViewModelBase>? PositionChanged;
		public event Action<ViewModelBase>? Destroy;

		public SwitchViewModel(SwitchView switchView, SwitchModel switchModel)
		{
			View = switchView;
			_switchModel= switchModel;
			MoveAdorner = new MoveAdorner(View, 0, 0, 30, 30);
			MoveAdorner.Drag += OnDrag;

			// activate image using bug
			double a = ImageSource.Width;

			SelectCommand = new Command(OnSelect);
			MouseLeftButtonUpCommand = new Command(OnMouseLeftButtonUp);
			LoadedCommand = new Command(OnLoaded);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			AddCaseCommand = new Command(OnAddCase);
		}

		private void OnAddCase(object? obj)
		{
			PopupWindow popupWindow = new PopupWindow();
			CaseConfigViewModel caseConfigViewModel = new();
			popupWindow.DataContext = new PopupViewModel(popupWindow, caseConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}

			CaseModel caseModel = new CaseModel(caseConfigViewModel.CaseName,
				caseConfigViewModel.CaseText);
			CaseView caseView = new CaseView();
			CaseViewModel caseViewModel = new CaseViewModel(caseView, caseModel, true);
			//连接上下文
			caseView.DataContext = caseViewModel;
			// 外部事件
			CanvasClicked += caseViewModel.OnCanvasClicked;
			KeyDown += caseViewModel.OnKeyDown;
			PositionChanged += caseViewModel.OnPositionChanged;
			// 内部事件
			caseViewModel.Destroy += OnCaseDestroy;
			caseViewModel.DragStarted += OnDragStarted;
			caseViewModel.UpperClicked += OnCaseUpperClicked;
			caseViewModel.LowerClicked += OnCaseLowerClicked;
			// 三重添加
			CaseViews.Add(caseView);
			CaseViewModels.Add(caseViewModel);
			_switchModel.CaseModels.Add(caseModel);
		}
		public void OnCaseDestroy(CaseViewModel caseViewModel)
		{
			CaseViews.Remove(caseViewModel.View);
			_switchModel.CaseModels.Remove(caseViewModel.CaseModel);
			Destroy?.Invoke(caseViewModel);
			CaseViewModels.Remove(caseViewModel);
			Task task = new Task(() =>
			{
				Thread.Sleep(10);
				Application.Current.Dispatcher.Invoke(() =>
				{
					foreach(var caseViewModel in CaseViewModels)
					{
						PositionChanged?.Invoke(caseViewModel);
					}
				});
			});
			task.Start();
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
				foreach(var caseViewModel in CaseViewModels)
				{
					Destroy?.Invoke(caseViewModel);
				}
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
			Console.WriteLine("Mouse left button up triggered in switch module.");
			DragEnded?.Invoke(this);
		}
		public void OnCaseUpperClicked(CaseViewModel caseViewModel)
		{
			int index = CaseViewModels.IndexOf(caseViewModel);
			Debug.Assert(index !=  -1);
			if (index == 0)
				return;
			
			CaseViewModel tempViewModel = CaseViewModels[index-1];
			CaseViewModels[index - 1] = CaseViewModels[index];
			CaseViewModels[index] = tempViewModel;
			CaseViews.Move(index, index - 1);
			Task task = new Task(() =>
			{
				Thread.Sleep(10);
				Application.Current.Dispatcher.Invoke(() =>
				{
					PositionChanged?.Invoke(CaseViewModels[index - 1]);
					PositionChanged?.Invoke(CaseViewModels[index]);
				});
			});
			task.Start();
		}
		public void OnCaseLowerClicked(CaseViewModel caseViewModel)
		{
			int index = CaseViewModels.IndexOf(caseViewModel);
			Debug.Assert(index != -1);
			if (index == CaseViewModels.Count - 1)
				return;
			CaseViewModel tempViewModel = CaseViewModels[index + 1];
			CaseViewModels[index + 1] = caseViewModel;
			CaseViewModels[index] = tempViewModel;
			CaseViews.Move(index, index + 1);

			Task task = new Task(() =>
			{
				Thread.Sleep(10);
				Application.Current.Dispatcher.Invoke(() =>
				{
					PositionChanged?.Invoke(CaseViewModels[index]);
					PositionChanged?.Invoke(CaseViewModels[index + 1]);
				});
			});
			task.Start();
		}
	}
}
