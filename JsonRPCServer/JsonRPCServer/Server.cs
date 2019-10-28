using System;
using System.IO;
using System.Threading.Tasks;
using JsonRPCServer.Handlers;
using StreamJsonRpc;

namespace JsonRPCServer.JsonRPCServer
{
	public class Server
	{
		public static void StartAsync()
		{
			Task nowait = ResponseToRpcRequestsAsync();
			while (true)
			{

			}
		}

		private static async Task ResponseToRpcRequestsAsync()
		{
			Stream inp = Console.OpenStandardInput();
			Stream outp = Console.OpenStandardOutput();
			var jsonRpc = JsonRpc.Attach(outp, inp, new ConfigureAuthentificationHandler());
			await jsonRpc.Completion;
		}
	}
}
