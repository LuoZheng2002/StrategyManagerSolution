using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
    public class __STRATEGYCLASSNAME__: StrategyBase
    {

    }
	public class AddStrategyTemplate:ProjSlnFuncProviderBase
	{
        public override ProjectSolution ProjectSolution { get; } = new();
        public StrategySet __STRATEGYSETNAME__ { get; }= new StrategySet();
        public AddStrategyTemplate()
        {
            //start
            __STRATEGYCLASSNAME__ __STRATEGYCLASSNAME__ = new __STRATEGYCLASSNAME__();
            __STRATEGYSETNAME__.AddStrategy(__STRATEGYCLASSNAME__);

            //end
        }
    }
}
