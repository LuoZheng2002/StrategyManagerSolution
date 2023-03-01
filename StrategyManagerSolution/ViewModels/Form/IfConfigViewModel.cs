using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class IfConfigViewModel:ViewModelBase
	{
		public string IfModuleNamePrompt { get; } = "请输入If模块名称: ";
		public string IfModuleName { get; set; } = "";
		public string IfStatementTextPrompt { get; } = "请输入要判断的内容的描述: ";
		public string IfStatementText { get; set; } = "";
		public string IfModelClassNamePrompt { get; set; } = "请输入If模块实现的类名: ";
		public string IfModelClassName { get; set; } = "";

	}
}
