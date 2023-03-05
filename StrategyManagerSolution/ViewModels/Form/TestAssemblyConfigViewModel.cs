using Microsoft.Win32;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
    internal class TestAssemblyConfigViewModel:ViewModelBase
    {
        private Model _model;
        public string TestAssemblyPathPrompt { get; } = "测试程序集路径: ";
        public string? TestAssemblyPath
        {
            get { return _model.CurrentProjectModel!.TestAssemblyPath; }
            set { _model.CurrentProjectModel!.TestAssemblyPath = value; }
        }
        public string TestAssemblyClassFullNamePrompt { get; } = "测试程序集类全名: (命名空间.类名)";
        public string? TestAssemblyClassFullName
        {
            get { return _model.CurrentProjectModel!.TestAssemblyClassFullName; }
            set { _model.CurrentProjectModel!.TestAssemblyClassFullName = value; }
        }
        public Command ChooseFileCommand { get; }
        public TestAssemblyConfigViewModel(Model model)
        {
            _model = model;
            ChooseFileCommand = new Command(OnChooseFile);
        }

		private void OnChooseFile(object? obj)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = "Assembly DLL|*.dll";
			bool? selected = openFileDialog.ShowDialog();
			if (selected == null || !selected.Value)
			{
				return;
			}
            TestAssemblyPath = openFileDialog.FileName;
            OnPropertyChanged(nameof(TestAssemblyPath));
		}
	}
}
