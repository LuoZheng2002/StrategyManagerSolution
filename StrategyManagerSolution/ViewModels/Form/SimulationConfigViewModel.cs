using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class SimulationConfigViewModel:ViewModelBase
	{
		public string SimulationModuleNamePrompt { get; } = "推演模块名称: ";
		public string SimulationModuleName { get; set; } = "";
		public string SimulationDescriptionPrompt { get; } = "推演描述: ";
		public string SimulationDescription { get; set; } = "";
		public string SimulationModelClassNamePrompt { get; } = "推演模块类名: ";
		public string SimulationModelClassName { get; set; } = "";
		public string Player1NamePrompt { get; } = "玩家1名称: ";
		public string Player1Name { get; set; } = "";
		public string Player1SolutionNamePrompt { get; } = "玩家1解决方案名称: ";
		public string Player1SolutionName { get; set; } = "";
		public string Player2NamePrompt { get; } = "玩家2名称: ";
		public string Player2Name { get; set; } = "";
		public string Player2SolutionNamePrompt { get; } = "玩家2解决方案名称: ";
		public string Player2SolutionName { get; set; } = "";
	}
}
