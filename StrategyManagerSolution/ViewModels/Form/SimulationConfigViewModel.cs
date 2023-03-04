using Contracts.MVVMModels;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels.Form
{
	internal class SimulationConfigViewModel:ViewModelBase
	{
		private SimulationModel _simulationModel;
		public string SimulationModuleNamePrompt { get; } = "推演模块名称: ";
		public string SimulationModuleName
		{
			get { return _simulationModel.SimulationModuleName; }
			set { _simulationModel.SimulationModuleName = value; }
		}
		public string SimulationDescriptionPrompt { get; } = "推演描述: ";
		public string SimulationDescription
		{
			get { return _simulationModel.SimulationDescription; }
			set { _simulationModel.SimulationDescription = value; }
		}
		public string SimulationModelClassNamePrompt { get; } = "推演模块类名: ";
		public string SimulationModelClassName
		{
			get { return _simulationModel.SimulationModelClassName; }
			set { _simulationModel.SimulationModelClassName = value; }
		}
		public string Player1NamePrompt { get; } = "玩家1名称: ";
		public string Player1Name
		{
			get { return _simulationModel.Player1Name; }
			set { _simulationModel.Player1Name = value; }
		}
		public string Player1SolutionNamePrompt { get; } = "玩家1解决方案名称: ";
		public string Player1SolutionName
		{
			get { return _simulationModel.Player1SolutionName; }
			set { _simulationModel.Player1SolutionName = value; }
		}
		public string Player2NamePrompt { get; } = "玩家2名称: ";
		public string Player2Name
		{
			get { return _simulationModel.Player2Name; }
			set { _simulationModel.Player2Name = value;}
		}
		public string Player2SolutionNamePrompt { get; } = "玩家2解决方案名称: ";
		public string Player2SolutionName
		{
			get { return _simulationModel.Player2SolutionName; }
			set { _simulationModel.Player2SolutionName = value;}
		}
		public Command OpenScriptCommand { get; }
		public event Action? OpenScript;
        public SimulationConfigViewModel(SimulationModel simulationModel)
        {
            _simulationModel = simulationModel;
			OpenScriptCommand = new Command((_)=>OpenScript?.Invoke());
        }
    }
}
