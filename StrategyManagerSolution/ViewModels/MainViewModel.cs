using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.ViewModels.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategyManagerSolution.ViewModels
{
	internal class MainViewModel : ViewModelBase
	{
		private Model _model;
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
		public event Action<KeyEventArgs>? KeyDown;
		public void NavigateToDiagram()
		{
			DiagramViewModel diagramViewModel = new DiagramViewModel(_model);
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
		public MainViewModel(Model model)
		{
			_model = model;
			NavigateToStartMenu();
			KeyDownCommand = new Command(OnKeyDown);
		}
		public void OnKeyDown(object? obj)
		{
			KeyEventArgs e = (obj as KeyEventArgs)!;
			KeyDown?.Invoke(e);
		}
	}
}
