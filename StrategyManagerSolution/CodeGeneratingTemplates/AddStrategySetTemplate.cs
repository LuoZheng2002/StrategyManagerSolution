using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class AddStrategySetTemplate
	{
        public StrategySet __STRATEGYSETNAME__ = new StrategySet();
        public Solution __SOLUTIONNAME__ = new Solution();
        public AddStrategySetTemplate()
        {
            //start
            __SOLUTIONNAME__.StrategySets.Add(__STRATEGYSETNAME__);
            //end
        }
    }
}
