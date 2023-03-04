using Contracts;
using Contracts.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __NAMESPACENAME__
{
	public class __CLASSNAME__: ProjSlnFuncProviderBase
	{
		public override ProjectSolution ProjectSolution { get; }
        public __CLASSNAME__()
        {
            ProjectSolution = new ProjectSolution();

			/*__DECLARESOLUTIONS__*/
			/*__CONFIGURESOLUTIONS__*/
			/*__ADDSOLUTIONS__*/
			ProjectSolution.MainSolution = __MAINSOLUTION__;
		}
	}
}
