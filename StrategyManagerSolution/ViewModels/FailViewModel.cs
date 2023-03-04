using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.ViewModels
{
	internal class FailViewModel:ViewModelBase, IStatus
	{
		public ImageSource FailImageSource { get; } = new BitmapImage(new Uri("../../../Images/fail.jpg",UriKind.Relative));
        public UserControl? View { get; set; }
        public Command ViewDetailCommand { get; }
        public event Action? ViewDetail;
        public Command LoadedCommand { get; }
        public FailViewModel()
        {
            double a = FailImageSource.Width;
            ViewDetailCommand = new Command((_)=>ViewDetail?.Invoke());
            LoadedCommand = new Command(OnLoaded);
        }
		private void OnLoaded(object? obj)
		{
			RoutedEventArgs e = (obj as RoutedEventArgs)!;
			View = (FailView)e.Source;
		}

	}
}
