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
		public ProgramData ProgramData { get; set; }
		public List<RecentProject> RecentProjects => ProgramData.RecentProjects;

		public ProjectModel? CurrentProjectModel { get; set; }
		// 当前选中的流程图文件
		public SolutionModel? CurrentSolutionModel { get; set; }
		public void OpenProject(string directory)
		{
			CurrentProjectModel = Serializer.Deserialize<ProjectModel>(directory);
		}
		public void SaveProject()
		{
			Serializer.Serialize(CurrentProjectModel!.ProjectDirectory, CurrentProjectModel);
		}
		public void CreateProject(string projectName, string location, string vsCodeFolderName)
		{
			string projectFolder = location + "/" + projectName;
			string projectDirectory = projectFolder + "/" + projectName + ".smproj";
			CurrentProjectModel = new ProjectModel(projectName, projectFolder, projectDirectory, vsCodeFolderName);
			CurrentProjectModel.Initialize(ProgramData.ProjectReferenceDirectory!);
			CurrentProjectModel.AddSolution("main");
			CurrentSolutionModel = Serializer.Deserialize<SolutionModel>(projectFolder + "/main.smsln");
		}
		public void SaveCurrentSolution()
		{
			string solutionDirectory = CurrentProjectModel!.ProjectFolder + "/" + CurrentSolutionModel!.SolutionFileName;
			Serializer.Serialize(solutionDirectory, CurrentSolutionModel);
		}
		public void SaveProgramData()
		{
			Serializer.Serialize("../../../ProgramData.json", ProgramData);
		}
        public Model()
        {
			ProgramData = Serializer.Deserialize<ProgramData>("../../../ProgramData.json");
        }
    }
}
