using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Utils
{
	public static class DLLLoader
	{
		public static T LoadDLL<T>(string directory, string fullTypeName)
		{
			AssemblyLoadContext context = new AssemblyLoadContext("assemblycontext", true);
			Assembly assembly = context.LoadFromAssemblyPath(directory);
			Type? type = assembly.GetType(fullTypeName);
			if (type == null) throw new Exception("Cannot find specified type");
			T? result = (T?)Activator.CreateInstance(type);
			if (result == null) throw new Exception("Cannot create instance or type doesn't match.");
			context.Unload();
			return result;
		}
	}
}
