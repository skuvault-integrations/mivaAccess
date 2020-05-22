using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;
using Newtonsoft.Json;

namespace MivaAccess.Models.Requests
{
	public sealed class UpdateProductInventoryRequest : MivaRequest
	{
		[ JsonProperty( "Product_Sku" ) ]
		public string ProductSku { get; private set; }

		[ JsonProperty( "Product_Inventory" ) ]
		public int ProductInventory { get; private set; }

		public UpdateProductInventoryRequest( MivaCredentials credentials, string sku, int quantity ) : base( credentials, "Product_Update" )
		{
			Condition.Requires( sku, "sku" ).IsNotNullOrEmpty();
			Condition.Requires( quantity, "quantity" ).IsGreaterOrEqual( 0 );

			this.ProductSku = sku;
			this.ProductInventory = quantity;
		}
	}
}