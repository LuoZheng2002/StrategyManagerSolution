using StrategyManagerSolution.ViewModels;
using StrategyManagerSolution.ViewModels.Diagram;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.DiagramMisc
{
	internal class DragInfo
	{
		public StrategySetViewModel? StrategySetFrom { get; set; }
		public StrategySetViewModel? StrategySetTo { get; set; }
		public StrategyViewModel? StrategyFrom { get; set; }
		public bool Dragging { get; set; }
		public Point StartPos { get; set; }
		public void Clear()
		{
			StrategySetFrom = null;
			StrategySetTo = null;
			StrategyFrom = null;
			Dragging = false;
		}
	}
}
