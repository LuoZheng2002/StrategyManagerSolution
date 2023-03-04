using Contracts.Enums;
using Contracts.MVVMModels;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Utils;
using StrategyManagerSolution.ViewModels.Form;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StrategyManagerSolution.ViewModels.Diagram
{
    
    internal class DiagramViewModel:ViewModelBase
    {
        private Model _model;
        private readonly DragInfo _dragInfo = new DragInfo();
        public Canvas? Canvas { get; set; }
        public ObservableCollection<FrameworkElement> DiagramItems { get; } = new();
        public List<ViewModelBase> DiagramItemViewModels { get; } = new();
        public DisplayTileViewModel DisplayTile0 { get; }
        public DisplayTileViewModel DisplayTile1 { get; }
        public DisplayTileViewModel DisplayTile2 { get; }
        public DisplayTileViewModel DisplayTile3 { get; }
		public DisplayTileViewModel DisplayTile4 { get; }
		public DisplayTileViewModel DisplayTile5 { get; }
		public DisplayTileViewModel DisplayTile6 { get; }
		public string HintText { get; set; } = "Diagram operation hint messages here!";
        public ObservableCollection<string> ProjectFiles { get; set; } = new();
        public string ProjectName { get; set; } ="";
        public string SelectedFile { get; set; } = "未选择文件";
        public Command DropCommand { get; }
        public Command ClickCommand { get; }
        public Command MouseLeftButtonUpCommand { get; }
        public Command CanvasLoadedCommand { get; }
        public Command FileSelectionChangedCommand { get; }
        public Command SaveFileCommand { get; }
        public Command DeleteFileCommand { get; }
        public Command CreateNewFileCommand { get; }
        public event Action? CanvasClicked;
        public event Action<KeyEventArgs>? KeyDown;
        public event Action? DragLineStarted;
        public event Action? DragLineEnded;
        
        public DiagramViewModel(Model model)
        {
            _model = model;
            DisplayTile0 = new DisplayTileViewModel() { Text = "开始", ImageName = "../../../Images/Start.jpg" };
			DisplayTile1 = new DisplayTileViewModel() { Text = "单个策略", ImageName = "../../../Images/bulb.jpg" };
            DisplayTile2 = new DisplayTileViewModel() { Text = "平行策略集", ImageName = "../../../Images/liberty.jpg" };
            DisplayTile3 = new DisplayTileViewModel() { Text = "等级策略集", ImageName = "../../../Images/hierarchy.jpg" };
            DisplayTile4 = new DisplayTileViewModel() { Text = "If模块", ImageName = "../../../Images/if.jpg" };
            DisplayTile5 = new DisplayTileViewModel() { Text = "Switch模块", ImageName = "../../../Images/switch.jpg" };
			DisplayTile6 = new DisplayTileViewModel() { Text = "推演模块", ImageName = "../../../Images/simulation.jpg" };
			DropCommand = new Command(OnDrop);
            ClickCommand = new Command(OnClick);
            MouseLeftButtonUpCommand= new Command(OnMouseLeftButtonUp);
            CanvasLoadedCommand = new Command(OnCanvasLoaded);
            FileSelectionChangedCommand = new Command(OnFileSelectionChanged);
            SaveFileCommand = new Command(OnSaveFile);
            CreateNewFileCommand = new Command(OnCreateNewFile);
            DeleteFileCommand = new Command(OnDeleteFile);
            foreach(var solutionName in _model.CurrentProjectModel!.SolutionNames)
            {
                ProjectFiles.Add(solutionName + ".smsln");
            }
            ProjectName = $"项目'{_model.CurrentProjectModel.ProjectName}'";
            if (_model.CurrentSolutionModel != null)
            {
                SelectedFile = $"文件: {_model.CurrentSolutionModel!.SolutionFileName}";
				LoadSolutionFile();
			}
            else
            {
                SwitchFile(_model.CurrentProjectModel.CurrentSolutionFileName);
            }
        }

		private void OnDeleteFile(object? obj)
		{
            if (_model.CurrentSolutionModel == null)
                return;
            string solutionFileName = _model.CurrentSolutionModel!.SolutionFileName;
            string solutionName = _model.CurrentSolutionModel!.SolutionName;
			ProjectFiles.Remove(solutionFileName);
            SelectedFile = "未选择文件";
            _model.CurrentProjectModel!.DeleteSolution(solutionName);
            _model.CurrentSolutionModel = null;
            DiagramItems.Clear();
            DiagramItemViewModels.Clear();
		}

		private void OnCreateNewFile(object? obj)
		{
			PopupWindow popupWindow = new PopupWindow();
            FileConfigViewModel fileConfigViewModel = new FileConfigViewModel();
            popupWindow.DataContext = new PopupViewModel(popupWindow, fileConfigViewModel);
            bool? result = popupWindow.ShowDialog();
            if (result == null || !result.Value)
            {
                return;
            }
            string solutionName = fileConfigViewModel.SolutionName;
            _model.CurrentProjectModel!.AddSolution(solutionName);
            ProjectFiles.Add(solutionName + ".smsln");
            SwitchFile(solutionName + ".smsln");
		}
        private void SwitchFile(string filename)
        {
			if (_model.CurrentSolutionModel != null)
			{
				_model.SaveCurrentSolution();
			}
			_model.CurrentSolutionModel = Serializer.Deserialize<SolutionModel>(_model.CurrentProjectModel!.ProjectFolder + "/" + filename);
			SelectedFile = $"文件: {filename}";
			OnPropertyChanged(nameof(SelectedFile));
			LoadSolutionFile();
		}
		public void OnWindowClosing()
		{
            
		}

		private void OnSaveFile(object? obj)
		{
            _model.SaveCurrentSolution();
		}
        // Can only find view models on top layer.
        private IDragDestination FindDragDestination(DiagramElementModel model)
        {
            IDragDestination? result = null;
            foreach(var viewModel in DiagramItemViewModels)
            {
                if (viewModel is IDiagramItem)
                {
					IDiagramItem diagramItem = (IDiagramItem)viewModel;
                    if (diagramItem.ModelRef == model)
                    {
                        Debug.Assert(viewModel is IDragDestination);
                        result = (IDragDestination)viewModel;
                        break;
                    }
                }
            }
            Debug.Assert(result != null);
            return result;
        }

		private void LoadSolutionFile()
        {
            DiagramItems.Clear();
            DiagramItemViewModels.Clear();
            foreach(var diagramItemModel in _model.CurrentSolutionModel!.DiagramItemModels)
            {
                if (diagramItemModel is StartModel)
                {
                    LoadStartView((StartModel)diagramItemModel);
                }
                else if (diagramItemModel is StrategySetModel)
                {
                    LoadStrategySetView((StrategySetModel)diagramItemModel);
                }
                else if(diagramItemModel is IfModel)
                {
                    LoadIfView((IfModel)diagramItemModel);
                }
                else if(diagramItemModel is SwitchModel)
                {
                    LoadSwitchView((SwitchModel)diagramItemModel);
                }
                else if(diagramItemModel is SimulationModel)
                {
                    LoadSimulationView((SimulationModel)diagramItemModel);
                }
            }
            Task task = new Task(() =>
            {
                Thread.Sleep(10);
                Application.Current.Dispatcher.Invoke(LoadLines);
            });
            task.Start();
        }
        private void LoadLines()
        {
			foreach (var diagramItemViewModel in DiagramItemViewModels)
			{
				if (diagramItemViewModel is IDragSource)
				{
					IDragSource dragSource = (IDragSource)diagramItemViewModel;
					IDiagramItem diagramItem = (IDiagramItem)diagramItemViewModel;
					Debug.Assert(diagramItem.ModelRef is ILinkSource);
					DiagramElementModel? destinationModel = ((ILinkSource)diagramItem.ModelRef).LinkingTo;
					if (destinationModel != null)
					{
						IDragDestination dragDestination = FindDragDestination(destinationModel);
						AddConnection(dragSource, dragDestination);
					}
				}
				else if (diagramItemViewModel is StrategySetViewModel)
				{
					StrategySetViewModel strategySetViewModel = (StrategySetViewModel)diagramItemViewModel;
					foreach (var strategyViewModel in strategySetViewModel.StrategyViewModels)
					{
						if (strategyViewModel.StrategyModel.LinkingTo != null)
						{
							IDragDestination dragDestination = FindDragDestination(strategyViewModel.StrategyModel.LinkingTo);
							AddConnection(strategyViewModel, dragDestination);
						}
					}
				}
                else if (diagramItemViewModel is IfViewModel)
                {
                    IfViewModel ifViewModel = (IfViewModel)diagramItemViewModel;
                    if (ifViewModel.TrueCaseViewModel.CaseModel.LinkingTo !=null)
                    {
						IDragDestination dragDestination = FindDragDestination(ifViewModel.TrueCaseViewModel.CaseModel.LinkingTo);
						AddConnection(ifViewModel.TrueCaseViewModel, dragDestination);
					}
					if (ifViewModel.FalseCaseViewModel.CaseModel.LinkingTo != null)
					{
						IDragDestination dragDestination = FindDragDestination(ifViewModel.FalseCaseViewModel.CaseModel.LinkingTo);
						AddConnection(ifViewModel.FalseCaseViewModel, dragDestination);
					}
				}
                else if (diagramItemViewModel is SwitchViewModel)
                {
                    SwitchViewModel switchViewModel = (SwitchViewModel)diagramItemViewModel;
                    foreach(var caseViewModel in switchViewModel.CaseViewModels)
                    {
                        if (caseViewModel.CaseModel.LinkingTo !=null)
                        {
                            IDragDestination dragDestination = FindDragDestination(caseViewModel.CaseModel.LinkingTo);
                            AddConnection(caseViewModel, dragDestination);
                        }
                    }
                }
                else if(diagramItemViewModel is SimulationViewModel)
                {
                    SimulationViewModel simulationViewModel = (SimulationViewModel)diagramItemViewModel;
                    if (simulationViewModel.Player1WinsViewModel.CaseModel.LinkingTo !=null)
                    {
						IDragDestination dragDestination = FindDragDestination(simulationViewModel.Player1WinsViewModel.CaseModel.LinkingTo);
						AddConnection(simulationViewModel.Player1WinsViewModel, dragDestination);
					}
					if (simulationViewModel.Player2WinsViewModel.CaseModel.LinkingTo != null)
					{
						IDragDestination dragDestination = FindDragDestination(simulationViewModel.Player2WinsViewModel.CaseModel.LinkingTo);
						AddConnection(simulationViewModel.Player2WinsViewModel, dragDestination);
					}
				}
			}
		}
		private void OnFileSelectionChanged(object? obj)
		{
			SelectionChangedEventArgs e = (SelectionChangedEventArgs)obj!;
            if (e.AddedItems.Count > 0)
            {
				string filename = (string)e.AddedItems[0]!;
				SwitchFile(filename);
                _model.CurrentProjectModel!.CurrentSolutionFileName = filename;
			}
		}

		public void OnDrop(object? obj)
        {
            if (_model.CurrentSolutionModel==null)
            {
                HintText = "请先选择流程图文件!";
                OnPropertyChanged(nameof(HintText));
                return;
            }
            DragEventArgs e = (obj as DragEventArgs)!;
            DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
            if (displayTile == null)
                return;
            if (Canvas == null)
            {
				Console.WriteLine("The item doesn't drop on canvas!");
				HintText = "The item doesn't drop on canvas!";
				OnPropertyChanged(nameof(HintText));
                return;
			}
            Point pos = e.GetPosition(Canvas);
            switch(displayTile.Text)
            {
                case "开始":
                    CreateStartView(pos);
					break;
                case "等级策略集":
                    CreateStrategySetView(pos, StrategySetType.Hierarchical);
                    break;
                case "平行策略集":
                    CreateStrategySetView(pos, StrategySetType.Parallel);
                    break;
                case "If模块":
                    CreateIfView(pos);
                    break;
                case "Switch模块":
                    CreateSwitchView(pos);
                    break;
                case "推演模块":
                    CreateSimulationView(pos);
                    break;
            }
        }
        //用于撤销选中
		public void OnClick(object? obj)
        {
            Console.WriteLine("Clicked on canvas!");
            CanvasClicked?.Invoke();
        }
        public void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }
        void OnMouseLeftButtonUp(object? obj)
        {
            Console.WriteLine("Mouse released, drag interrupted");
            _dragInfo.Clear();
            DragLineEnded?.Invoke();
        }
        void OnDragStarted(IDragSource dragSource)
        {
            _dragInfo.Dragging = true;
            _dragInfo.DragSource = dragSource;            
			Console.WriteLine($"Start Dragging");
            DragLineStarted?.Invoke();
        }
        private void AddConnection(IDragSource dragSource, IDragDestination dragDestination)
        {
			Point dragSourcePos = dragSource.DragSourceView.TranslatePoint(new Point(0, 0), Canvas);
			Point startPos = new Point(dragSourcePos.X + dragSource.Offset.X, dragSourcePos.Y + dragSource.Offset.Y);
			Point dragDestinationPos = dragDestination.DragDestinationView.TranslatePoint(new Point(0, 0), Canvas);
			Point endPos = new Point(dragDestinationPos.X + dragDestination.Offset.X, dragDestinationPos.Y + dragDestination.Offset.Y);
			if (dragSource!.LineLeaving == null
				&& dragDestination!.LineEntering == null)
			{
				ConnectionLine line = new ConnectionLine(new Line
				{
					X1 = startPos.X,
					X2 = endPos.X,
					Y1 = startPos.Y,
					Y2 = endPos.Y,
					Stroke = Brushes.Black,
					StrokeThickness = 5
				},
				dragSource,
				dragDestination);
				// 内部注册外部事件
				KeyDown += line.OnKeyDown;
				CanvasClicked += line.OnDeselect;
				// 外部注册内部事件
				line.Destroyed += OnConnectionLineDestroyed;

				DiagramItems.Add(line.Line);
				dragSource.LineLeaving = line;
				dragDestination!.LineEntering = line;
                
				line.Destroyed += dragSource.OnLineLeavingDestroyed;
				line.Destroyed += dragDestination!.OnLineEnteringDestroyed;
				dragSource.LinkingTo = dragDestination;
				dragDestination.LinkingFrom = dragSource;
                dragSource.ModelLinkingTo = dragDestination.DestinationModel;
				// 外部订阅被连接物体的事件
				dragSource.PositionChanged += OnConnectedItemPositionChanged;
				dragDestination.PositionChanged += OnConnectedItemPositionChanged;

				_dragInfo.Clear();
			}
			else
			{
				Console.WriteLine("你已经连接过一条线了！");
				HintText = "你已经连接过一条线了！";
				OnPropertyChanged(nameof(HintText));
			}

		}
		void OnDragEnded(IDragDestination dragDestination)
        {
            IDragSource dragSource = _dragInfo.DragSource!;
            if (_dragInfo.Dragging)
            {
				AddConnection(dragSource, dragDestination);
            }
			_dragInfo.Clear();
			DragLineEnded?.Invoke();
		}
        void OnConnectionLineDestroyed(ConnectionLine line)
        {
            // 取消订阅位置移动事件
            line.DragSource.PositionChanged -= OnConnectedItemPositionChanged;
            line.DragDestination.PositionChanged -= OnConnectedItemPositionChanged;
            DiagramItems.Remove(line.Line);
        }
        void OnDiagramItemDestroy(ViewModelBase diagramItem)
        {
            if (diagramItem is IDragSource)
            {
                IDragSource dragSource = (IDragSource)diagramItem;
                if (dragSource.LineLeaving != null)
                {
                    // remove line by invoking its destruction so that connection is thoroughly destroyed
                    dragSource.LineLeaving.OnDestroy();
                }
            }
            if (diagramItem is IDragDestination)
            {
                IDragDestination dragDestination = (IDragDestination)diagramItem;
                if (dragDestination.LineEntering !=null)
                {
                    dragDestination.LineEntering.OnDestroy();
                }
            }
            if (diagramItem is StrategyViewModel)
            {
                return;
            }
            if (diagramItem is CaseViewModel)
            {
                return;
            }
            Debug.Assert(diagramItem is IDiagramItem);
            IDiagramItem dItem = (IDiagramItem)diagramItem;
            if (!DiagramItemViewModels.Contains(diagramItem))
            {
                throw new Exception("Cannot find view model in container");
            }
            if (!DiagramItems.Contains(dItem.ViewRef))
            {
                throw new Exception("Cannot find view in container");
            }
            if (!_model.CurrentSolutionModel!.DiagramItemModels.Contains(dItem.ModelRef))
            {
                throw new Exception("Cannot find model in container");
            }
            DiagramItemViewModels.Remove(diagramItem);
            DiagramItems.Remove(dItem.ViewRef);
            _model.CurrentSolutionModel!.DiagramItemModels.Remove(dItem.ModelRef);
        }
        void OnDiagramItemPositionChanged(ViewModelBase diagramItem)
        {
            if (diagramItem is IDiagramItem)
            {
                IDiagramItem dItem = (IDiagramItem)diagramItem;
                Point pos = dItem.ViewRef.TranslatePoint(new Point(0, 0), Canvas);
				dItem.CanvasPos = pos;
            }
        }
        void OnConnectedItemPositionChanged(ViewModelBase connectedItem)
        {
            if (connectedItem is IDragSource)
            {
                IDragSource dragSource = (IDragSource)connectedItem;
                if (dragSource.LineLeaving == null)
                {
					throw new Exception("Line missing, but position changed event subscribed");
				}
                Point dragSourcePos = dragSource.DragSourceView.TranslatePoint(new Point(0, 0), Canvas);
				dragSource.LineLeaving.Line.X1 = dragSourcePos.X + dragSource.Offset.X;
                dragSource.LineLeaving.Line.Y1 = dragSourcePos.Y + dragSource.Offset.Y;
            }
            if (connectedItem is IDragDestination)
            {
                IDragDestination dragDestination = (IDragDestination)connectedItem;
                if (dragDestination.LineEntering== null)
                {
					throw new Exception("Line missing, but position changed event subscribed");
				}
                Point dragDestinationPos = dragDestination.DragDestinationView.TranslatePoint(new Point(0, 0), Canvas);
                dragDestination.LineEntering.Line.X2 = dragDestinationPos.X + dragDestination.Offset.X;
                dragDestination.LineEntering.Line.Y2 = dragDestinationPos.Y + dragDestination.Offset.Y;
            }
        }

        //Retrieve Canvas from View
        void OnCanvasLoaded(object? obj)
        {
            RoutedEventArgs e = (obj as RoutedEventArgs)!;
            Canvas = e.Source as Canvas;
        }
        private void CreateStartView(Point pos)
        {
			StartModel startModel = new StartModel(pos);
			StartView startView = new StartView();
			StartViewModel startViewModel = new StartViewModel(startView, startModel);
            //连接上下文
			startView.DataContext = startViewModel;
			//外部事件
			CanvasClicked += startViewModel.OnDeselect;
			KeyDown += startViewModel.OnKeyDown;
			//内部事件
			startViewModel.Destroy += OnDiagramItemDestroy;
            startViewModel.PositionChanged += OnDiagramItemPositionChanged;
            startViewModel.DragStarted += OnDragStarted;
			//三重添加
			DiagramItems.Add(startView);
			DiagramItemViewModels.Add(startViewModel);
			_model.CurrentSolutionModel!.DiagramItemModels.Add(startModel);
			//调整位置
			Canvas.SetLeft(startView, startViewModel.CanvasPos.X);
			Canvas.SetTop(startView, startViewModel.CanvasPos.Y);
		}
        private void CreateStrategySetView(Point pos, StrategySetType type)
        {
			StrategySetModel strategySetModel = new StrategySetModel(type, pos);
			StrategySetView strategySetView = new StrategySetView();
			StrategySetViewModel strategySetViewModel = new StrategySetViewModel(strategySetView, strategySetModel);

			PopupWindow popupWindow = new PopupWindow();
            StrategySetConfigViewModel strategySetConfigViewModel = new(strategySetModel, type);
            popupWindow.DataContext = new PopupViewModel(popupWindow,strategySetConfigViewModel);
            bool? result = popupWindow.ShowDialog();
            if (result == null || !result.Value)
            {
                return;
            }

           // 连接上下文
			strategySetView.DataContext = strategySetViewModel;
			// 外部事件
			CanvasClicked += strategySetViewModel.OnCanvasClicked;
			KeyDown += strategySetViewModel.OnKeyDown;
			DragLineStarted += strategySetViewModel.OnDragLineStarted;
			DragLineEnded += strategySetViewModel.OnDragLineEnded;
			// 内部事件
			strategySetViewModel.DragStarted += OnDragStarted;
			strategySetViewModel.DragEnded += OnDragEnded;
            strategySetViewModel.Destroy += OnDiagramItemDestroy;
            strategySetViewModel.PositionChanged += OnDiagramItemPositionChanged;
            strategySetViewModel.OpenScript += OnOpenScript;
			// 三重添加
			DiagramItems.Add(strategySetView);
			DiagramItemViewModels.Add(strategySetViewModel);
			_model.CurrentSolutionModel!.DiagramItemModels.Add(strategySetModel);
            // 调整位置
			Canvas.SetLeft(strategySetView, strategySetViewModel.CanvasPos.X);
			Canvas.SetTop(strategySetView, strategySetViewModel.CanvasPos.Y);
		}
        private void CreateIfView(Point pos)
        {
            IfModel ifModel = new IfModel(pos);
            IfView ifView = new IfView();
            IfViewModel ifViewModel = new IfViewModel(ifView, ifModel);

			PopupWindow popupWindow = new PopupWindow();
            IfConfigViewModel ifConfigViewModel = new IfConfigViewModel(ifModel);
            ifConfigViewModel.OpenScript += ifViewModel.OnOpenScript;
            ifViewModel.OpenScript += OnOpenScript;
			popupWindow.DataContext = new PopupViewModel(popupWindow, ifConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}

			//连接上下文
			ifView.DataContext = ifViewModel;
            //外部事件
            CanvasClicked += ifViewModel.OnCanvasClicked;
            KeyDown += ifViewModel.OnKeyDown;
            DragLineStarted += ifViewModel.OnDragLineStarted;
            DragLineEnded += ifViewModel.OnDragLineEnded;
            //内部事件
            ifViewModel.DragStarted += OnDragStarted;
            ifViewModel.DragEnded += OnDragEnded;
            ifViewModel.Destroy += OnDiagramItemDestroy;
            ifViewModel.PositionChanged += OnDiagramItemPositionChanged;
            // 三重添加
            DiagramItems.Add(ifView);
            DiagramItemViewModels.Add(ifViewModel);
            _model.CurrentSolutionModel!.DiagramItemModels.Add(ifModel);
            //调整位置
            Canvas.SetLeft(ifView, ifViewModel.CanvasPos.X);
            Canvas.SetTop(ifView, ifViewModel.CanvasPos.Y);
        }
        private void CreateSwitchView(Point pos)
        {
            SwitchModel switchModel = new SwitchModel(pos);
            SwitchView switchView = new SwitchView();
            SwitchViewModel switchViewModel = new SwitchViewModel(switchView, switchModel);
            
			PopupWindow popupWindow = new PopupWindow();
			SwitchConfigViewModel switchConfigViewModel = new SwitchConfigViewModel(switchModel);
            switchConfigViewModel.OpenScript += switchViewModel.OnOpenScript;
            switchViewModel.OpenScript += OnOpenScript;
			popupWindow.DataContext = new PopupViewModel(popupWindow, switchConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}

			//连接上下文
			switchView.DataContext = switchViewModel;
            //外部事件
            CanvasClicked += switchViewModel.OnCanvasClicked;
            KeyDown += switchViewModel.OnKeyDown;
            DragLineStarted += switchViewModel.OnDragLineStarted;
            DragLineEnded += switchViewModel.OnDragLineEnded;
            //内部事件
            switchViewModel.DragStarted += OnDragStarted;
            switchViewModel.DragEnded += OnDragEnded;
            switchViewModel.Destroy += OnDiagramItemDestroy;
            switchViewModel.PositionChanged += OnDiagramItemPositionChanged;
            // 三重添加
            DiagramItems.Add(switchView);
            DiagramItemViewModels.Add(switchViewModel);
            _model.CurrentSolutionModel!.DiagramItemModels.Add(switchModel);
            //调整位置
            Canvas.SetLeft(switchView, switchViewModel.CanvasPos.X);
            Canvas.SetTop(switchView, switchViewModel.CanvasPos.Y);
		}
        private void CreateSimulationView(Point pos)
        {
			SimulationModel simulationModel = new SimulationModel(pos);
			SimulationView simulationView = new SimulationView();
			SimulationViewModel simulationViewModel = new SimulationViewModel(simulationView, simulationModel);

			PopupWindow popupWindow = new PopupWindow();
            SimulationConfigViewModel simulationConfigViewModel = new SimulationConfigViewModel(simulationModel) ;
            simulationConfigViewModel.OpenScript += simulationViewModel.OnOpenScript;
            simulationViewModel.OpenScript += OnOpenScript;
            popupWindow.DataContext = new PopupViewModel(popupWindow, simulationConfigViewModel);
            bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}
            //连接上下文
            simulationView.DataContext = simulationViewModel;
            //外部事件
            CanvasClicked += simulationViewModel.OnCanvasClicked;
            KeyDown += simulationViewModel.OnKeyDown;
            DragLineStarted += simulationViewModel.OnDragLineStarted;
            DragLineEnded += simulationViewModel.OnDragLineEnded;
            //内部事件
            simulationViewModel.DragStarted += OnDragStarted;
            simulationViewModel.DragEnded += OnDragEnded;
            simulationViewModel.Destroy += OnDiagramItemDestroy;
            simulationViewModel.PositionChanged += OnDiagramItemPositionChanged;
            // 三重添加
            DiagramItems.Add(simulationView);
            DiagramItemViewModels.Add(simulationViewModel);
            _model.CurrentSolutionModel!.DiagramItemModels.Add(simulationModel);
            //调整位置
            Canvas.SetLeft(simulationView, simulationViewModel.CanvasPos.X);
            Canvas.SetTop(simulationView, simulationViewModel.CanvasPos.Y);
		}
        private void LoadStartView(StartModel startModel)
        {
			StartView startView = new StartView();
			StartViewModel startViewModel = new StartViewModel(startView, startModel);
            //连接上下文
			startView.DataContext = startViewModel;
			//外部事件
			CanvasClicked += startViewModel.OnDeselect;
			KeyDown += startViewModel.OnKeyDown;
			//内部事件
			startViewModel.PositionChanged += OnDiagramItemPositionChanged;
			startViewModel.Destroy += OnDiagramItemDestroy;
            startViewModel.DragStarted += OnDragStarted;
            //双重添加
			DiagramItems.Add(startView);
			DiagramItemViewModels.Add(startViewModel);
            //调整位置
			Canvas.SetLeft(startView, startViewModel.CanvasPos.X);
			Canvas.SetTop(startView, startViewModel.CanvasPos.Y);
		}
        private void LoadStrategySetView(StrategySetModel strategySetModel)
        {
            StrategySetView strategySetView = new StrategySetView();
            StrategySetViewModel strategySetViewModel = new StrategySetViewModel(strategySetView, strategySetModel);
            //连接上下文
            strategySetView.DataContext = strategySetViewModel;
            // 加载数据
            foreach (var strategyModel in strategySetModel.Strategies)
            {
                StrategyView strategyView = new StrategyView();
                StrategyViewModel strategyViewModel = new StrategyViewModel(strategyView, strategyModel);
                //连接上下文
                strategyView.DataContext = strategyViewModel;
				//外部事件
				strategySetViewModel.PositionChanged += strategyViewModel.OnPositionChanged;
				strategySetViewModel.KeyDown += strategyViewModel.OnKeyDown;
                strategySetViewModel.CanvasClicked += strategyViewModel.OnCanvasClicked;
				//内部事件
				strategyViewModel.Dropped += strategySetViewModel.OnChildDrop;
				strategyViewModel.DragStarted += strategySetViewModel.OnDragStarted;
				strategyViewModel.Destroy += strategySetViewModel.OnDeleteChild;
                strategyViewModel.OpenScript += strategySetViewModel.OnOpenScript;
				//二重添加
				strategySetViewModel.StrategyViewModels.Add(strategyViewModel);
                strategySetViewModel.StrategyViews.Add(strategyView);
            }
            //外部事件
            CanvasClicked += strategySetViewModel.OnCanvasClicked;
			KeyDown += strategySetViewModel.OnKeyDown;
			DragLineStarted += strategySetViewModel.OnDragLineStarted;
			DragLineEnded += strategySetViewModel.OnDragLineEnded;
			// 内部事件
			strategySetViewModel.DragStarted += OnDragStarted;
			strategySetViewModel.DragEnded += OnDragEnded;
			strategySetViewModel.Destroy += OnDiagramItemDestroy;
            strategySetViewModel.PositionChanged += OnDiagramItemPositionChanged;
            strategySetViewModel.OpenScript += OnOpenScript;
			// 二重添加
			DiagramItems.Add(strategySetView);
			DiagramItemViewModels.Add(strategySetViewModel);
			// 调整位置
			Canvas.SetLeft(strategySetView, strategySetViewModel.CanvasPos.X);
			Canvas.SetTop(strategySetView, strategySetViewModel.CanvasPos.Y);
		}

		private void OnOpenScript(ViewModelBase diagramItemViewModel)
		{
            string? className = null;
			string folder = _model.CurrentProjectModel!.VSCodeFolder;
            string namespaceName = _model.CurrentProjectModel!.VSCodeFolderName;
            string? content = null;
			if (diagramItemViewModel is StrategyViewModel)
            {
                StrategyViewModel strategyViewModel = (StrategyViewModel)diagramItemViewModel;
                className = strategyViewModel.StrategyModel.StrategyClassName;
                content = ScriptTemplate.StrategyTemplate(namespaceName, className);
			}
            else if(diagramItemViewModel is IfViewModel)
            {
                IfViewModel ifViewModel = (IfViewModel)diagramItemViewModel;
                className = ifViewModel.IfModel.IfModelClassName;
                content = ScriptTemplate.IfTemplate(namespaceName, className);
            }
            else if (diagramItemViewModel is SwitchViewModel)
            {
                SwitchViewModel switchViewModel = (SwitchViewModel)diagramItemViewModel;
                className = switchViewModel.SwitchModel.SwitchModelClassName;
                content = ScriptTemplate.SwitchTemplate(namespaceName, className);
            }
            else if(diagramItemViewModel is SimulationViewModel)
            {
                MessageBox.Show("暂时不支持Simulation脚本");
                return;
            }
            Debug.Assert(className != null);
            Debug.Assert(content != null);
            string filename = $"{className}.cs";
			string directory = $"{folder}/{filename}";
            if (!File.Exists(directory))
            {
                MessageBoxResult result = MessageBox.Show($"脚本文件\"{filename}\"未创建，是否创建？", "脚本未创建", MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }
                File.Create(directory).Close();
                File.WriteAllText(directory, content);
			}
            Cmd.ExecuteCommand($"code -r {folder}");
            Cmd.ExecuteCommand($"code -r {directory}");
			
		}

		private void LoadIfView(IfModel ifModel)
        {
            IfView ifView = new IfView();
            IfViewModel ifViewModel = new IfViewModel(ifView, ifModel);
            //连接上下文
            ifView.DataContext = ifViewModel;
            //外部事件
            CanvasClicked += ifViewModel.OnCanvasClicked;
            KeyDown += ifViewModel.OnKeyDown;
            DragLineStarted += ifViewModel.OnDragLineStarted;
            DragLineEnded += ifViewModel.OnDragLineEnded;
            // 内部事件
            ifViewModel.DragStarted += OnDragStarted;
            ifViewModel.DragEnded += OnDragEnded;
            ifViewModel.Destroy += OnDiagramItemDestroy;
            ifViewModel.PositionChanged += OnDiagramItemPositionChanged;
            ifViewModel.OpenScript += OnOpenScript;
            // 二重添加
            DiagramItems.Add(ifView);
            DiagramItemViewModels.Add(ifViewModel);
            //调整位置
            Canvas.SetLeft(ifView, ifViewModel.CanvasPos.X);
            Canvas.SetTop(ifView, ifViewModel.CanvasPos.Y);
        }
        private void LoadSwitchView(SwitchModel switchModel)
        {
			SwitchView switchView = new SwitchView();
			SwitchViewModel switchViewModel = new SwitchViewModel(switchView, switchModel);
			//连接上下文
			switchView.DataContext = switchViewModel;
            // 属性注册
            foreach(var caseModel in switchModel.CaseModels)
            {
                CaseView caseView = new CaseView();
                CaseViewModel caseViewModel = new CaseViewModel(caseView, caseModel, true);
                //连接上下文
                caseView.DataContext = caseViewModel;
				// 外部事件
				switchViewModel.CanvasClicked += caseViewModel.OnCanvasClicked;
				switchViewModel.KeyDown += caseViewModel.OnKeyDown;
				switchViewModel.PositionChanged += caseViewModel.OnPositionChanged;
				// 内部事件
				caseViewModel.Destroy += switchViewModel.OnCaseDestroy;
				caseViewModel.DragStarted += switchViewModel.OnDragStarted;
                caseViewModel.UpperClicked += switchViewModel.OnCaseUpperClicked;
                caseViewModel.LowerClicked += switchViewModel.OnCaseLowerClicked;
				// 二重添加
				switchViewModel.CaseViews.Add(caseView);
				switchViewModel.CaseViewModels.Add(caseViewModel);
			}
			//外部事件
			CanvasClicked += switchViewModel.OnCanvasClicked;
			KeyDown += switchViewModel.OnKeyDown;
			DragLineStarted += switchViewModel.OnDragLineStarted;
			DragLineEnded += switchViewModel.OnDragLineEnded;
			// 内部事件
			switchViewModel.DragStarted += OnDragStarted;
			switchViewModel.DragEnded += OnDragEnded;
			switchViewModel.Destroy += OnDiagramItemDestroy;
			switchViewModel.PositionChanged += OnDiagramItemPositionChanged;
            switchViewModel.OpenScript += OnOpenScript;
			// 二重添加
			DiagramItems.Add(switchView);
			DiagramItemViewModels.Add(switchViewModel);
			//调整位置
			Canvas.SetLeft(switchView, switchViewModel.CanvasPos.X);
			Canvas.SetTop(switchView, switchViewModel.CanvasPos.Y);
		}
        private void LoadSimulationView(SimulationModel simulationModel)
        {
            SimulationView simulationView = new SimulationView();
            SimulationViewModel simulationViewModel = new SimulationViewModel(simulationView, simulationModel);
            //连接上下文
            simulationView.DataContext = simulationViewModel;
			//外部事件
			CanvasClicked += simulationViewModel.OnCanvasClicked;
			KeyDown += simulationViewModel.OnKeyDown;
			DragLineStarted += simulationViewModel.OnDragLineStarted;
			DragLineEnded += simulationViewModel.OnDragLineEnded;
			//内部事件
			simulationViewModel.DragStarted += OnDragStarted;
			simulationViewModel.DragEnded += OnDragEnded;
			simulationViewModel.Destroy += OnDiagramItemDestroy;
			simulationViewModel.PositionChanged += OnDiagramItemPositionChanged;
            simulationViewModel.OpenScript += OnOpenScript;
            // 二重添加
            DiagramItems.Add(simulationView);
            DiagramItemViewModels.Add(simulationViewModel);
            //调整位置
            Canvas.SetLeft(simulationView, simulationViewModel.CanvasPos.X);
            Canvas.SetTop(simulationView, simulationViewModel.CanvasPos.Y);
		}
    }
}
