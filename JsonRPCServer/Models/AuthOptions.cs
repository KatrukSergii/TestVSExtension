namespace JsonRPCServer.Models
{
	public class AuthOptions
	{
		public AuthType AuthType
		{
			get;
			set;
		}
		public string IntegratorKey
		{
			get;
			set;
		}
		public override string ToString()
		{
			return $"Auth type: {this.AuthType}, IntegratorKey: {IntegratorKey}.";
		}
	}
}