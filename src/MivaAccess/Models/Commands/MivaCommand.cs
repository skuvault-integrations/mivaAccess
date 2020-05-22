using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;

namespace MivaAccess.Models.Commands
{
	public class MivaCommand
	{
		public MivaConfig Config { get; private set; }
		public MivaRequest Payload { get; private set; }
		public string Url { get; private set; }

		public MivaCommand( MivaConfig config, MivaRequest payload )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( payload, "payload" ).IsNotNull();

			this.Config = config;
			this.Payload = payload;
			this.Url = config.ApiBaseUrl;
		}
	}
}