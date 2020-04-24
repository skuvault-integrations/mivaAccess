using CuttingEdge.Conditions;
using MivaAccess.Configuration;

namespace MivaAccess.Models.Commands
{
	public class MivaCommand
	{
		public MivaConfig Config { get; private set; }
		public string Payload { get; private set; }
		public string Url { get; private set; }

		public MivaCommand( MivaConfig config, string payload )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( payload, "payload" ).IsNotNullOrEmpty();

			this.Config = config;
			this.Payload = payload;
			this.Url = config.ApiBaseUrl;
		}
	}
}