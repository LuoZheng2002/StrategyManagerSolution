using Contracts.BaseClasses;
using Contracts.Communication;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Utils;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.ViewModels
{
	internal class BuildSolutionViewModel:ViewModelBase
	{
		private Model _model;
		public BuildSolutionView? View { get; set; }
		private string? _assemblyError;
		private string? _compileError;
		private string? _integrityError;
		public ImageSource AssemblyImageSource { get; } = new BitmapImage(new Uri("../../../Images/assembly.jpg", UriKind.Relative));
		public ImageSource CompileImageSource { get; } = new BitmapImage(new Uri("../../../Images/compile.jpg", UriKind.Relative));
		public ImageSource IntegrityImageSource { get; } = new BitmapImage(new Uri("../../../Images/integrity.jpg", UriKind.Relative));
		public IStatus AssemblyStatusViewModel { get; set; }
		public IStatus CompileStatusViewModel { get; set; }	
		public IStatus IntegrityStatusViewModel { get; set; }
		public Visibility TryVisibility { get; set; } = Visibility.Hidden;
		public Command TryCommand { get; }
		public Command BuildCommand { get; }
		public Command LoadedCommand { get; }
		public Command NavigateToDiagramCommand { get; }
		public event Action? NavigateToDiagram;
		public event Action? NavigateToTest;
        public BuildSolutionViewModel(Model model)
        {
            _model = model;
			// activate images using bugs
			double a = AssemblyImageSource.Width;
			double b = CompileImageSource.Width;
			double c = IntegrityImageSource.Width;
			AssemblyStatusViewModel = new UnstartedViewModel();
			CompileStatusViewModel = new UnstartedViewModel();
			IntegrityStatusViewModel = new UnstartedViewModel();
			BuildCommand = new Command(OnBuild);
			NavigateToDiagramCommand = new Command((_) => NavigateToDiagram?.Invoke());
			TryCommand = new Command(OnTry);
			LoadedCommand = new Command(OnLoaded);
        }
		private void OnLoaded(object? obj)
		{
			RoutedEventArgs e = (obj as RoutedEventArgs)!;
			View = (BuildSolutionView)e.Source;
		}
		private bool DoAssembly()
		{

			bool succeeded = AssemblyGenerator.GenerateCode(_model.CurrentProjectModel!, out string result, out _assemblyError);
			File.WriteAllText(_model.CurrentProjectModel!.VSCodeFolder + "/assembly.cs", result);
			
			return succeeded;
			//MessageBox.Show("生成成功!\n");
		}
		private bool DoCompile()
		{
			// Cmd.ExecuteCommand("", _model.CurrentProjectModel!.VSCodeFolder);
			string buildBatDirectory = $"{_model.CurrentProjectModel!.VSCodeFolder}/build.bat";
			File.WriteAllText(buildBatDirectory, "set DOTNET_CLI_UI_LANGUAGE=en\ndotnet build");
			string resultStr = Cmd.ExecuteCommand("build.bat", _model.CurrentProjectModel!.VSCodeFolder);
			File.Delete(buildBatDirectory);
			bool succeeded = resultStr.Contains("Build succeeded.");
			if (succeeded)
			{
				CompileStatusViewModel = new SuccessViewModel();
			}
			else
			{
				_compileError = "来自编译命令行的信息: \n" + resultStr;
			}
			return succeeded;
		}
		private bool DoIntegrityCheck()
		{
			bool succeeded = false;
			if (_model.TestProcess !=null && !_model.TestProcess.HasExited)
			{
				_model.TestProcess.Kill();
			}
			_model.TestProcess = new Process();
			_model.TestProcess.StartInfo.FileName = "../../../../StrategyTester/bin/Debug/net6.0-windows/StrategyTester.exe";
			_model.TestProcess.StartInfo.CreateNoWindow = false;
			_model.TestProcess.StartInfo.RedirectStandardInput = true;
			_model.TestProcess.StartInfo.RedirectStandardOutput = true;
			_model.TestProcess.Start();
			_model.TestProcess.StandardInput.WriteLine(TextConvention.ProjSlnPath + " " + _model.CurrentProjectModel!.VSCodeFolder + "/bin/Debug/net6.0-windows/" + _model.CurrentProjectModel.ProjectName + ".dll");
			string str = _model.TestProcess.StandardOutput.ReadLine();
			Console.WriteLine(str);
			_model.TestProcess.StandardInput.WriteLine(TextConvention.FuncProviderFullName + " " + _model.CurrentProjectModel!.VSCodeFolderName + "." + _model.CurrentProjectModel.ProjectName + "ProjSlnFuncProvider");
			str = _model.TestProcess.StandardOutput.ReadLine();
			Console.WriteLine(str);
			_model.TestProcess.StandardInput.WriteLine(TextConvention.Integrity);
			str = _model.TestProcess.StandardOutput.ReadLine();
			Console.WriteLine(str);
			if (str!.Contains(TextConvention.OK))
				succeeded = true;

			_model.TestProcess.Kill();
			_model.TestProcess = null;
			return succeeded;
		}
		private void OnTry(object? obj)
		{
			NavigateToTest?.Invoke();
		}
		private void BuildAsync()
		{
			bool assemblySuccess = false;
			bool compileSuccess = false;
			bool integritySuccess = false;
			Application.Current.Dispatcher.Invoke(() =>
			{
				BuildCommand._canExecute = false;
				NavigateToDiagramCommand._canExecute = false;
				BuildCommand.OnCanExecuteChanged();
				NavigateToDiagramCommand.OnCanExecuteChanged();
				TryVisibility = Visibility.Hidden;
				AssemblyStatusViewModel = new UnstartedViewModel();
				CompileStatusViewModel = new UnstartedViewModel();
				IntegrityStatusViewModel = new UnstartedViewModel();
				OnPropertyChanged(nameof(TryVisibility));
				OnPropertyChanged(nameof(AssemblyStatusViewModel));
				OnPropertyChanged(nameof(AssemblyStatusViewModel));
				OnPropertyChanged(nameof(CompileStatusViewModel));
				OnPropertyChanged(nameof(IntegrityStatusViewModel));
			});

			assemblySuccess = DoAssembly();
			
			Application.Current.Dispatcher.Invoke(() =>
			{

				if (!assemblySuccess)
				{
					FailViewModel failViewModel = new FailViewModel();
					failViewModel.ViewDetail += OnViewAssemblyError;
					AssemblyStatusViewModel = failViewModel;
					OnPropertyChanged(nameof(AssemblyStatusViewModel));
				}
				else
				{
					AssemblyStatusViewModel = new SuccessViewModel();
					OnPropertyChanged(nameof(AssemblyStatusViewModel));
				}
			});
			if (assemblySuccess)
			{
				compileSuccess = DoCompile();
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (!compileSuccess)
					{
						FailViewModel failViewModel = new FailViewModel();
						failViewModel.ViewDetail += OnViewCompileError;
						CompileStatusViewModel = failViewModel;
						OnPropertyChanged(nameof(CompileStatusViewModel));
					}
					else
					{
						CompileStatusViewModel = new SuccessViewModel();
						OnPropertyChanged(nameof(CompileStatusViewModel));
					}
				});
			}
			if (compileSuccess)
			{
				integritySuccess = DoIntegrityCheck();
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (!integritySuccess)
					{
						FailViewModel failViewModel = new FailViewModel();
						failViewModel.ViewDetail += OnViewIntegrityError;
						IntegrityStatusViewModel = failViewModel;
						OnPropertyChanged(nameof(IntegrityStatusViewModel));
					}
					else
					{
						IntegrityStatusViewModel = new SuccessViewModel();
						OnPropertyChanged(nameof(IntegrityStatusViewModel));
					}
				});
			}
			if (integritySuccess)
			{
				TryVisibility = Visibility.Visible;
				OnPropertyChanged(nameof(TryVisibility));
			}
			Application.Current.Dispatcher.Invoke(() =>
			{
				BuildCommand._canExecute = true;
				NavigateToDiagramCommand._canExecute = true;
				BuildCommand.OnCanExecuteChanged();
				NavigateToDiagramCommand.OnCanExecuteChanged();
				if (assemblySuccess && compileSuccess && integritySuccess)
				{
					MessageBox.Show("恭喜! 生成解决方案成功! ");
				}
				else
				{
					MessageBox.Show("亲爱的开发者，非常高兴你能走到这一步，你的努力是值得被尊敬的，然而一个不可否认的事实是，你的设计出现了一定的问题，导致了解决方案生成的失败。请排查好错误后再次尝试。祝你好运！");
				}
			});
			
			
		}
		private void OnBuild(object? obj)
		{
			Task task = new Task(BuildAsync);
			task.Start();
		}

		public void OnViewAssemblyError()
		{
			MessageBox.Show(_assemblyError);
		}
		public void OnViewCompileError()
		{
			MessageBox.Show(_compileError);
		}
		public void OnViewIntegrityError()
		{
			MessageBox.Show(_integrityError);
		}
	}
}
