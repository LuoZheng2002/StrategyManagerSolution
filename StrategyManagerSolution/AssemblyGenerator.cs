using Contracts;
using Contracts.MVVMModels;
using StrategyManagerSolution.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution
{
	public static class AssemblyGenerator
	{
		public static bool GenerateCode(ProjectModel projectModel, out string result, out string errorMessages)
		{
			bool succeeded = true;
			errorMessages = "";
			Dictionary<string, SolutionModel> solutionModels = new();
			foreach (var solutionName in projectModel.SolutionNames)
			{
				SolutionModel solutionModel = Serializer.Deserialize<SolutionModel>($"{projectModel.ProjectFolder}/{solutionName}.smsln");
				solutionModels.Add(solutionName, solutionModel);
			}
			result = "";
			result += "/*** [GenerateCode] ***/\n";
			result += File.ReadAllText("../../../CodeGeneratingTemplates/CodeGeneratingTemplate.txt");
			result = result.Replace("__NAMESPACENAME__", projectModel.VSCodeFolderName);
			result = result.Replace("__CLASSNAME__", projectModel.ProjectName + "ProjSlnFuncProvider");
			result = result.Replace("/*__DECLARESOLUTIONS__*/", DeclareSolutions(projectModel));
			result = result.Replace("/*__CONFIGURESOLUTIONS__*/", ConfigureSolutions(projectModel, solutionModels));
			result = result.Replace("/*__ADDSOLUTIONS__*/", AddSolutions(projectModel));
			result = result.Replace("__MAINSOLUTION__", "main");
			return succeeded;
		}
		private static string AddSolutions(ProjectModel projectModel)
		{
			string result = "";
			result += "/*** [GenerateCode/AddSolutions] ***/\n";
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
			result += "/*** [GenerateCode/DeclareSolutions] ***/\n";
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
			result += "/*** [GenerateCode/ConfigureSolutions] ***/\n";
			foreach (SolutionModel solutionModel in solutionModels.Values)
			{
				string configureSolution = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureSolutionTemplate.txt");
				configureSolution = configureSolution.Replace("/*__CREATESTRATEGYSETS__*/", CreateStrategySets(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATEIFMODULES__*/", CreateIfModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATESWITCHMODULES__*/", CreateSwitchModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CREATESIMULATIONMODULES__*/", CreateSimulationModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGURESTRATEGIES__*/", ConfigureStrategies(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGUREIFMODULES__*/", ConfigureIfModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGURESWITCHMODULES__*/", ConfigureSwitchModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__CONFIGURESIMULATIONMODULES__*/", ConfigureSimulationModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__ADDSTRATEGYSETS__*/", AddStrategySets(solutionModel));
				configureSolution = configureSolution.Replace("/*__ADDIFMODULES__*/", AddIfModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__ADDSWITCHMODULES__*/", AddSwitchModules(solutionModel));
				configureSolution = configureSolution.Replace("/*__ADDSIMULATIONMODULES__*/", AddSimulationModules(solutionModel));
				configureSolution = configureSolution.Replace("__SOLUTIONNAME__", solutionModel.SolutionName);
				StartModel startModel = (StartModel)solutionModel.DiagramItemModels.FirstOrDefault(item=>item is StartModel)!;
				configureSolution = configureSolution.Replace("__STARTEXECUTABLE__", startModel.LinkingTo!.ClassName);
				result += configureSolution;
			}
			return result;
		}
		private static string ConfigureStrategies(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/ConfigureStrategies] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is StrategySetModel)
				{
					StrategySetModel strategySetModel = (StrategySetModel)item;
					foreach (var strategy in strategySetModel.Strategies)
					{
						if (strategy.LinkingTo != null)
						{
							string configureStrategy = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureStrategyTemplate.txt");
							configureStrategy = configureStrategy.Replace("__STRATEGYCLASSNAME__", strategy.StrategyClassName);
							configureStrategy = configureStrategy.Replace("__EXECUTABLECLASSNAME__", strategy.LinkingTo.ClassName);
							result += configureStrategy;
						}
					}
				}
			}
			return result;
		}
		private static string ConfigureIfModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/ConfigureIfModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is IfModel)
				{
					IfModel ifModel = (IfModel)item;
					string configureIfModule = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureIfModuleTemplate.txt");
					configureIfModule = configureIfModule.Replace("__IFCLASSNAME__", ifModel.IfModelClassName);
					configureIfModule = configureIfModule.Replace("__CONNECTEDTOTRUEEXECUTABLE__", ifModel.TrueCaseModel.LinkingTo!.ClassName);
					configureIfModule = configureIfModule.Replace("__CONNECTEDTOFALSEEXECUTABLE__", ifModel.FalseCaseModel.LinkingTo!.ClassName);
					result += configureIfModule;
				}
			}
			return result;
		}
		private static string ConfigureSwitchModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/ConfigureSwitchModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is SwitchModel)
				{
					SwitchModel switchModel = (SwitchModel)item;
					foreach (var switchCase in switchModel.CaseModels)
					{
						if (switchCase.LinkingTo != null)
						{
							string configureCase = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureCaseTemplate.txt");
							configureCase = configureCase.Replace("__SWITCHCLASSNAME__", switchModel.SwitchModelClassName);
							configureCase = configureCase.Replace("__KEY__", switchCase.CaseName);
							configureCase = configureCase.Replace("__EXECUTABLECLASSNAME__", switchCase.LinkingTo.ClassName);
							result += configureCase;
						}
					}
				}
			}
			return result;
		}
		private static string ConfigureSimulationModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/ConfigureSimulationModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is SimulationModel)
				{
					SimulationModel simulationModel = (SimulationModel)item;
					string configureSimulationModule = File.ReadAllText("../../../CodeGeneratingTemplates/ConfigureSimulationModuleTemplate.txt");
					configureSimulationModule = configureSimulationModule.Replace("__SIMULATIONCLASSNAME__", simulationModel.SimulationModelClassName);
					configureSimulationModule = configureSimulationModule.Replace("__PLAYER1EXECUTABLE__", simulationModel.Player1WinsCaseModel.LinkingTo!.ClassName);
					configureSimulationModule = configureSimulationModule.Replace("__PLAYER2EXECUTABLE__", simulationModel.Player2WinsCaseModel.LinkingTo!.ClassName);
					result += configureSimulationModule;
				}
			}
			return result;
		}
		private static string AddStrategySets(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/AddStrategySets] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is StrategySetModel)
				{
					StrategySetModel strategySetModel = (StrategySetModel)item;
					string addStrategySet = File.ReadAllText("../../../CodeGeneratingTemplates/AddStrategySetTemplate.txt");
					addStrategySet = addStrategySet.Replace("__SOLUTIONNAME__", solutionModel.SolutionName);
					addStrategySet = addStrategySet.Replace("__STRATEGYSETNAME__", strategySetModel.StrategySetName);
					result += addStrategySet;
				}
			}
			return result;
		}
		private static string AddIfModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/AddIfModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is IfModel)
				{
					IfModel ifModel = (IfModel)item;
					string addIfModule = File.ReadAllText("../../../CodeGeneratingTemplates/AddIfModuleTemplate.txt");
					addIfModule = addIfModule.Replace("__SOLUTIONNAME__", solutionModel.SolutionName);
					addIfModule = addIfModule.Replace("__IFCLASSNAME__", ifModel.IfModelClassName);
					result += addIfModule;
				}
			}
			return result;
		}
		private static string AddSwitchModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/AddSwitchModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is SwitchModel)
				{
					SwitchModel switchModel = (SwitchModel)item;
					string addSwitchModule = File.ReadAllText("../../../CodeGeneratingTemplates/AddSwitchModuleTemplate.txt");
					addSwitchModule = addSwitchModule.Replace("__SOLUTIONNAME__", solutionModel.SolutionName);
					addSwitchModule = addSwitchModule.Replace("__SWITCHCLASSNAME__", switchModel.SwitchModelClassName);
					result += addSwitchModule;
				}
			}
			return result;
		}
		private static string AddSimulationModules(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/AddSimulationModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
			{
				if (item is SimulationModel)
				{
					SimulationModel simulationModel = (SimulationModel)item;
					string addSimulationModule = File.ReadAllText("../../../CodeGeneratingTemplates/AddSimulationModuleTemplate.txt");
					addSimulationModule = addSimulationModule.Replace("__SOLUTIONNAME__", solutionModel.SolutionName);
					addSimulationModule = addSimulationModule.Replace("__SIMULATIONCLASSNAME__", simulationModel.SimulationModelClassName);
					result += addSimulationModule;
				}
			}
			return result;
		}
		private static string CreateStrategySets(SolutionModel solutionModel)
		{
			string result = "";
			result += "/*** [GenerateCode/ConfigureSolutions/CreateStrategySets] ***/\n";
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
			result += "/*** [GenerateCode/ConfigureSolutions/CreateStrategySets/AddStrategies] ***/\n";
			foreach (var strategyModel in  strategySetModel.Strategies)
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
			result += "/*** [GenerateCode/ConfigureSolutions/CreateIfModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
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
			result += "/*** [GenerateCode/ConfigureSolutions/CreateSwitchModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
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
			result += "/*** [GenerateCode/ConfigureSolutions/CreateSimulationModules] ***/\n";
			foreach (var item in solutionModel.DiagramItemModels)
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
