using Microsoft.Win32;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StrategyManagerSolution.ViewModels
{
	internal class StartMenuViewModel:ViewModelBase
	{
		private Model _model;
		public ObservableCollection<RecentProjectViewModel> RecentProjectViewModels { get; }
		public Command OpenRecentCommand { get; }
		public Command OpenProjectCommand { get; }
		public Command CreateProjectCommand { get; }
		public Command ClearRecordCommand { get; }
		public event Action? NavigateToDiagram;
		public event Action? NavigateToCreateProject;
		public StartMenuViewModel(Model model)
		{
			_model = model;
			RecentProjectViewModels = new();
			foreach(var recentProject in _model.RecentProjects)
			{
				RecentProjectViewModels.Add(new(recentProject));
			}
			OpenRecentCommand = new(OnOpenRecent);
			OpenProjectCommand = new(OnOpenProject);
			CreateProjectCommand = new(OnCreateProject);
			ClearRecordCommand = new(OnClearRecord);
		}
		private void OnClearRecord(object? obj)
		{
			RecentProjectViewModels.Clear();
			_model.RecentProjects.Clear();
		}
		private void OnOpenRecent(object? obj)
		{
			Console.WriteLine("Open recent called.");
			SelectionChangedEventArgs e = (SelectionChangedEventArgs)obj!;
			if (e.AddedItems.Count == 0)
				return;
			RecentProjectViewModel recentProjectViewModel = (RecentProjectViewModel)e.AddedItems[0]!;
			_model.OpenProject(recentProjectViewModel.Directory);
			NavigateToDiagram?.Invoke();
			e.Handled = true;
		}
		private void OnOpenProject(object? obj)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Strategy Manager Project|*.smproj";
			bool? selected = openFileDialog.ShowDialog();
			if (selected == true)
			{
				_model.OpenProject(openFileDialog.FileName);
				NavigateToDiagram?.Invoke();
			}
		}
		private void OnCreateProject(object? obj)
		{
			NavigateToCreateProject?.Invoke();
		}
	}
}
