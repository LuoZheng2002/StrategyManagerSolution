using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace StrategyManagerSolution.Utils
{
	public static class Cmd
	{
		public static string ExecuteCommand(string command, string? workingDirectory = null)
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
			string outputStr = "";
			process.OutputDataReceived += (s, e) => outputStr += e.Data + "\n";
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();
			Console.Write(outputStr);
			return outputStr;
		}
	}
}
