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
		public string SwitchModuleNamePrompt { get; } = "请输入Switch模块名称: ";
		public string SwitchModuleName { get; set; } = "";
		public string SwitchStatementTextPrompt { get; } = "请输入要判断的内容的描述: ";
		public string SwitchStatementText { get; set; } = "";
		public string SwitchModelClassNamePrompt { get; set; } = "请输入Switch模块实现的类名: ";
		public string SwitchModelClassName { get; set; } = "";
	}
}
