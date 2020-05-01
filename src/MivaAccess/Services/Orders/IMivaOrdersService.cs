using MivaAccess.Models;
using MivaAccess.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccess.Services.Orders
{
	public interface IMivaOrdersService
	{
		Task< IEnumerable< MivaOrder > > GetModifiedOrdersFromQueueAsync( string queueName, CancellationToken token, Mark mark = null );
		Task AcknowledgeOrders( IEnumerable< long > ordersIds, CancellationToken token, Mark mark = null );
	}
}