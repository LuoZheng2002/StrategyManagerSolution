using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class CaseConfigViewModel: ViewModelBase
	{
		public string CaseNamePrompt { get; } = "请输入Case的名称: ";
		public string CaseName { get; set; } = "";
		public string CaseTextPrompt { get; } = "请输入Case的描述: ";
		public string CaseText { get; set; } = "";

	}
}
