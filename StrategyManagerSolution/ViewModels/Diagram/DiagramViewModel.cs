﻿using Contracts.Enums;
using Contracts.MVVMModels;
using StrategyManagerSolution.DiagramMisc;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Utils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public string HintText { get; set; } = "Diagram operation hint messages here!";
        public ObservableCollection<string> ProjectFiles { get; set; } = new();
        public string ProjectName { get; set; } ="";
        public string SelectedFile { get; set; }
        public Command DropCommand { get; }
        public Command ClickCommand { get; }
        public Command MouseLeftButtonUpCommand { get; }
        public Command CanvasLoadedCommand { get; }
        public Command FileSelectionChangedCommand { get; }
        public Command SaveFileCommand { get; }
        public event Action? CanvasClicked;
        public event Action<KeyEventArgs>? KeyDown;
        public event Action? DragLineStarted;
        public event Action? DragLineEnded;
        
        public DiagramViewModel(Model model)
        {
            _model = model;
            DisplayTile0 = new DisplayTileViewModel() { Text = "Start", ImageName = "../../../Images/Start.jpg" };
			DisplayTile1 = new DisplayTileViewModel() { Text = "HS", ImageName = "../../../Images/114514.jpeg" };
            DisplayTile2 = new DisplayTileViewModel() { Text = "PS", ImageName = "../../../Images/fufu.jpg" };
            DisplayTile3 = new DisplayTileViewModel() { Text = "H", ImageName = "../../../Images/53.jpg" };
            DisplayTile4 = new DisplayTileViewModel() { Text = "P", ImageName = "../../../Images/gua.jpg" };
            
            DropCommand = new Command(OnDrop);
            ClickCommand = new Command(OnClick);
            MouseLeftButtonUpCommand= new Command(OnMouseLeftButtonUp);
            CanvasLoadedCommand = new Command(OnCanvasLoaded);
            FileSelectionChangedCommand = new Command(OnFileSelectionChanged);
            SaveFileCommand = new Command(OnSaveFile);
            foreach(var solutionName in _model.CurrentProjectModel!.SolutionNames)
            {
                ProjectFiles.Add(solutionName + ".smsln");
            }
            ProjectName = $"项目'{_model.CurrentProjectModel.ProjectName}'";
            if (_model.CurrentSolutionModel != null)
            {
                SelectedFile = $"文件: {_model.CurrentSolutionModel!.SolutionFileName}";
            }
            else
            {
                SelectedFile = $"未选择文件";
            }
            LoadSolutionFile();
        }

		private void OnSaveFile(object? obj)
		{
            _model.SaveCurrentSolution();
		}

		private void LoadSolutionFile()
        {
            DiagramItems.Clear();
            DiagramItemViewModels.Clear();
            foreach(var diagramItemModel in _model.CurrentSolutionModel!.DiagramItemModels)
            {
                if (diagramItemModel is StartModel)
                {
                    StartView startView = new StartView();
                    StartViewModel startViewModel = new StartViewModel(startView, (StartModel)diagramItemModel);
                    startView.DataContext = startViewModel;
                    startViewModel.PositionChanged += OnDiagramItemPositionChanged;
                    CanvasClicked += startViewModel.OnDeselect;

                    DiagramItems.Add(startView);
                    DiagramItemViewModels.Add(startViewModel);
                    Canvas.SetLeft(startView, startViewModel.CanvasPos.X);
                    Canvas.SetTop(startView, startViewModel.CanvasPos.Y);
                }
                else
                {

                }
            }
        }
		private void OnFileSelectionChanged(object? obj)
		{
			SelectionChangedEventArgs e = (SelectionChangedEventArgs)obj!;
			string filename = (string)e.AddedItems[0]!;
            if (_model.CurrentSolutionModel!= null) 
            {
                _model.SaveCurrentSolution();
            }
            _model.CurrentSolutionModel = Serializer.Deserialize<SolutionModel>(_model.CurrentProjectModel!.ProjectFolder + "/" + filename);
            SelectedFile = $"文件: {filename}";
            OnPropertyChanged(nameof(SelectedFile));
            LoadSolutionFile();
		}

		public void OnDrop(object? obj)
        {
            DragEventArgs e = (obj as DragEventArgs)!;
            DisplayTileViewModel displayTile = (e.Data.GetData(typeof(DisplayTileViewModel)) as DisplayTileViewModel)!;
            if (displayTile != null)
            {
                if (Canvas != null)
                {
                    Point pos = e.GetPosition(Canvas);
                    switch(displayTile.Text)
                    {
                        case "Start":
                            {
                                StartModel startModel = new StartModel(pos);
                                StartView startView = new StartView();
                                StartViewModel startViewModel = new StartViewModel(startView, startModel);
                                startView.DataContext=startViewModel;
                                CanvasClicked += startViewModel.OnDeselect;

								DiagramItems.Add(startView);
                                DiagramItemViewModels.Add(startViewModel);
								_model.CurrentSolutionModel!.DiagramItemModels.Add(startModel);
                                

								Canvas.SetLeft(startView, startViewModel.CanvasPos.X);
                                Canvas.SetTop(startView, startViewModel.CanvasPos.Y);
								break;
							}
                        default:
                            {
                                StrategySetModel strategySetModel = new StrategySetModel("name", StrategySetType.Hierarchical, pos);
                                StrategySetView strategySetView = new StrategySetView();
                                StrategySetViewModel strategySetViewModel = new StrategySetViewModel(strategySetView, strategySetModel);
                                // 属性注册
                                strategySetViewModel.Text = "Strategy Set Name Here!";
                                strategySetViewModel.ImageName = "../../../Images/114514.jpeg";
                                // 拖动连接提醒
                                strategySetViewModel.DragStarted += OnDragStarted;
                                strategySetViewModel.DragEnded += OnDragEnded;
                                // 外部消息注册
                                CanvasClicked += strategySetViewModel.OnCanvasClicked;
                                KeyDown += strategySetViewModel.OnKeyDown;
                                DragLineStarted += strategySetViewModel.OnDragLineStarted;
                                DragLineEnded += strategySetViewModel.OnDragLineEnded;
                                // 连接上下文
                                strategySetView.DataContext= strategySetViewModel;
                                // 装入容器
                                DiagramItems.Add(strategySetView);
                                DiagramItemViewModels.Add(strategySetViewModel);
                                _model.CurrentSolutionModel!.DiagramItemModels.Add(strategySetModel);
                                strategySetModel.Destroy += _model.CurrentSolutionModel!.OnDiagramItemDestroy;

                                Canvas.SetLeft(strategySetView, strategySetViewModel.CanvasPos.X);
                                Canvas.SetTop(strategySetView, strategySetViewModel.CanvasPos.Y);
                                break;
                            }
                    }
                }
                else
                {
                    Console.WriteLine("The item doesn't drop on canvas!");
                    HintText = "The item doesn't drop on canvas!";
                    OnPropertyChanged(nameof(HintText));
				}
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
            Point dragSourcePos = dragSource.DragSourceView.TranslatePoint(new Point(0, 0), Canvas);
			_dragInfo.StartPos = new Point(dragSourcePos.X + dragSource.Offset.X, dragSourcePos.Y + dragSource.Offset.Y);
			Console.WriteLine($"Dragged at ({_dragInfo.StartPos.X}, {_dragInfo.StartPos.Y})");
            DragLineStarted?.Invoke();
        }
        void OnDragEnded(IDragDestination dragDestination)
        {
            if (_dragInfo.Dragging)
            {
                _dragInfo.DragDestination = dragDestination;
                Point dragDestinationPos = dragDestination.DragDestinationView.TranslatePoint(new Point(0,0), Canvas);
                _dragInfo.EndPos = new Point(dragDestinationPos.X + dragDestination.Offset.X, dragDestinationPos.Y + dragDestination.Offset.Y);
                if (_dragInfo.DragSource!.LineLeaving == null 
                    && _dragInfo.DragDestination!.LineEntering == null)
                {
                    ConnectionLine line = new ConnectionLine(new Line
                    {
                        X1 = _dragInfo.StartPos.X,
                        X2 = _dragInfo.EndPos.X,
                        Y1 = _dragInfo.StartPos.Y,
                        Y2 = _dragInfo.EndPos.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 5
                    },
                    _dragInfo.DragSource,
                    _dragInfo.DragDestination);
                    // 内部注册外部事件
                    KeyDown += line.OnKeyDown;
                    CanvasClicked += line.OnDeselect;
                    // 外部注册内部事件
                    line.Destroyed += OnConnectionLineDestroyed;

                    DiagramItems.Add(line.Line);
                    _dragInfo.DragSource!.LineLeaving = line;
                    _dragInfo.DragDestination!.LineEntering = line;
                    line.Destroyed += _dragInfo.DragSource!.OnLineLeavingDestroyed;
                    line.Destroyed += _dragInfo.DragDestination!.OnLineEnteringDestroyed;
                    _dragInfo.DragSource.LinkingTo = _dragInfo.DragDestination;
                    _dragInfo.DragDestination.LinkingFrom = _dragInfo.DragSource;
                    // 外部订阅被连接物体的事件
                    _dragInfo.DragSource.PositionChanged += OnConnectedItemPositionChanged;
                    _dragInfo.DragDestination.PositionChanged += OnConnectedItemPositionChanged;

                    _dragInfo.Clear();
                }
                else
                {
                    Console.WriteLine("你已经连接过一条线了！");
					HintText = "你已经连接过一条线了！";
					OnPropertyChanged(nameof(HintText));
				}
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
        void OnDiagramItemPositionChanged(ViewModelBase diagramItem)
        {
            if (diagramItem is IHasPosition)
            {
                IHasPosition hasPosition = (IHasPosition)diagramItem;
                Point pos = hasPosition.PositionView.TranslatePoint(new Point(0, 0), Canvas);
                hasPosition.CanvasPos = pos;
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
    }
}