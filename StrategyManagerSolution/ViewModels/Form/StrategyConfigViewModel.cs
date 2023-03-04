using Contracts.MVVMModels;
using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class StrategyConfigViewModel:ViewModelBase
	{
		private StrategyModel _strategyModel;
		public string StrategyNamePrompt { get; } = "策略名称: ";
		public string StrategyName
		{
			get { return _strategyModel.StrategyName; }
			set { _strategyModel.StrategyName = value;}
		}
		public string StrategyModelClassNamePrompt { get; } = "策略类名: ";
		public string StrategyModelClassName
		{
			get { return _strategyModel.StrategyClassName; }
			set { _strategyModel.StrategyClassName = value; }
		}
		public Command OpenScriptCommand { get; }
		public event Action? OpenScript;
        public StrategyConfigViewModel(StrategyModel strategyModel)
        {
			_strategyModel = strategyModel;
			OpenScriptCommand = new Command(OnOpenScript);
        }

		private void OnOpenScript(object? obj)
		{
			OpenScript?.Invoke();
		}
	}
}
