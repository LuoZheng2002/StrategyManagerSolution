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
		public static void ExecuteCommand(string command, string? workingDirectory = null)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = $"/c {command}";
			process.StartInfo.RedirectStandardOutput = true;
			if (workingDirectory != null)
			{
				process.StartInfo.WorkingDirectory = workingDirectory;
			}			
			process.Start();
			while(!process.StandardOutput.EndOfStream)
			{
				Console.Write(process.StandardOutput.ReadLine());
			}
			
		}
	}
}
