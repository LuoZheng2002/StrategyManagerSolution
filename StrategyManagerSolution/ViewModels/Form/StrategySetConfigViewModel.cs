using Contracts.Enums;
using Contracts.MVVMModels;
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
		private StrategySetModel _strategySetModel;
		public string PromptText { get; }
		public string StrategySetName
		{
			get { return _strategySetModel.StrategySetName; }
			set {  _strategySetModel.StrategySetName = value;}
		}
		
		public StrategySetConfigViewModel(StrategySetModel strategySetModel, StrategySetType type)
		{
			_strategySetModel = strategySetModel;
			PromptText = (type == StrategySetType.Hierarchical ? "等级策略集" : "平行策略集") + "名称: ";
		}
	}
}
