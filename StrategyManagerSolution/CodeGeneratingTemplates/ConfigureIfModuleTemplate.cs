using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class ConfigureIfModuleTemplate
	{
        public __IFCLASSNAME__ __IFCLASSNAME__ = new();
        public ExecutableBase __CONNECTEDTOTRUEEXECUTABLE__ = new __IFCLASSNAME__();
        public ExecutableBase __CONNECTEDTOFALSEEXECUTABLE__ = new __IFCLASSNAME__();
        public ConfigureIfModuleTemplate()
        {
            //start
            __IFCLASSNAME__.ConnectedToTrue = __CONNECTEDTOTRUEEXECUTABLE__;
            __IFCLASSNAME__.ConnectedToFalse = __CONNECTEDTOFALSEEXECUTABLE__;
            //end
        }
    }
}
