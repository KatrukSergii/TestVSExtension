using JsonRPCServer.Models;

namespace JsonRPCServer.Handlers
{
	public class ConfigureAuthentificationHandler
	{
		public string ConfigureProjectAuth(AuthOptions parameters) => parameters.ToString();

		public string ConfigureProject(string parameters) => $"Parameters: {parameters}";
	}
}