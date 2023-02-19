using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views.Diagram;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.MVVMModels;
using StrategyManagerSolution.Adorners;
using System.Windows.Forms;
using System.Windows.Documents;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal class StartViewModel:ViewModelBase
	{
		public StartView View { get;set; }
		private StartModel _startModel;
		public MoveAdorner MoveAdorner { get; set; }
		public Command LoadedCommand { get; }
		public Point CanvasPos
		{
			get { return _startModel.CanvasPos; }
			set { _startModel.CanvasPos = value; }
		}
		public StartViewModel(StartView startView, StartModel startModel)
		{
			View = startView;
			_startModel = startModel;
			MoveAdorner = new MoveAdorner(View, 140,70);
			LoadedCommand = new(OnLoaded);
		}

		private void OnLoaded(object? obj)
		{
			SetUpAdorner();
		}

		public void SetUpAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(View);
			if (adornerLayer == null)
			{
				Console.WriteLine("Failed to get adorner layer!");
			}
			else
			{
				adornerLayer.Add(MoveAdorner);
			}
		}
	}
}
