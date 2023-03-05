using Microsoft.Win32;
using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Contracts.Communication;
using StrategyManagerSolution.Views;
using StrategyManagerSolution.ViewModels.Form;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace StrategyManagerSolution.ViewModels
{
	internal class TestViewModel:ViewModelBase
	{
		public Model _model;
		public ImageSource GraphicsImageSource { get; } = new BitmapImage(new Uri("../../../Images/nulltest.jpg", UriKind.Relative));
        public string ConsoleOutputText { get; set; } = "壅壑壑";
        public string ConsoleInputText { get; set; } = "壅壑壑啊";
        public Command ChooseAssemblyCommand { get; }
        public Command NavigateToDiagramCommand { get; }
		public Command InputKeyDownCommand { get; }
        public event Action? NavigateToDiagram;
        public TestViewModel(Model model)
        {
            _model = model;
            double a = GraphicsImageSource.Width;
            ChooseAssemblyCommand = new Command(OnChooseAssembly);
            NavigateToDiagramCommand = new Command((_) => NavigateToDiagram?.Invoke());
			InputKeyDownCommand = new Command(OnInputKeyDown);
        }
		private void ProcessReceivedMessage()
		{
			var result = _model.TestProcess!.StandardOutput.ReadLineAsync();
			if (!result.Wait(1000))
			{
				throw new Exception("超时");
			}
			string message = result.Result!;
			string[] segments = message.Split();
			Console.WriteLine("Received message: " + message);
			if (message.Contains(TextConvention.OK))
				return;
			switch(segments[0])
			{
					
			}
		}
		private void OnInputKeyDown(object? obj)
		{
			if (_model.TestProcess == null || _model.TestProcess.HasExited)
				return;
			KeyEventArgs e = (KeyEventArgs)obj!;
			if (e.Key == Key.Return)
			{
				_model.TestProcess.StandardInput.WriteLine(TextConvention.ConsoleInput + " " + ConsoleInputText);
				ConsoleOutputText += ConsoleInputText + "\n";
				ConsoleInputText = "";
				OnPropertyChanged(nameof(ConsoleInputText));
				OnPropertyChanged(nameof(ConsoleOutputText));
				ProcessReceivedMessage();
			}
		}
		private void ConfigureTestProcess()
		{
			Task task = new Task(async () =>
			{
				try
				{
					_model.TestProcess!.StandardInput.WriteLine(TextConvention.ProjSlnPath + " " + _model.CurrentProjectModel!.VSCodeFolder + "/bin/Debug/net6.0-windows/" + _model.CurrentProjectModel.ProjectName + ".dll");
					ProcessReceivedMessage();
					_model.TestProcess.StandardInput.WriteLine(TextConvention.FuncProviderFullName + " " + _model.CurrentProjectModel!.VSCodeFolderName + "." + _model.CurrentProjectModel.ProjectName + "ProjSlnFuncProvider");
					ProcessReceivedMessage();
					_model.TestProcess.StandardInput.WriteLine(TextConvention.TestAssemblyPath + " " + _model.CurrentProjectModel.TestAssemblyPath);
					ProcessReceivedMessage();
					_model.TestProcess.StandardInput.WriteLine(TextConvention.TestAssemblyClassFullName + " " + _model.CurrentProjectModel.TestAssemblyClassFullName);
					ProcessReceivedMessage();
				}
				catch(Exception e)
				{
					MessageBox.Show("发生了异常: " + e.Message);
				}

			});
			task.Start();
		}
		private void OnChooseAssembly(object? obj)
		{
			PopupWindow popupWindow = new PopupWindow();
			TestAssemblyConfigViewModel testAssemblyConfigViewModel = new TestAssemblyConfigViewModel(_model);
			popupWindow.DataContext = new PopupViewModel(popupWindow, testAssemblyConfigViewModel);
			bool? result = popupWindow.ShowDialog();
			if (result == null || !result.Value)
			{
				return;
			}
			if (_model.TestProcess != null && !_model.TestProcess.HasExited)
			{
				_model.TestProcess.Kill();
			}
			_model.TestProcess = new Process();
			_model.TestProcess.StartInfo.FileName = "../../../../StrategyTester/bin/Debug/net6.0-windows/StrategyTester.exe";
			_model.TestProcess.StartInfo.CreateNoWindow = false;
			_model.TestProcess.StartInfo.RedirectStandardInput = true;
			_model.TestProcess.StartInfo.RedirectStandardOutput = true;
			_model.TestProcess.Start();

			//_model.TestProcess.StandardInput.WriteLine(TextConvention.Integrity);
			//string? str = _model.TestProcess.StandardOutput.ReadLine();
			//Console.WriteLine(str);

			try
			{
				ConfigureTestProcess();
			}
			
			catch(AggregateException ae)
			{
				foreach(var e in  ae.InnerExceptions)
				{
					MessageBox.Show("发生了异常: " + e.Message);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("发生了异常: " + e.Message);
			}
		}
	}
}
