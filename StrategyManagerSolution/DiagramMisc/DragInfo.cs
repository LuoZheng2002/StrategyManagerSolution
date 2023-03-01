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
		public IDragSource? DragSource { get; set; }
		public bool Dragging { get; set; }
		public void Clear()
		{
			DragSource = null;
			Dragging = false;
		}
	}
}
