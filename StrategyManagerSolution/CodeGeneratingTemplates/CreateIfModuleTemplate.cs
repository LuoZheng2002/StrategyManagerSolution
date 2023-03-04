using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
    public class __IFCLASSNAME__: IfModuleBase
    {
		public override bool JudgeStatement(GameModelBase gameModel)
		{
			throw new NotImplementedException();
		}
	}
	public class CreateIfModuleTemplate: ProjSlnFuncProviderBase
	{
        
		public override ProjectSolution ProjectSolution => throw new NotImplementedException();
        public CreateIfModuleTemplate()
        {
			//start
			__IFCLASSNAME__ __IFCLASSNAME__ = new __IFCLASSNAME__();
			//end
        }
    }
}
