using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MivaAccess.Configuration;
using MivaAccess.Exceptions;
using MivaAccess.Models;
using MivaAccess.Models.Commands;
using MivaAccess.Models.Requests;
using MivaAccess.Shared;

namespace MivaAccess.Services.Products
{
	public class MivaProductsService : ServiceBase, IMivaProductsService
	{
		public MivaProductsService( MivaConfig config ) : base( config )
		{
		}

		public async Task< IEnumerable< MivaProduct > > GetProductsCreatedOrUpdatedAfterAsync( DateTime lastModifiedDateUtc, CancellationToken token )
		{
			var mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( "", mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Get modified products request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var request = new GetModifiedProductsRequestBody( base.Config.Credentials, lastModifiedDateUtc );
			var command = new MivaCommand( base.Config, request.ToJson() );

			var response = await base.PostAsync< IEnumerable< Product > >( command, token, mark ).ConfigureAwait( false );

			return response.Select( r => r.ToSVProduct() );
		}
	}
}