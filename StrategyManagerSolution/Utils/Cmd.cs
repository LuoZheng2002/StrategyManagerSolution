using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Utils
{
	public static class Cmd
	{
		public static void ExecuteCommand(string command)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = $"/c {command}";
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
		}
	}
}
