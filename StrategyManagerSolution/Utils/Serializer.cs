using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Utils
{
	public static class Serializer
	{
		private static JsonSerializerOptions options = new()
		{
			WriteIndented = true,
			ReferenceHandler = ReferenceHandler.Preserve
		};
		public static void Serialize<T>(string filename, T data)
		{
			string result = JsonSerializer.Serialize(data, options);
			File.WriteAllText(filename, result);
		}
		public static T Deserialize<T>(string filename)
		{
			string data = File.ReadAllText(filename);
			T? result = JsonSerializer.Deserialize<T>(data, options);
			if (result == null)
			{
				throw new Exception("Failed to deserialize!");
			}
			return result!;
		}
	}
}
