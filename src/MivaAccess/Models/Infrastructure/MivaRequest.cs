using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Shared;
using Newtonsoft.Json;
using System;

namespace MivaAccess.Models.Infrastructure
{
	public abstract class MivaRequest
	{
		[ JsonProperty( "Store_Code" ) ]
		public string StoreCode { get; private set; }

		[ JsonProperty( "Function" ) ]
		public string FunctionName { get; private set; }

		[ JsonProperty( "Miva_Request_Timestamp" ) ]
		public int Timestamp { get; private set; }

		public MivaRequest( MivaCredentials credentials, string functionName )
		{
			Condition.Requires( credentials, "credentials" ).IsNotNull();
			Condition.Requires( functionName, "functionName" ).IsNotNullOrEmpty();

			this.StoreCode = credentials.StoreCode;
			this.FunctionName = functionName;
			
			UpdateTimestamp();
		}

		protected void UpdateTimestamp()
		{
			this.Timestamp = DateTime.UtcNow.FromUtcTimeToEpoch();
		}
	}
}