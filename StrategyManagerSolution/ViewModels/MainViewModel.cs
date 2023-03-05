using Contracts;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.ViewModels.Diagram;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StrategyManagerSolution.ViewModels
{
	internal class MainViewModel : ViewModelBase
	{
		private Model _model;
		private MainWindow _mainWindow;
		private ViewModelBase? _currentViewModel;
		public ViewModelBase? CurrentViewModel
		{
			get { return _currentViewModel; }
			set
			{
				if (_currentViewModel != value)
				{
					_currentViewModel = value;
					OnPropertyChanged(nameof(CurrentViewModel));
				}
			}
		}
		public Command KeyDownCommand { get; }
		public Command NavigateToStartMenuCommand { get; }
		public Command NavigateToDiagramCommand { get; }
		public Command BuildSolutionCommand { get; }
		public Command NavigateToSettingsCommand { get; }
		public Command NavigateToTestCommand { get; }
		public Command ClosingCommand { get; }
		public event Action<KeyEventArgs>? KeyDown;
		public event Action? Closing;
		public void NavigateToDiagram()
		{
			if (_model.CurrentProjectModel == null)
			{
				MessageBox.Show("还未指定打开的项目文件!","错误", MessageBoxButton.OK);
				return;
			}
			DiagramViewModel diagramViewModel = new DiagramViewModel(_model);
			KeyDown += diagramViewModel.OnKeyDown;
			Closing += diagramViewModel.OnWindowClosing;
			CurrentViewModel = diagramViewModel;
		}
		public void NavigateToStartMenu()
		{
			StartMenuViewModel startMenuViewModel = new StartMenuViewModel(_model);
			startMenuViewModel.NavigateToDiagram += NavigateToDiagram;
			startMenuViewModel.NavigateToCreateProject += NavigateToCreateProject;
			CurrentViewModel = startMenuViewModel;
		}
		public void NavigateToCreateProject()
		{
			if (_model.ProgramData.ProjectReferenceDirectory == null)
			{
				MessageBox.Show("尚未指定SMContracts项目引用路径。\n请到【文件】->【选项】->SMContracts项目引用路径填写相关信息", "错误", MessageBoxButton.OK);
				return;
			}
			CreateProjectViewModel createProjectViewModel = new CreateProjectViewModel(_model);
			createProjectViewModel.NavigateToStartMenu += NavigateToStartMenu;
			createProjectViewModel.NavigateToDiagram += NavigateToDiagram;
			CurrentViewModel = createProjectViewModel;
		}
		public void NavigateToSettings()
		{
			SettingsViewModel settingsViewModel = new SettingsViewModel(_model);
			settingsViewModel.NavigateToStartMenu += NavigateToStartMenu;
			settingsViewModel.NavigateToDiagram += NavigateToDiagram;
			CurrentViewModel = settingsViewModel;
		}
		public void NavigateToBuildSolution()
		{
			BuildSolutionViewModel buildSolutionViewModel = new BuildSolutionViewModel(_model);
			buildSolutionViewModel.NavigateToDiagram += NavigateToDiagram;
			buildSolutionViewModel.NavigateToTest += NavigateToTest;
			CurrentViewModel = buildSolutionViewModel;
		}
		public void NavigateToTest()
		{
			if (_model.CurrentProjectModel == null)
			{
				MessageBox.Show("请先选择项目!");
				return;
			}
			TestViewModel testViewModel = new TestViewModel(_model);
			CurrentViewModel = testViewModel;
		}
		public MainViewModel(Model model, MainWindow mainWindow)
		{
			_model = model;
			_mainWindow = mainWindow;
			NavigateToStartMenu();
			KeyDownCommand = new Command(OnKeyDown);
			NavigateToStartMenuCommand = new Command((_)=>NavigateToStartMenu());
			NavigateToDiagramCommand = new Command((_)=> NavigateToDiagram());
			BuildSolutionCommand = new Command(OnBuildSolution);
			NavigateToSettingsCommand = new Command((_) => NavigateToSettings());
			NavigateToTestCommand = new Command((_) => NavigateToTest());
			ClosingCommand = new Command(OnClosing);
		}

		private void OnClosing(object? obj)
		{
			
			_model.SaveProgramData();
			if (_model.CurrentSolutionModel != null)
				_model.SaveCurrentSolution();
			if (_model.CurrentProjectModel != null)
				_model.SaveProject();
			Closing?.Invoke();
		}

		private void OnBuildSolution(object? obj)
		{
			if (_model.CurrentProjectModel == null)
			{
				MessageBox.Show("未指定项目文件!", "错误", MessageBoxButton.OK);
				return;
			}
			NavigateToBuildSolution();
			//string result = AssemblyGenerator.GenerateCode(_model.CurrentProjectModel!);
			//File.WriteAllText(_model.CurrentProjectModel.VSCodeFolder + "/assembly.cs", result);
			//MessageBox.Show("生成成功!\n");
		}

		public void OnKeyDown(object? obj)
		{
			KeyEventArgs e = (obj as KeyEventArgs)!;
			KeyDown?.Invoke(e);
		}
	}
}
