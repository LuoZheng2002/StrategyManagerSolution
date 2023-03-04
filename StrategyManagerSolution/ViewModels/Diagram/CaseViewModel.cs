using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.ViewModels.Form;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.Views.Diagram;
using StrategyManagerSolution.Views.Form;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class CaseViewModel:ViewModelBase, ISelectable, IDragSource
	{
		public bool ShowPosAdorner { get; } = false;
		private NodeAdorner _nodeAdorner;
		private PosAdorner _posAdorner;
		public CaseView View { get; }
		private CaseModel _caseModel;
		public CaseModel CaseModel => _caseModel;
		public FrameworkElement DragSourceView => View;
		public TextBlock TextBlock => View.TextBlock;
		public Brush Background { get; set; } = Brushes.AliceBlue;
		public bool IsSelected { get; set; }
		public ConnectionLine? LineLeaving { get; set; }
		public IDragDestination? LinkingTo { get; set; }
		public DiagramElementModel? ModelLinkingTo
		{
			get { return _caseModel.LinkingTo; }
			set { _caseModel.LinkingTo = value; }
		}
		public string CaseName
		{
			get { return _caseModel.CaseName; }
			set { _caseModel.CaseName = value;}
		}
		public string CaseText
		{
			get { return _caseModel.CaseText; }
			set { _caseModel.CaseText = value; }
		}
		public Point Offset => new Point(View.ActualWidth, View.ActualHeight/2);
		public Brush TextColor { get; set; } = Brushes.LightBlue;
		public Command SelectCommand { get; }
		public Command MouseEnterCommand { get; }
		public Command MouseLeaveCommand { get; }

		public event Action? CanvasClicked;
		public event Action<IDragSource>? DragStarted;
		public event Action<ViewModelBase>? PositionChanged;
		public event Action<CaseViewModel>? Destroy;
		public event Action<CaseViewModel>? UpperClicked;
		public event Action<CaseViewModel>? LowerClicked;
		public CaseViewModel(CaseView view, CaseModel caseModel, bool showPosAdorner)
		{
			ShowPosAdorner = showPosAdorner;
			View = view;
			_caseModel = caseModel;
			SelectCommand = new Command(OnMouseDown);
			MouseEnterCommand = new Command(OnMouseEnter);
			MouseLeaveCommand = new Command(OnMouseLeave);
			_nodeAdorner = new NodeAdorner(TextBlock);
			_nodeAdorner.MouseLeave += OnMouseLeaveAdorner;
			_nodeAdorner.DragStarted += OnDragStarted;
			_posAdorner = new PosAdorner(TextBlock);
			_posAdorner.MouseLeave += OnMouseLeaveAdorner;
			_posAdorner.UpperClicked += OnUpperClicked;
			_posAdorner.LowerClicked += OnLowerClicked;
		}
		private void OnUpperClicked()
		{
			UpperClicked?.Invoke(this);
		}
		private void OnLowerClicked()
		{
			LowerClicked?.Invoke(this);
		}

		public void OnKeyDown(KeyEventArgs e)
		{
			if (IsSelected && e.Key == Key.Delete)
			{
				Destroy?.Invoke(this);
				IsSelected = false;
			}
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

		public void OnDeselect(object? obj)
		{
			IsSelected = false;
			TextColor = Brushes.AliceBlue;
			OnPropertyChanged(nameof(TextColor));
		}
		private void OnDoubleClick()
		{
			PopupWindow popupWindow = new PopupWindow();
			CaseConfigViewModel caseConfigViewModel = new CaseConfigViewModel(_caseModel);
			popupWindow.DataContext = new PopupViewModel(popupWindow, caseConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}
			OnPropertyChanged(nameof(CaseText));
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
		public void OnCanvasClicked()
		{
			CanvasClicked?.Invoke();
			OnDeselect(null);
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
				if (ShowPosAdorner)
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
				if (ShowPosAdorner)
					adornerLayer.Remove(_posAdorner);
			}
			else
			{
				Console.WriteLine("Attempt to remove node adorner twice.");
			}
		}
		~CaseViewModel()
		{
			Console.WriteLine("Case Destructed!");
		}

		public void OnLineLeavingDestroyed(ConnectionLine line)
		{
			LineLeaving = null;
			LinkingTo = null;
			ModelLinkingTo = null;
		}
		public void OnPositionChanged(ViewModelBase viewModelBase)
		{
			PositionChanged?.Invoke(this);
		}
	}
}
