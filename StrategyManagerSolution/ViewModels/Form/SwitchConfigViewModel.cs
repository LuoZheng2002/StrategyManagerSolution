using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class SwitchConfigViewModel:ViewModelBase
	{
		public string SwitchModuleNamePrompt { get; } = "Switch模块名称: ";
		public string SwitchModuleName { get; set; } = "";
		public string SwitchStatementTextPrompt { get; } = "判断内容的描述: ";
		public string SwitchStatementText { get; set; } = "";
		public string SwitchModelClassNamePrompt { get; set; } = "Switch模块类名: ";
		public string SwitchModelClassName { get; set; } = "";
	}
}
