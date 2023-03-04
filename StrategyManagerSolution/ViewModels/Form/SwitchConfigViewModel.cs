using Contracts.MVVMModels;
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
		private SwitchModel _switchModel;
		public string SwitchModuleNamePrompt { get; } = "Switch模块名称: ";
		public string SwitchModuleName
		{
			get { return _switchModel.SwitchModuleName; }
			set { _switchModel.SwitchModuleName = value; }
		}
		public string SwitchStatementTextPrompt { get; } = "判断内容的描述: ";
		public string SwitchStatementText
		{
			get { return _switchModel.SwitchTargetText; }
			set { _switchModel.SwitchTargetText = value; }
		}
		public string SwitchModelClassNamePrompt { get; set; } = "Switch模块类名: ";
		public string SwitchModelClassName
		{
			get { return _switchModel.SwitchModelClassName; }
			set { _switchModel.SwitchModelClassName = value; }
		}
		public Command OpenScriptCommand { get; }
		public event Action? OpenScript;
        public SwitchConfigViewModel(SwitchModel switchModel)
        {
            _switchModel = switchModel;
			OpenScriptCommand = new Command((_) => OpenScript?.Invoke());
        }
    }
}
