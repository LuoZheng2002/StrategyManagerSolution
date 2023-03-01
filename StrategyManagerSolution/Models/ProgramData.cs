using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Models
{
	internal class ProgramData
	{
        public List<RecentProject> RecentProjects { get; set; } = new();
        public ProgramData()
        {
            
        }
    }
}
