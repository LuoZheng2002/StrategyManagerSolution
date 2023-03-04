using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
    public class __SIMULATIONCLASSNAME__: SimulationModuleBase
    {

    }
	internal class CreateSimulationModuleTemplate
	{
        public CreateSimulationModuleTemplate()
        {
            Solution __PLAYER1SOLUTION__ = new Solution();
            Solution __PLAYER2SOLUTION__ = new Solution();
            bool __PLAYER1MOVESFIRST__ = false;

            //start
            __SIMULATIONCLASSNAME__ __SIMULATIONCLASSNAME__ = new __SIMULATIONCLASSNAME__();
            __SIMULATIONCLASSNAME__.Player1Solution = __PLAYER1SOLUTION__;
            __SIMULATIONCLASSNAME__.Player2Solution = __PLAYER2SOLUTION__;
            __SIMULATIONCLASSNAME__.Player1MovesFirst = __PLAYER1MOVESFIRST__;
            //end

        }
    }
}
