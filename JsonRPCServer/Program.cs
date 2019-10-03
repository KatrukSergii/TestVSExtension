using System;
using JsonRPCServer.JsonRPCServer;

namespace JsonRPCServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Server.StartAsync();
		}
	}
}
