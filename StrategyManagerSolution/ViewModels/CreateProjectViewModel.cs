using Microsoft.Win32;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace StrategyManagerSolution.ViewModels
{
	internal class CreateProjectViewModel:ViewModelBase
	{
		private Model _model;
		public string ProjectName { get; set; } = "";
		public string Location { get; set; } = "";
		public string VSCodeFolderName { get; set; } = "";
		public Command CreateCommand { get; }
		public Command CancelCommand { get; }
		public Command SelectFolderCommand { get; }
		public event Action? NavigateToStartMenu;
		public event Action? NavigateToDiagram;
		public CreateProjectViewModel(Model model)
		{
			_model = model;
			CreateCommand = new(OnCreate);
			CancelCommand = new(OnCancel);
			SelectFolderCommand = new(OnSelectFolder);
		}
		private void OnCreate(object? obj)
		{
			_model.CreateProject(ProjectName, Location, VSCodeFolderName);
			_model.RecentProjects.Insert(0, new RecentProject(ProjectName + ".smproj", $"{Location}/{ProjectName}/{ProjectName}.smproj", DateTime.Now));
			_model.SaveProgramData();
			NavigateToDiagram?.Invoke();
		}
		private void OnCancel(object? obj)
		{
			NavigateToStartMenu?.Invoke();
		}
		private void OnSelectFolder(object? obj)
		{
			using var dialog = new FolderBrowserDialog
			{
				Description = "选择项目位置",
				UseDescriptionForTitle = true,
				ShowNewFolderButton = true
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Location = dialog.SelectedPath;
				OnPropertyChanged(nameof(Location));
			}
		}
	}
}
