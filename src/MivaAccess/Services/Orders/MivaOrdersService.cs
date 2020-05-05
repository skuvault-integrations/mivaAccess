using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MivaAccess.Configuration;
using MivaAccess.Exceptions;
using MivaAccess.Models;
using MivaAccess.Models.Infrastructure;
using MivaAccess.Models.Requests;
using MivaAccess.Shared;

namespace MivaAccess.Services.Orders
{
	public class MivaOrdersService : ServiceBase, IMivaOrdersService
	{
		public MivaOrdersService( MivaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< MivaOrder > > GetModifiedOrdersFromQueueAsync( string queueName, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Get modified orders request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var orders = new List< MivaOrder >();
			var pageIndex = 0;

			while( true )
			{
				var ordersFromPage = await CollectOrdersFromPage( queueName, pageIndex, base.Config.OrdersPageSize, token, mark ).ConfigureAwait( false );

				if ( !ordersFromPage.Any() )
				{
					break;
				}

				orders.AddRange( ordersFromPage );
				++pageIndex;
			}

			return orders;
		}

		private async Task< IEnumerable< MivaOrder > > CollectOrdersFromPage( string queueName, int pageIndex, int pageSize, CancellationToken token, Mark mark )
		{
			var request = new GetOrdersFromQueueRequest( base.Config.Credentials, queueName );
			request.SetPage( pageIndex, pageSize );

			var response = await base.PostAsync< MivaDataResponse< IEnumerable< Order > > >( request, token, mark ).ConfigureAwait( false );

			if ( response.Success == 1 )
			{
				if ( response.Data?.Data != null && response.Data.Data.Any() )
				{
					return response.Data.Data.Select( o => o.ToSVOrder() );
				}

				return Array.Empty< MivaOrder >();
			}

			throw new MivaException( response.ErrorMessage, response.ErrorCode );
		}

		public async Task AcknowledgeOrders( IEnumerable< long > ordersIds, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Acknowledge orders request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var request = new AcknowledgeOrdersRequest( base.Config.Credentials, ordersIds );
			var response = await base.PostAsync< MivaResponse >( request, token, mark ).ConfigureAwait( false );

			if ( response.Success == 0
				|| ( response.Processed != null && response.Processed == 0 ) )
			{
				throw new MivaException( response.ErrorMessage, response.ErrorCode );
			}
		}
	}
}