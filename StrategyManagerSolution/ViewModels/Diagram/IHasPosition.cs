using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal interface IHasPosition
	{
		Point CanvasPos { get; set; }
		FrameworkElement PositionView { get; }
	}
}
