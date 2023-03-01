using StrategyManagerSolution.Models;
using StrategyManagerSolution.Utils;
using StrategyManagerSolution.ViewModels;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StrategyManagerSolution
{
	public partial class App : Application
	{
		private Model _model;
		private MainWindow _mainWindow;
		public App()
		{
			_model = new Model();
			_mainWindow= new MainWindow();
			_mainWindow.DataContext = new MainViewModel(_model,_mainWindow);
			_mainWindow.Show();
		}
	}
}
