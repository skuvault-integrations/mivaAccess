using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MivaAccess.Models.Requests
{
	public class UpdateProductsInventoryBatchRequest : MivaBatchRequest
	{
		public UpdateProductsInventoryBatchRequest( MivaCredentials credentials, Dictionary< string, int > inventory ) 
			: base ( credentials, "Product_Update" )
		{
			Condition.Requires( inventory, "inventory" ).IsNotEmpty();

			this.Iterations = this.GenerateIterations( inventory );
		}

		private IEnumerable< IMivaBatchIteration > GenerateIterations( Dictionary< string, int > inventory )
		{
			return inventory.Select( pair => new UpdateProductBatchIteration() { ProductSku = pair.Key, ProductInventory = pair.Value } ).ToList();
		}
	}

	public class UpdateProductBatchIteration : IMivaBatchIteration
	{
		[ JsonProperty( "Product_Sku" ) ]
		public string ProductSku { get; set; }

		[ JsonProperty( "Product_Inventory" ) ]
		public int ProductInventory { get; set; }
	}
}