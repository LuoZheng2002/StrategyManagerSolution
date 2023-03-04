using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	public class CreateStrategySetTemplate:ProjSlnFuncProviderBase
	{
		public override ProjectSolution ProjectSolution { get; }
        public CreateStrategySetTemplate()
        {
            ProjectSolution = new ProjectSolution();

            //start
            StrategySet __STRATEGYSETNAME__ = new StrategySet();
            /*__ADDSTRATEGIES__*/

            //end
        }
    }
}
