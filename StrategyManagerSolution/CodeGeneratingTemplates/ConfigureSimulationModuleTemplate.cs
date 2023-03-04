using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class ConfigureSimulationModuleTemplate
	{
        public __SIMULATIONCLASSNAME__ __SIMULATIONCLASSNAME__ = new();
        public ExecutableBase __PLAYER1EXECUTABLE__ = new __SIMULATIONCLASSNAME__();
        public ExecutableBase __PLAYER2EXECUTABLE__ = new __SIMULATIONCLASSNAME__();
        public ConfigureSimulationModuleTemplate()
        {
            //start
            __SIMULATIONCLASSNAME__.ConnectedToPlayer1Wins = __PLAYER1EXECUTABLE__;
            __SIMULATIONCLASSNAME__.ConnectedToPlayer2Wins = __PLAYER2EXECUTABLE__;

            //end
        }
    }
}
