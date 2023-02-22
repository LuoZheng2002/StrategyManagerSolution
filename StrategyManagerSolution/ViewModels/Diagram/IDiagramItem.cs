using Contracts.MVVMModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StrategyManagerSolution.ViewModels.Diagram
{
	internal interface IDiagramItem
	{
		Point CanvasPos { get; set; }
		UserControl ViewRef { get; }
		DiagramElementModel ModelRef { get; }
	}
}
