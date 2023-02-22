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
	internal interface IDragDestination
	{
		public FrameworkElement DragDestinationView { get; }
		public ConnectionLine? LineEntering { get; set; }
		public IDragSource? LinkingFrom { get; set; }
		public Point Offset { get; }
		public DiagramElementModel DestinationModel { get; }
		public void OnLineEnteringDestroyed(ConnectionLine line);
		public event Action<ViewModelBase>? PositionChanged;
	}
}
