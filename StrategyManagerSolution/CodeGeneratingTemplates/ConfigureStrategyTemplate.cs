using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
    
	internal class ConfigureStrategyTemplate
	{
        public __STRATEGYCLASSNAME__ __STRATEGYCLASSNAME__ = new();
        public ExecutableBase __EXECUTABLECLASSNAME__ = new __STRATEGYCLASSNAME__();
        public ConfigureStrategyTemplate()
        {

            //start
            __STRATEGYCLASSNAME__.LinkTo(__EXECUTABLECLASSNAME__);

            //end
        }
    }
}
