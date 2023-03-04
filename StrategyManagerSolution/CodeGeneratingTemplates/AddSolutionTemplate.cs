using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.CodeGeneratingTemplates
{
	internal class AddSolutionTemplate
	{
		public Solution __SOLUTIONNAME__ = new Solution();
		public ProjectSolution ProjectSolution = new();
        public AddSolutionTemplate()
        {
			//start
			ProjectSolution.Solutions.Add(__SOLUTIONNAME__);

			//end
		}
    }
}
