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
		public IDragDestination? DragDestination { get; set; }
		public bool Dragging { get; set; }
		public Point StartPos { get; set; }
		public Point EndPos { get; set; }
		public void Clear()
		{
			DragSource = null;
			DragDestination = null;
			Dragging = false;
			StartPos = new();
			EndPos = new();
		}
	}
}
