using Contracts.BaseClasses;
using Contracts.Communication;
using System.Diagnostics;
using System.Reflection;
namespace StrategyTester
{
	class Program
	{
		public static Assembly? SolutionAssembly { get; set; }
		public static Assembly? TestAssembly { get; set; }
		public static Type? FuncProviderType { get; set; }
		public static ProjSlnFuncProviderBase? ProjSlnFuncProvider { get; set; }
		public static Type? TestAssemblyClassType { get; set; }
		public static TesterBase? Tester { get; set; }
		public static void ReceiveMessage()
		{
			string line = Console.ReadLine()!;
			string[] segments = line.Split(" ");
			switch (segments[0])
			{
				case TextConvention.ProjSlnPath:
					{
						string projslnpath = segments[1];
						OnProjSlnPathGiven(projslnpath);
						break;
					}
				case TextConvention.FuncProviderFullName:
					{
						string funcProviderFullName = segments[1];
						OnFuncProviderFullNameGiven(funcProviderFullName);
						break;
					}
				case TextConvention.Integrity:
					{
						OnCheckIntegrity();
						break;
					}
				case TextConvention.TestAssemblyPath:
					{
						string testAssemblyPath = segments[1];
						OnTestAssemblyPathGiven(testAssemblyPath);
						break;
					}
				case TextConvention.TestAssemblyClassFullName:
					{
						string testAssemblyClassFullName = segments[1];
						OnTestAssemblyClassFullNameGiven(testAssemblyClassFullName);
						break;
					}
				case TextConvention.Start:
					{
						OnStart();
						break;
					}
				case TextConvention.LeftButtonDown:
					{
						OnLeftButtonDown(double.Parse(segments[1]), double.Parse(segments[2]));
						break;
					}
				case TextConvention.RightButtonDown:
					{
						OnRightButtonDown(double.Parse(segments[1]), double.Parse(segments[2]));
						break;
					}
				case TextConvention.ConsoleInput:
					{
						string input = line.Replace(TextConvention.ConsoleInput + " ", "");
						Tester!.OnReceiveMessage(input);
						break;
					}
				default:
					{
						throw new Exception("Unrecognized command");
					}
			}
		}
		private static void OnLeftButtonDown(double x, double y)
		{
			Tester!.OnLeftButtonDown(x, y);
		}
		private static void OnRightButtonDown(double x, double y)
		{
			Tester!.OnRightButtonDown(x, y);
		}
		private static void OnStart()
		{
			Console.WriteLine(TextConvention.OK);
			if (Tester == null)
				throw new Exception("Tester not set.");
			Tester.ProjSlnFuncProvider = ProjSlnFuncProvider;
			Tester!.Init();
		}
		private static void OnTestAssemblyClassFullNameGiven(string fullName)
		{
			if (TestAssembly == null)
				throw new Exception("test assembly not given.");
			TestAssemblyClassType = TestAssembly.GetType(fullName);
			if (TestAssemblyClassType == null)
				throw new Exception("Cannot get type from assembly");
			Tester = (TesterBase)Activator.CreateInstance(TestAssemblyClassType)!;
			if (Tester == null)
				throw new Exception("Cannot get instance of tester.");
			Console.WriteLine(TextConvention.OK);
		}
		private static void OnTestAssemblyPathGiven(string testAssemblyPath)
		{
			if (TestAssembly !=null)
				throw new Exception("Assembly path already given.");
			TestAssembly = Assembly.LoadFrom(testAssemblyPath);
			if (TestAssembly == null)
				throw new Exception("Cannot load assembly");
			Console.WriteLine(TextConvention.OK);
		}
		public static void Main(string[] args)
		{
			while(true)
			{
				try
				{
					ReceiveMessage();
				}
				catch(Exception e)
				{
					Console.WriteLine(TextConvention.Error + " " + e.Message);
				}
			}
			
		}
		private static void OnProjSlnPathGiven(string path)
		{
			if (SolutionAssembly != null)
				throw new Exception("Assembly path already given.");
			SolutionAssembly = Assembly.LoadFrom(path);
			if (SolutionAssembly == null)
				throw new Exception("Cannot load assembly");
			Console.WriteLine(TextConvention.OK);
		}
		private static void OnCheckIntegrity()
		{
			if (SolutionAssembly == null)
				throw new Exception("Failed to load assembly");
			if (FuncProviderType == null)
				throw new Exception("Failed to load type from assembly");
			if (ProjSlnFuncProvider == null)
				throw new Exception("Cannot get instance of func provider.");
			Console.WriteLine(TextConvention.Integrity + " " + TextConvention.OK + " " + "fuck");
		}
		private static void OnFuncProviderFullNameGiven(string name)
		{
			if (SolutionAssembly == null)
			throw new Exception("assembly path not given.");
			FuncProviderType = SolutionAssembly.GetType(name);
			if (FuncProviderType == null)
				throw new Exception("Cannot get type from assembly");
			ProjSlnFuncProvider = (ProjSlnFuncProviderBase)Activator.CreateInstance(FuncProviderType)!;
			if (ProjSlnFuncProvider == null)
				throw new Exception("Cannot get instance of func provider");
			Console.WriteLine(TextConvention.OK);
		}
	}
}
