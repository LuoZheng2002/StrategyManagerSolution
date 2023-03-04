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
	internal class UnstartedViewModel:ViewModelBase, IStatus
	{
		public ImageSource UnstartedImageSource { get; } = new BitmapImage(new Uri("../../../Images/unstarted.jpg", UriKind.Relative));
        public UserControl? View { get;set; }
        public Command LoadedCommand { get; }
        public UnstartedViewModel()
        {
            double a = UnstartedImageSource.Width;
            LoadedCommand = new Command(OnLoaded);
        }

		private void OnLoaded(object? obj)
		{
			RoutedEventArgs e = (obj as RoutedEventArgs)!;
			View = (UnstartedView)e.Source;
		}
	}
}
