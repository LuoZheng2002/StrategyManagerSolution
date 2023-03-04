using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __NAMESPACENAME2__
{
    public class __CLASSNAME__: SwitchModuleBase
    {
		// string: Case名称, Func<GameModelBase, bool>: Case判断函数
		public override Dictionary<string, Func<GameModelBase, bool>> Cases { get; } = new()
		{
			// to do
		};
	}
}
