using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ViewModel
{
	public class Command : ICommand
	{
		private readonly Action executeAction;

		public Command(Action executeAction)
		{
			this.executeAction = executeAction;
		}
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter) => this.executeAction?.Invoke();
	}
}
