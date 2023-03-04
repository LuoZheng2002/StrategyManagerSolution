using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class AddSwitchModuleTemplate
	{
        public Solution __SOLUTIONNAME__ = new Solution();
        public __SWITCHCLASSNAME__ __SWITCHCLASSNAME__ = new __SWITCHCLASSNAME__();
        public AddSwitchModuleTemplate()
        {
            //start
            __SOLUTIONNAME__.SwitchModules.Add(__SWITCHCLASSNAME__);
        }
    }
}
