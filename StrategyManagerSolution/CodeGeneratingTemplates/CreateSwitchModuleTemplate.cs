using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
    public class __SWITCHCLASSNAME__:SwitchModuleBase
    {
		public override Dictionary<string, Func<GameModelBase, bool>> Cases => throw new NotImplementedException();
	}
	public class CreateSwitchModuleTemplate: ProjSlnFuncProviderBase
	{
		public override ProjectSolution ProjectSolution => throw new NotImplementedException();

        public CreateSwitchModuleTemplate()
        {
            
            //start
            __SWITCHCLASSNAME__ __SWITCHCLASSNAME__ = new __SWITCHCLASSNAME__();
            //end
        }
    }
}
