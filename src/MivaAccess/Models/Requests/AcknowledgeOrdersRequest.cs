using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MivaAccess.Models.Requests
{
	public class AcknowledgeOrdersRequest : MivaModuleQuery
	{
		[ JsonProperty( "Order_Ids" ) ]
		public IEnumerable< long > OrdersIds { get; private set; }

		public AcknowledgeOrdersRequest( MivaCredentials credentials, IEnumerable< long > ordersIds ) : base( credentials, "orderworkflow", "OrderList_Acknowledge" )
		{
			Condition.Requires( ordersIds, "ordersIds" ).IsNotEmpty();

			this.OrdersIds = ordersIds;
		}
	}
}