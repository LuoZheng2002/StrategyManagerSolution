using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class FileConfigViewModel:ViewModelBase
	{
		public string SolutionNamePrompt { get; } = "解决方案名称: ";
		public string SolutionName { get; set; } = "";
	}
}
