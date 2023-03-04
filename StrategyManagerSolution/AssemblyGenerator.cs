using Contracts.MVVMModels;
using StrategyManagerSolution.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution
{
	public static class AssemblyGenerator
	{
		public static string GenerateCode(ProjectModel projectModel)
		{
			Dictionary<string, SolutionModel> solutionModels = new();
			foreach (var solutionName in projectModel.SolutionNames)
			{
				SolutionModel solutionModel = Serializer.Deserialize<SolutionModel>($"{projectModel.ProjectFolder}/{solutionName}.smsln");
				solutionModels.Add(solutionName, solutionModel);
			}
			string result = File.ReadAllText("../../../CodeGeneratingTemplates/CodeGeneratingTemplate.txt");
			result = result.Replace("/*__DECLARESOLUTIONS__*/", DeclareSolutions(projectModel));
			result = result.Replace("/*__CONFIGURESOLUTIONS__*/", ConfigureSolutions(projectModel, solutionModels));
			result = result.Replace("/*__ADDSOLUTIONS__*/", AddSolutions(projectModel));
			return result;
		}
		private static string AddSolutions(ProjectModel projectModel)
		{
			string result = "";
			foreach (var solutionName in projectModel.SolutionNames)
			{
				string addSolution = File.ReadAllText("../../../CodeGeneratingTemplates/AddSolutionTemplate.txt");
				addSolution = addSolution.Replace("__SOLUTIONNAME__", solutionName);
				result += addSolution;
			}
			return result;
		}
		private static string DeclareSolutions(ProjectModel projectModel)
		{
			string result = "";
			foreach (var solutionName in projectModel.SolutionNames)
			{
				string declareSolution = File.ReadAllText("../../../CodeGeneratingTemplates/DeclareSolutionTemplate.txt");
				declareSolution = declareSolution.Replace("__SOLUTIONNAME__", solutionName);
				result += declareSolution;
			}
			return result;
		}
		private static string ConfigureSolutions(ProjectModel projectModel, Dictionary<string, SolutionModel>solutionModels)
		{
			string result = "";
			foreach (SolutionModel solutionModel in solutionModels.Values)
			{
				string configureSolution = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureSolutionTemplate.txt");
				configureSolution = configureSolution.Replace("/*__CREATESTRATEGYSETS__*/", CreateStrategySets(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATEIFMODULES__*/", CreateIfModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATESWITCHMODULES__*/", CreateSwitchModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATESIMULATIONMODULES__*/", CreateSimulationModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGURESTRATEGIES__*/", ConfigureStrategies(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGUREIFMODULES__*/", ConfigureIfModules(solutionModel));
				/*__CONFIGURESTRATEGIES__*/
				/*__CONFIGUREIFMODULES__*/
				/*__CONFIGURESWITCHMODULES__*/
				/*__CONFIGURESIMULATIONMODULES*/
				/*__ADDSTRATEGYSETS__*/
				/*__ADDIFMODULES__*/
				/*__ADDSWITCHMODULES__*/
				/*__ADDSIMULATIONMODULES__*/
				result += configureSolution;
			}
			return result;
		}
		private static string ConfigureStrategies(SolutionModel solutionModel)
		{

		}
		private static string ConfigureIfModules(SolutionModel solutionModel)
		{

		}
		private static string ConfigureSwitchModules(SolutionModel solutionModel)
		{

		}
		private static string ConfigureSimulationModules(SolutionModel solutionModel)
		{

		}
		private static string AddStrategySets(SolutionModel solutionModel)
		{

		}
		private void 
		private static string CreateStrategySets(SolutionModel solutionModel)
		{
			string result = "";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is StrategySetModel)
				{
					StrategySetModel strategySetModel = (StrategySetModel)item;
					string createStrategySet = File.ReadAllText("../../../CodeGeneratingTemplates/CreateStrategySetTemplate.txt");
					createStrategySet = createStrategySet.Replace("__STRATEGYSETNAME__", strategySetModel.StrategySetName);
					createStrategySet = createStrategySet.Replace("/*__ADDSTRATEGIES__*/", AddStrategies(strategySetModel));
					result += createStrategySet;
				}
			}
			return result;
		}
		private static string AddStrategies(StrategySetModel strategySetModel)
		{
			string result = "";
			foreach(var strategyModel in  strategySetModel.Strategies)
			{
				string addStrategy = File.ReadAllText("../../../CodeGeneratingTemplates/AddStrategyTemplate.txt");
				addStrategy = addStrategy.Replace("__STRATEGYCLASSNAME__", strategyModel.StrategyClassName);
				addStrategy = addStrategy.Replace("__STRATEGYSETNAME__", strategySetModel.StrategySetName);
				result += addStrategy;
			}
			return result;
		}
		private static string CreateIfModules(SolutionModel solutionModel)
		{
			string result = "";
			foreach(var item in solutionModel.DiagramItemModels)
			{
				if (item is IfModel)
				{
					IfModel ifModel = (IfModel)item;
					string createIfModule = File.ReadAllText("../../../CodeGeneratingTemplates/CreateIfModuleTemplate.txt");
					createIfModule = createIfModule.Replace("__IFCLASSNAME__", ifModel.IfModelClassName);
					result += createIfModule;
				}
			}
			return result;
		}
		private static string CreateSwitchModules(SolutionModel solutionModel)
		{
			string result = "";
			foreach(var item in solutionModel.DiagramItemModels)
			{
				if (item is SwitchModel)
				{
					SwitchModel switchModel = (SwitchModel)item;
					string createSwitchModule = File.ReadAllText("../../../CodeGeneratingTemplates/CreateSwitchModuleTemplate.txt");
					createSwitchModule = createSwitchModule.Replace("__SWITCHCLASSNAME__", switchModel.SwitchModelClassName);
					result += createSwitchModule;
				}
			}
			return result;
		}
		private static string CreateSimulationModules(SolutionModel solutionModel)
		{
			string result = "";
			foreach(var item in solutionModel.DiagramItemModels)
			{
				if (item is SimulationModel)
				{
					SimulationModel simulationModel = (SimulationModel)item;
					string createSimulationModule = File.ReadAllText("../../../CodeGeneratingTemplates/CreateSimulationModuleTemplate.txt");
					createSimulationModule = createSimulationModule.Replace("__PLAYER1SOLUTION__", simulationModel.Player1SolutionName);
					createSimulationModule = createSimulationModule.Replace("__PLAYER2SOLUTION__", simulationModel.Player2SolutionName);
					createSimulationModule = createSimulationModule.Replace("__PLAYER1MOVESFIRST__", simulationModel.Player1MovesFirst?"true":"false");
					createSimulationModule = createSimulationModule.Replace("__SIMULATIONCLASSNAME__", simulationModel.SimulationModelClassName);
					result += createSimulationModule;
				}
			}
			return result;
		}
	}
}
