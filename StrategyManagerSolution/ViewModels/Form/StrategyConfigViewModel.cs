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
		public string StrategyNamePrompt { get; } = "策略名称: ";
		public string StrategyName { get; set; } = "";
		public string StrategyModelClassNamePrompt { get; } = "策略类名: ";
		public string StrategyModelClassName { get; set; } = "";
		public Command OpenScriptCommand { get; }
		public event Action? OpenScript;
        public StrategyConfigViewModel()
        {
			OpenScriptCommand = new Command(OnOpenScript);
        }

		private void OnOpenScript(object? obj)
		{
			OpenScript?.Invoke();
		}
	}
}
