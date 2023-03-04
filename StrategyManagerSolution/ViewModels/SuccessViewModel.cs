using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StrategyManagerSolution.ViewModels
{
	internal class SuccessViewModel:ViewModelBase, IStatus
	{
		public ImageSource SuccessImageSource { get; } = new BitmapImage(new Uri("../../../Images/success.jpg", UriKind.Relative));
        public UserControl? View { get; set; }
        public Command LoadedCommand { get; }
        public SuccessViewModel()
        {
            double a = SuccessImageSource.Width;
            LoadedCommand = new Command(OnLoaded);
        }
		private void OnLoaded(object? obj)
		{
			RoutedEventArgs e = (obj as RoutedEventArgs)!;
			View = (SucceedView)e.Source;
		}
	}
}
