using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtension.Commands.Handlers;

namespace VSIXProject2.Commands.Handlers
{
	public abstract class CommandHandlerBase : ICommandHandler
	{
		public async void BeforeQueryStatus(object sender, EventArgs e)
		{
			if (!(sender is MenuCommand command))
				return;

			command.Enabled = await this.CanExecute();
		}

		protected virtual async Task<bool> CanExecute() => true;

		public virtual async Task ExecuteAsync()
		{

		}
	}
}
