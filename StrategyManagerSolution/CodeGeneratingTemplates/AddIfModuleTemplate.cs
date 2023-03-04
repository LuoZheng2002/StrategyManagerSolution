using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class AddIfModuleTemplate
	{
        public Solution __SOLUTIONNAME__ = new Solution();
        public __IFCLASSNAME__ __IFCLASSNAME__ = new __IFCLASSNAME__();
		public AddIfModuleTemplate()
        {
            //start
            __SOLUTIONNAME__.IfModules.Add(__IFCLASSNAME__);
            //end
        }
    }
}
