using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MivaAccess.Configuration;
using MivaAccess.Exceptions;
using MivaAccess.Models;
using MivaAccess.Models.Infrastructure;
using MivaAccess.Models.Requests;
using MivaAccess.Shared;
using SkuVault.Integrations.Core.Extensions;

namespace MivaAccess.Services.Orders
{
	public class MivaOrdersService : ServiceBase, IMivaOrdersService
	{
		/// <summary>
		/// We use batching for AcknowledgeOrders to avoid the MIVA_gateway_timeout_error (see GUARD-2666 and GUARD-2601)
		/// </summary>
		public const int AcknowledgeOrdersBatchLimit = 100;
		public MivaOrdersService( MivaConfig config ) : base( config )
		{
		}

		public async Task<IEnumerable<MivaOrder>> GetModifiedOrdersFromQueueAsync(string queueName, Mark mark, CancellationToken cancellationToken)
		{
			LogIfCancellationTokenRequested(mark, cancellationToken);

			var orders = new List<MivaOrder>();
			var pageIndex = 0;

			while (true)
			{
				var ordersFromPage = await CollectOrdersFromPageAsync(queueName, pageIndex, base.Config.OrdersPageSize, mark, cancellationToken).ConfigureAwait(false);

				if (!ordersFromPage.Any())
				{
					break;
				}

				orders.AddRange(ordersFromPage);
				++pageIndex;
			}

			return orders;
		}

		public async Task AcknowledgeOrdersAsync(IEnumerable<long> ordersIds, Mark mark, CancellationToken cancellationToken)
		{
			var chunks = ordersIds.SplitInBatches(AcknowledgeOrdersBatchLimit);
			foreach (var chunk in chunks)
			{
				LogIfCancellationTokenRequested(mark, cancellationToken);

				var request = new AcknowledgeOrdersRequest(base.Config.Credentials, chunk);
				var response = await base.PostAsync<MivaResponse>(request, cancellationToken, mark).ConfigureAwait(false);

				if (response.Success == 0
					|| (response.Processed != null && response.Processed == 0))
				{
					throw new MivaException(response.ErrorMessage, response.ErrorCode);
				}
			}
		}

		private async Task<IEnumerable<MivaOrder>> CollectOrdersFromPageAsync(string queueName, int pageIndex, int pageSize, Mark mark, CancellationToken token)
		{
			var request = new GetOrdersFromQueueRequest(base.Config.Credentials, queueName);
			request.SetPage(pageIndex, pageSize);

			var response = await base.PostAsync<MivaDataResponse<IEnumerable<Order>>>(request, token, mark).ConfigureAwait(false);

			if (response.Success == 1)
			{
				if (response.Data?.Data != null && response.Data.Data.Any())
				{
					return response.Data.Data.Select(o => o.ToSVOrder());
				}

				return Array.Empty<MivaOrder>();
			}

			throw new MivaException(response.ErrorMessage, response.ErrorCode);
		}

		private void LogIfCancellationTokenRequested(Mark mark, CancellationToken cancellationToken, [CallerMemberName] string callerMethodName = "")
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return;
			}

			var exceptionDetails = CreateMethodCallInfo(base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo());
			var mivaException = new MivaException($"{exceptionDetails}. {callerMethodName} method request was cancelled");
			MivaLogger.LogTraceException(mivaException);
		}
	}
}