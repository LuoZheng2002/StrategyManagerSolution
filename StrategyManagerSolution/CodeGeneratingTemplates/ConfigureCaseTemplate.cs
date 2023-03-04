using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class ConfigureCaseTemplate
	{
        public __SWITCHCLASSNAME__ __SWITCHCLASSNAME__ = new __SWITCHCLASSNAME__();
        public string __KEY__ = "";
        public ExecutableBase __EXECUTABLECLASSNAME__ = new __SWITCHCLASSNAME__();
        public ConfigureCaseTemplate()
        {
            //start
            __SWITCHCLASSNAME__.Connections.Add(__KEY__, __EXECUTABLECLASSNAME__);
            //end
        }
    }
}
