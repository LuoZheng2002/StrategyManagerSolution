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
		public static void Initialize(this ProjectModel projectModel)
		{
			Directory.CreateDirectory(projectModel.ProjectFolder);
			Directory.CreateDirectory(projectModel.VSCodeFolder);
			Serializer.Serialize(projectModel.ProjectDirectory, projectModel);
		}
	}
}
