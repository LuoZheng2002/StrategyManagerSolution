using StrategyManagerSolution.MVVMUtils;
using StrategyManagerSolution.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerSolution.ViewModels
{
	internal class PopupViewModel:ViewModelBase
	{
		private PopupWindow _popupWindow;
		public ViewModelBase Content { get; }
		public Command OKCommand { get; }
		public Command CancelCommand { get; }
		public PopupViewModel(PopupWindow popupWindow, ViewModelBase content)
		{
			_popupWindow = popupWindow;
			Content = content;
			OKCommand = new(OnOK);
			CancelCommand = new(OnCancel);
		}
		private void OnOK(object? obj)
		{
			_popupWindow.DialogResult = true;
			_popupWindow.Close();
		}
		private void OnCancel(object? obj)
		{
			_popupWindow.DialogResult = false;
			_popupWindow.Close();
		}
	}
}
