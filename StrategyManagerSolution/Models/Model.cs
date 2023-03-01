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
	internal class Model
	{
		private List<RecentProject>? _recentProjects;
		public List<RecentProject> RecentProjects
		{
			get 
			{ 
				if (_recentProjects==null)
				{
					_recentProjects = GetRecentProjects();
					return _recentProjects;
				}
				else
					return _recentProjects;
			}
		}

		public ProjectModel? CurrentProjectModel { get; set; }
		// 当前选中的流程图文件
		public SolutionModel? CurrentSolutionModel { get; set; }
		private List<RecentProject> GetRecentProjects()
		{
			//to do
			return new List<RecentProject>()
			{
				new RecentProject("a.smproj", "C:/a.smproj", new DateTime(2022,1,1)),
				new RecentProject("b.smproj", "C:/b.smproj", new DateTime(2022,1,1)),
				new RecentProject("c.smproj", "C:/c.smproj", new DateTime(2022,1,1)),
			};
		}
		public void OpenProject(string directory)
		{
			CurrentProjectModel = Serializer.Deserialize<ProjectModel>(directory);
		}
		public void SaveProject()
		{
			Serializer.Serialize(CurrentProjectModel!.ProjectDirectory, CurrentProjectModel);
		}
		public void CreateProject(string projectName, string location, string visualStudioSolutionName)
		{
			string projectFolder = location + "/" + projectName;
			string projectDirectory = projectFolder + "/" + projectName + ".smproj";
			CurrentProjectModel = new ProjectModel(projectName, projectFolder, projectDirectory, visualStudioSolutionName);
			CurrentProjectModel.Initialize();
			CurrentProjectModel.AddSolution("main");
			CurrentSolutionModel = Serializer.Deserialize<SolutionModel>(projectFolder + "/main.smsln");
		}
		public void SaveCurrentSolution()
		{
			string solutionDirectory = CurrentProjectModel!.ProjectFolder + "/" + CurrentSolutionModel!.SolutionFileName;
			Serializer.Serialize(solutionDirectory, CurrentSolutionModel);
		}
	}
}
