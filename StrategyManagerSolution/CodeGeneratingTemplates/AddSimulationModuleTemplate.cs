using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class AddSimulationModuleTemplate
	{
		__SIMULATIONCLASSNAME__ __SIMULATIONCLASSNAME__ = new __SIMULATIONCLASSNAME__();
		public Solution __SOLUTIONNAME__ = new Solution();
        public AddSimulationModuleTemplate()
        {
            //start
            __SOLUTIONNAME__.SimulationModules.Add(__SIMULATIONCLASSNAME__);
            //end
        }
    }
}
