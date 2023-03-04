using Contracts.MVVMModels;
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
		private CaseModel _caseModel;
		public string CaseNamePrompt { get; } = "请输入Case的名称: ";
		public string CaseName
		{
			get { return _caseModel.CaseName; }
			set { _caseModel.CaseName = value;}
		}
		public string CaseTextPrompt { get; } = "请输入Case的描述: ";
		public string CaseText
		{
			get { return _caseModel.CaseText; }
			set { _caseModel.CaseText = value; }
		}
        public CaseConfigViewModel(CaseModel caseModel)
        {
            _caseModel = caseModel;
        }
    }
}
