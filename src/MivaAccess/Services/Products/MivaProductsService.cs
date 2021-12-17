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
using MivaAccess.Models.Infrastructure;
using MivaAccess.Models.Requests;
using MivaAccess.Shared;

namespace MivaAccess.Services.Products
{
	public class MivaProductsService : ServiceBase, IMivaProductsService
	{
		public MivaProductsService( MivaConfig config ) : base( config )
		{
		}

		/// <summary>
		///	List all products that were modified since the specified date
		/// </summary>
		/// <param name="lastModifiedDateUtc"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< MivaProduct > > GetProductsUpdatedAfterAsync( DateTime lastModifiedDateUtc, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Get modified products request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var products = new List< MivaProduct >();
			var pageIndex = 0;

			AddLog( "Get Products Started", this.CreateMethodCallInfo( mark: mark, url: base.Config.ApiBaseUrl, additionalInfo: this.AdditionalLogInfo() ) );

			while( true )
			{
				var productsFromPage = await CollectProductsFromPage( lastModifiedDateUtc, pageIndex, base.Config.ProductsPageSize, token, mark ).ConfigureAwait( false );

				AddLog( "Get Products Retry", this.CreateMethodCallInfo( mark: mark, url: base.Config.ApiBaseUrl,responseBodyRaw: productsFromPage.ToJson(), additionalInfo: this.AdditionalLogInfo() ) );

				if ( !productsFromPage.Any() )
				{
					break;
				}

				products.AddRange( productsFromPage );
				++pageIndex;
			}

			AddLog( "Get Products Finished", this.CreateMethodCallInfo( mark: mark, url: base.Config.ApiBaseUrl, additionalInfo: this.AdditionalLogInfo() ) );

			return products;
		}

		private async Task< IEnumerable< MivaProduct > > CollectProductsFromPage( DateTime lastModifiedDateUtc, int pageIndex, int pageSize, CancellationToken token, Mark mark )
		{
			var request = new GetModifiedProductsRequest( base.Config.Credentials, lastModifiedDateUtc );
			request.SetPage( pageIndex, pageSize );

			AddLog( "Products Request", this.CreateMethodCallInfo( mark: mark, url: base.Config.ApiBaseUrl, responseBodyRaw: request.ToJson(), additionalInfo: this.AdditionalLogInfo() ) );

			var response = await base.PostAsync< MivaDataResponse < IEnumerable< Product > > >( request, token, mark ).ConfigureAwait( false );

			AddLog( "Products Response", this.CreateMethodCallInfo( mark: mark, url: base.Config.ApiBaseUrl, responseBodyRaw: response.ToJson(), additionalInfo: this.AdditionalLogInfo() ) );

			if ( response.Success == 0 )
			{
				throw new MivaException( response.ErrorMessage, response.ErrorCode );
			}

			if ( response.Data?.Data != null )
			{
				return response.Data.Data.Select( r => r.ToSVProduct( base.Config.Credentials ) );
			}

			return Array.Empty< MivaProduct >();
		}

		/// <summary>
		///	Find product by sku
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task< IEnumerable< MivaProduct > > FindProductBySku( string sku, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Find product by sku request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var request = new FindProductBySkuRequest( base.Config.Credentials, sku );
			var response = await base.PostAsync< MivaDataResponse < IEnumerable< Product > > >( request, token, mark ).ConfigureAwait( false );

			if ( response.Data?.Data != null )
			{
				return response.Data.Data.Select( r => r.ToSVProduct( base.Config.Credentials ) );
			}

			return Array.Empty< MivaProduct >();
		}

		/// <summary>
		///	Update product's quantity using sku
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="quantity"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateProductQuantityBySkuAsync( string sku, int quantity, CancellationToken token, Mark mark = null )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Update product's sku quantity request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var request = new UpdateProductInventoryRequest( base.Config.Credentials, sku, quantity );
			var response = await base.PostAsync< MivaResponse >( request, token, mark ).ConfigureAwait( false );

			if ( response.Success == 0 )
			{
				MivaLogger.LogTrace( new MivaException( response.ErrorMessage, response.ErrorCode ), string.Format( "Failed to update product's quantity! Sku: {0}, New quantity: {1}", sku, quantity ) );
				return;
			}
		}

		/// <summary>
		///	Update products quantities via batch request
		/// </summary>
		/// <param name="skusQuantities"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public async Task UpdateProductsQuantitiesBySkuAsync( Dictionary< string, int > skusQuantities, CancellationToken token, Mark mark = null )
		{
			if ( skusQuantities == null || !skusQuantities.Any() )
				return;

			if ( mark == null )
				mark = Mark.CreateNew();

			if ( token.IsCancellationRequested )
			{
				var exceptionDetails = CreateMethodCallInfo( base.Config.ApiBaseUrl, mark, additionalInfo: this.AdditionalLogInfo() );
				var mivaException = new MivaException( string.Format( "{0}. Update products quantities request was cancelled", exceptionDetails ) );
				MivaLogger.LogTraceException( mivaException );
			}

			var chunks = skusQuantities.SplitToChunks( base.Config.InventoryUpdateBatchSize );

			foreach( var chunk in chunks )
			{
				var request = new UpdateProductsInventoryBatchRequest( base.Config.Credentials, chunk );
				var responses = await base.PostAsync< IEnumerable< MivaResponse > >( request, token, mark ).ConfigureAwait( false );

				if ( responses != null && responses.Any() )
				{
					for ( int i = 0; i < responses.Count(); i++ )
					{
						var response = responses.ElementAt( i );
					
						if ( response.Success == 0 )
						{
							MivaLogger.LogTrace( new MivaException( response.ErrorMessage, response.ErrorCode ), 
											string.Format( "Failed to update product {0} quantity to {1}", chunk.ElementAt( i ).Key, chunk.ElementAt( i ).Value ) );
						}
					}
				}
			}
		}

		private void AddLog( string message, string details )
		{
			var info = new MivaException( string.Format( "{0}: {1}", message, details ) );
			MivaLogger.LogTraceException( info );
		}
	}
}