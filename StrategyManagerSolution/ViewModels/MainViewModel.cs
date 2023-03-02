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
		public event Action<KeyEventArgs>? KeyDown;
		public void NavigateToDiagram()
		{
			if (_model.CurrentProjectModel == null)
			{
				MessageBox.Show("还未指定打开的项目文件!","错误", MessageBoxButton.OK);
				return;
			}
			DiagramViewModel diagramViewModel = new DiagramViewModel(_model, _mainWindow);
			KeyDown += diagramViewModel.OnKeyDown;
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
			CreateProjectViewModel createProjectViewModel = new CreateProjectViewModel(_model);
			createProjectViewModel.NavigateToStartMenu += NavigateToStartMenu;
			createProjectViewModel.NavigateToDiagram += NavigateToDiagram;
			CurrentViewModel = createProjectViewModel;
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
		}

		private void OnBuildSolution(object? obj)
		{
			if (_model.CurrentProjectModel == null)
			{
				MessageBox.Show("未指定项目文件!", "错误", MessageBoxButton.OK);
				return;
			}
			string result = AssemblyGenerator.GenerateCode(_model.CurrentProjectModel!);
			File.WriteAllText(_model.CurrentProjectModel.VSCodeFolder + "/assembly.cs", result);
		}

		public void OnKeyDown(object? obj)
		{
			KeyEventArgs e = (obj as KeyEventArgs)!;
			KeyDown?.Invoke(e);
		}
	}
}
