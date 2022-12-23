using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MivaAccess.Models;
using MivaAccess.Shared;

namespace MivaAccess.Services.Orders
{
	public interface IMivaOrdersService
	{
		Task<IEnumerable<MivaOrder>> GetModifiedOrdersFromQueueAsync(string queueName, Mark mark, CancellationToken cancellationToken);
		Task AcknowledgeOrdersAsync(IEnumerable<long> ordersIds, Mark mark, CancellationToken cancellationToken);
	}
}