using MivaAccess.Models;
using MivaAccess.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccess.Services.Products
{
	public interface IMivaProductsService
	{
		/// <summary>
		///	List all products that were modified since specified date
		/// </summary>
		/// <param name="startDateUtc"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task< IEnumerable< MivaProduct > > GetProductsCreatedOrUpdatedAfterAsync( DateTime startDateUtc, CancellationToken token, Mark mark = null );
		/// <summary>
		///	Get product's inventory by sku. Sku is not unique in Miva.
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		Task< IEnumerable< MivaProduct > > FindProductBySku( string sku, CancellationToken token, Mark mark = null );

		/// <summary>
		///	Update product's quantity
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="warehouseLocationCountryCode"></param>
		/// <param name="quantity"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		Task UpdateProductQuantityBySkuAsync( string sku, int quantity, CancellationToken token, Mark mark = null );

		/// <summary>
		///	Update products quantities
		/// </summary>
		/// <param name="skusQuantities"></param>
		/// <param name="warehouseLocationCode"></param>
		/// <param name="token"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		Task UpdateProductsQuantitiesBySkuAsync( Dictionary< string, int > skusQuantities, CancellationToken token, Mark mark = null );
	}
}