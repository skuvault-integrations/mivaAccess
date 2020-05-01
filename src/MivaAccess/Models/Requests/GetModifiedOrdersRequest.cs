using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;
using Newtonsoft.Json;

namespace MivaAccess.Models.Requests
{
	public class GetOrdersFromQueueRequest : MivaModuleQuery
	{
		[ JsonProperty( "Queue_Code" ) ]
		public string QueueName { get; private set; }

		public GetOrdersFromQueueRequest( MivaCredentials credentials, string queueName ) : base( credentials, "orderworkflow", "QueueOrderList_Load_Query" )
		{
			Condition.Requires( queueName, "queueName" ).IsNotNullOrEmpty();

			this.Filters = new MivaRequestFilter[]
			{
				new MivaRequestOnDemandFilter( new string[] { "ship_method", "items", "charges", "discounts", "notes" } )
			};
			this.QueueName = queueName;
		}
	}
}