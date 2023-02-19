using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Models
{
	internal class RecentProject
	{
		public string FileName { get; }
		public string Directory { get; }
		public DateTime Date { get; }
		public RecentProject(string fileName, string directory, DateTime date)
		{
			FileName = fileName;
			Directory = directory;
			Date = date;
		}
		
	}
}
