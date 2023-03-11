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
using Contracts;
using SMContracts;
using Emgu.CV.Structure;
using Emgu.CV;

namespace StrategyManagerSolution.ViewModels
{
	internal class TestViewModel:ViewModelBase
	{
		public Model _model;
		public ImageSource GraphicsImageSource { get; set; } = new BitmapImage(new Uri("../../../Images/nulltest.jpg", UriKind.Relative));
        public string ConsoleOutputText { get; set; } = "";
        public string ConsoleInputText { get; set; } = "";
        public Command ChooseAssemblyCommand { get; }
        public Command NavigateToDiagramCommand { get; }
		public Command InputKeyDownCommand { get; }
		public Command LeftButtonDownCommand { get; }
		public Command RightButtonDownCommand { get; }
        public event Action? NavigateToDiagram;
        public TestViewModel(Model model)
        {
            _model = model;
            double a = GraphicsImageSource.Width;
            ChooseAssemblyCommand = new Command(OnChooseAssembly);
            NavigateToDiagramCommand = new Command((_) => NavigateToDiagram?.Invoke());
			InputKeyDownCommand = new Command(OnInputKeyDown);
			LeftButtonDownCommand = new Command(OnLeftButtonDown);
			RightButtonDownCommand = new Command(OnRightButtonDown);
        }

		private void OnRightButtonDown(object? obj)
		{
			if (_model.TestProcess == null || _model.TestProcess.HasExited)
			{
				return;
			}
			MouseEventArgs e = (MouseEventArgs)obj!;
			Image image = (Image)e.Source;
			Point pos = e.GetPosition(image);
			double x = pos.X / image.ActualWidth;
			double y = pos.Y / image.ActualHeight;
			_model.TestProcess.StandardInput.WriteLine(TextConvention.RightButtonDown + " " + x.ToString() + " " + y.ToString());
			Console.WriteLine($"点击了({x}, {y})");
		}

		private void OnLeftButtonDown(object? obj)
		{
			if (_model.TestProcess == null || _model.TestProcess.HasExited)
			{
				return;
			}
			MouseEventArgs e = (MouseEventArgs)obj!;
			Image image = (Image)e.Source;
			Point pos = e.GetPosition(image);
			double x = pos.X / image.ActualWidth;
			double y = pos.Y / image.ActualHeight;
			_model.TestProcess.StandardInput.WriteLine(TextConvention.LeftButtonDown + " " + x.ToString() + " " + y.ToString());
			Console.WriteLine($"点击了({x}, {y})");

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
					Application.Current.Dispatcher.Invoke(() =>
						{
							// 开始测试
							_model.TestProcess.StandardInput.WriteLine(TextConvention.Start);
							ProcessReceivedMessage();
							Task task = new Task(ProcessFeedback);
							task.Start();
						});
				}
				catch(Exception e)
				{
					MessageBox.Show("发生了异常: " + e.Message);
				}

			});
			task.Start();
		}
		private void ProcessFeedback()
		{
			while(_model.TestProcess!=null && !_model.TestProcess.HasExited)
			{
				try
				{
					string str = _model.TestProcess.StandardOutput.ReadLine()!;
					if (str == null)
						return;
					string[] segments = str.Split(" ");
					switch (segments[0])
					{
						case TextConvention.SendMessage:
							{
								string message = str.Replace(TextConvention.SendMessage + " ", "");
								ConsoleOutputText += message + "\n";
								OnPropertyChanged(nameof(ConsoleOutputText));
								break;
							}
						case TextConvention.UpdateImage:
							{
								string message = str.Replace(TextConvention.UpdateImage + " ", "");
								byte[][][] jaggedData = Serializer.DeserializeString<byte[][][]>(message);
								byte[,,] data = jaggedData.To3D();
								Image<Bgr, byte> image = new Image<Bgr, byte>(data);
								Application.Current.Dispatcher.Invoke(() =>
								{
									GraphicsImageSource = BitmapConverter.Bitmap2ImageSource(image.ToBitmap());
									OnPropertyChanged(nameof(GraphicsImageSource));
								});

								break;
							}
						case TextConvention.Error:
							{
								string message = str.Replace(TextConvention.Error + " ", "");
								throw new Exception($"来自承载tester程序的错误: {message}");
							}
						default:
							{
								throw new Exception("unexpected feedback type");
							}
					}
				}
				catch(Exception e)
				{
					MessageBox.Show(e.Message);
				}
			}
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
				Console.WriteLine("Process killed");
				_model.TestProcess.Kill();
			}
			_model.TestProcess = new Process();
			_model.TestProcess.StartInfo.FileName = "../../../../StrategyTester/bin/Debug/net6.0-windows/StrategyTester.exe";
			_model.TestProcess.StartInfo.CreateNoWindow = false;
			_model.TestProcess.StartInfo.RedirectStandardInput = true;
			_model.TestProcess.StartInfo.RedirectStandardOutput = true;
			_model.TestProcess.Start();

			
			try
			{
				ConfigureTestProcess();
				
			}
			catch (Exception e)
			{
				MessageBox.Show("发生了异常: " + e.Message);
			}
		}
	}
}
