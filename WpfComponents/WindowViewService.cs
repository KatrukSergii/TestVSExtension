using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel;

namespace WpfComponents
{
	public class WindowViewService : IViewService
	{
		private readonly Window window;

		public WindowViewService(Window window)
		{
			this.window = window;
		}

		public void CloseWindow()
		{
			this.window.DialogResult = true;
			this.window.Close();
		}
	}
}
