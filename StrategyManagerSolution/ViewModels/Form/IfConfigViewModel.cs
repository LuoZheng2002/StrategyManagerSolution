using Contracts.MVVMModels;
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
		private IfModel _ifModel;
		public string IfModuleNamePrompt { get; } = "请输入If模块名称: ";
		public string IfModuleName
		{
			get { return _ifModel.IfModuleName; }
			set { _ifModel.IfModuleName = value; }
		}
		public string IfStatementTextPrompt { get; } = "请输入要判断的内容的描述: ";
		public string IfStatementText
		{
			get { return _ifModel.IfStatementText; }
			set { _ifModel.IfStatementText = value; }
		}
		public string IfModelClassNamePrompt { get; set; } = "请输入If模块实现的类名: ";
		public string IfModelClassName
		{
			get { return _ifModel.IfModelClassName; }
			set { _ifModel.IfModelClassName = value; }
		}
		public Command OpenScriptCommand { get; }
		public event Action? OpenScript;
        public IfConfigViewModel(IfModel ifModel)
        {
            _ifModel = ifModel;
			OpenScriptCommand = new Command((_) => OpenScript?.Invoke());
        }
    }
}
