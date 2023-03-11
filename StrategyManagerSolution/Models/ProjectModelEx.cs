using Contracts;
using Contracts.MVVMModels;
using StrategyManagerSolution.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Models
{
	public static class ProjectModelEx
	{
		public static void AddSolution(this ProjectModel projectModel,string solutionName)
		{
			projectModel.SolutionNames.Add(solutionName);
			Serializer.Serialize(projectModel.ProjectFolder + "/" + solutionName + ".smsln", new SolutionModel(solutionName));
		}
		public static void DeleteSolution(this ProjectModel projectModel,string solutionName)
		{
			projectModel.SolutionNames.Remove(solutionName);
			File.Delete(projectModel.ProjectFolder + "/" + solutionName + ".smsln");
		}
		public static void Initialize(this ProjectModel projectModel, string projectReferenceDirectory)
		{
			Directory.CreateDirectory(projectModel.ProjectFolder);
			Directory.CreateDirectory(projectModel.VSCodeFolder);
			Serializer.Serialize(projectModel.ProjectDirectory, projectModel);
			Cmd.ExecuteCommand("dotnet new classlib --framework net6.0", projectModel.VSCodeFolder);
			string csprojDir = $"{projectModel.VSCodeFolder}/{projectModel.VSCodeFolderName}.csproj";
			string csproj = File.ReadAllText(csprojDir);
			csproj = csproj.Replace("net6.0", "net6.0-windows");
			File.WriteAllText(csprojDir, csproj);
			Cmd.ExecuteCommand($"dotnet add reference {projectReferenceDirectory}", projectModel.VSCodeFolder);
			string class1File = $"{projectModel.VSCodeFolder}/Class1.cs";
			if (File.Exists(class1File))
			File.Delete(class1File);
		}
	}
}
