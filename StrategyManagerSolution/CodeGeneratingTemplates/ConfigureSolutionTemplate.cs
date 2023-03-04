using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	public class ConfigureSolutionTemplate:ProjSlnFuncProviderBase
	{
		public override ProjectSolution ProjectSolution { get; }
		public Solution __SOLUTIONNAME__ = new Solution();
		public ConfigureSolutionTemplate()
        {
            ProjectSolution = new ProjectSolution();
			ExecutableBase __STARTEXECUTABLE__ = null;

			// start
			
			
			/*__CREATESTRATEGYSETS__*/
			/*__CREATEIFMODULES__*/
			/*__CREATESWITCHMODULES__*/
			/*__CREATESIMULATIONMODULES__*/
			__SOLUTIONNAME__.StartExecutable = __STARTEXECUTABLE__;
			/*__CONFIGURESTRATEGIES__*/
			/*__CONFIGUREIFMODULES__*/
			/*__CONFIGURESWITCHMODULES__*/
			/*__CONFIGURESIMULATIONMODULES*/
			/*__ADDSTRATEGYSETS__*/
			/*__ADDIFMODULES__*/
			/*__ADDSWITCHMODULES__*/
			/*__ADDSIMULATIONMODULES__*/
			

			//end
        }
    }
}
