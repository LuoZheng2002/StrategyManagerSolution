using StrategyManagerSolution.Models;
using StrategyManagerSolution.MVVMUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels
{
	internal class RecentProjectViewModel:ViewModelBase
	{
		private readonly RecentProject _recentProject;
		public string FileName => _recentProject.FileName;
		public string Date => _recentProject.Date.ToString("g");
		public string Directory=>_recentProject.Directory;
		public RecentProjectViewModel(RecentProject recentProject)
		{
			_recentProject = recentProject;
		}
	}
}
