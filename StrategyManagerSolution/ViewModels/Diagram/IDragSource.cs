using StrategyManagerSolution.DiagramMisc;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManagerSolution.MVVMUtils;
using Contracts.MVVMModels;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal interface IDragSource
	{
		public FrameworkElement DragSourceView { get; }
		public ConnectionLine? LineLeaving { get; set; }
		public IDragDestination? LinkingTo { get; set; }
		public DiagramElementModel? ModelLinkingTo { get; set; }
		public Point Offset { get;}
		public void OnLineLeavingDestroyed(ConnectionLine line);
		public event Action<ViewModelBase>? PositionChanged;
	}
}
