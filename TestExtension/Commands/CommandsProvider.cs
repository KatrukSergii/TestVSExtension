using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using TextExtension.Commands.Handlers;
using Task = System.Threading.Tasks.Task;

namespace TextExtension.Commands
{
	public class CommandsProvider
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int AddCustomClassCommandId = 0x1220;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("aa2f00ca-b4bd-4532-baa7-a2faf6349f63");

		public static async System.Threading.Tasks.Task InitializeCommandsAsync(IMenuCommandService menuCommandService, AsyncPackage package)
		{
			Dictionary<int, ICommandHandler> commandHandlers = new Dictionary<int, ICommandHandler>
			{
				{ AddCustomClassCommandId, new AddCustomClassHandler(package) }
			};

		 	CommandsProvider.InitCommandsInternal(commandHandlers, menuCommandService);
		}

		private static void InitCommandsInternal(Dictionary<int, ICommandHandler> commandHandlers, IMenuCommandService commandService)
		{
			foreach (var command in commandHandlers)
			{
				var menuCommandID = new CommandID(CommandSet, command.Key);
				var menuItem = new OleMenuCommand((_, __) => executeCommand(command.Value), menuCommandID);
				menuItem.BeforeQueryStatus += command.Value.BeforeQueryStatus;

				commandService.AddCommand(menuItem);

				async void executeCommand(ICommandHandler commandHandler)
				{
					await commandHandler.ExecuteAsync();
				}
			}
		}

	}
}
