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
		public string StrategySetNamePrompt { get; }
		public string StrategySetName
		{
			get { return _strategySetModel.StrategySetName; }
			set {  _strategySetModel.StrategySetName = value;}
		}
		public string StrategySetDescriptionPrompt { get; }
		public string StrategySetDescription
		{
			get { return _strategySetModel.StrategySetDescription; }
			set { _strategySetModel.StrategySetDescription = value; }
		}
		public StrategySetConfigViewModel(StrategySetModel strategySetModel, StrategySetType type)
		{
			_strategySetModel = strategySetModel;
			StrategySetNamePrompt = (type == StrategySetType.Hierarchical ? "等级策略集" : "平行策略集") + "名称: ";
			StrategySetDescriptionPrompt = (type == StrategySetType.Hierarchical ? "等级策略集" : "平行策略集") + "描述: ";
		}
	}
}
