using Microsoft.Win32;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels
{
	internal class SettingsViewModel: ViewModelBase
	{
		private Model _model;
		public string ProjectReferenceDirectory
		{
			get { return _model.ProgramData.ProjectReferenceDirectory; }
			set { _model.ProgramData.ProjectReferenceDirectory = value; }
		}
		public Command ConfirmCommand { get; }
		public Command SelectProjectReferenceCommand { get; }
		public event Action? NavigateToDiagram;
		public event Action? NavigateToStartMenu;
        public SettingsViewModel(Model model)
        {
			_model = model;
			ConfirmCommand = new Command(OnConfirm);
			SelectProjectReferenceCommand = new Command(OnSelectProjectReference);
        }

		private void OnSelectProjectReference(object? obj)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Visual C# Project|*.csproj";
			bool? selected = openFileDialog.ShowDialog();
			if (selected == true)
			{
				ProjectReferenceDirectory = openFileDialog.FileName;
				OnPropertyChanged(nameof(ProjectReferenceDirectory));
			}
		}

		private void OnConfirm(object? obj)
		{
			if (_model.CurrentProjectModel == null)
			{
				NavigateToStartMenu?.Invoke();
			}
			else
			{
				NavigateToDiagram?.Invoke();
			}
		}
    }
}
