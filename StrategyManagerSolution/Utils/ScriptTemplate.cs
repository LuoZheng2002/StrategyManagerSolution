using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.Utils
{
    internal static class ScriptTemplate
    {
        public static string StrategyTemplate(string namespaceName, string strategyClassName)
        {
            string template = File.ReadAllText("../../../ScriptTemplates/StrategyTemplate.txt");
            template = template.Replace("__CLASSNAME__", strategyClassName);
            template = template.Replace("__NAMESPACENAME__", namespaceName);
            return template;
		}
        public static string IfTemplate(string namespaceName, string ifClassName)
        {
            string template = File.ReadAllText("../../../ScriptTemplates/IfTemplate.txt");
            template = template.Replace("__CLASSNAME__", ifClassName);
            template = template.Replace("__NAMESPACENAME__", namespaceName);
            return template;
        }
		public static string SwitchTemplate(string namespaceName, string switchClassName)
		{
			string template = File.ReadAllText("../../../ScriptTemplates/SwitchTemplate.txt");
			template = template.Replace("__CLASSNAME__", switchClassName);
			template = template.Replace("__NAMESPACENAME__", namespaceName);
			return template;
		}
	}
}
