using Contracts.Enums;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class StrategySetConfigViewModel:ViewModelBase
	{
		public string StrategySetName { get; set; } = "";
		public string PromptText { get; }
		public StrategySetConfigViewModel(StrategySetType type)
		{
			PromptText = (type == StrategySetType.Hierarchical ? "等级策略集" : "平行策略集") + "名称: ";
		}
	}
}
