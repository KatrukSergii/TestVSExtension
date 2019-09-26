using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextExtension.Commands.Handlers
{
	public interface ICommandHandler
	{
		Task ExecuteAsync();

		void BeforeQueryStatus(object sender, EventArgs e);
	}
}
