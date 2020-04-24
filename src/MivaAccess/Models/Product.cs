using MivaAccess.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MivaAccess.Models
{
	public class Product
	{
		[ JsonProperty( "id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "active" ) ]
		public bool Active { get; set; }
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
		[ JsonProperty( "Sku" ) ]
		public string Sku { get; set; }
		[ JsonProperty( "price" ) ]
		public decimal Price { get; set; }
		[ JsonProperty( "cost" ) ]
		public decimal Cost { get; set; }
		[ JsonProperty( "weight" ) ]
		public decimal Weight { get; set; }
		[ JsonProperty( "dt_created" ) ]
		public long CreatedDateTimestamp { get; set; }
		[ JsonProperty( "dt_updated" ) ]
		public long UpdatedDateTimestamp { get; set; }
		[ JsonProperty( "product_inventory_active" ) ]
		public bool IsInventoryTracked { get; set; }
		[ JsonProperty( "product_inventory" ) ]
		public int Inventory { get; set; }
	}

	public class MivaProduct
	{
		public long Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDateUtc { get; set; }
		public DateTime UpdatedDateUtc { get; set; }
		public string Code { get; set; }
		public string Sku { get; set; }
		public decimal SalePrice { get; set; }
		public decimal Cost { get; set; }
		public decimal Weight { get; set; }
		public int Quantity { get; set; }
		public bool IsInventoryTracked { get; set; }
	}

	public static class ProductExtensions
	{
		public static MivaProduct ToSVProduct( this Product product )
		{
			return new MivaProduct()
			{
				Id = product.Id,
				IsActive = product.Active,
				CreatedDateUtc = product.CreatedDateTimestamp.FromEpochTime(),
				UpdatedDateUtc = product.UpdatedDateTimestamp.FromEpochTime(),
				Sku = product.Sku,
				Code = product.Code,
				Weight = product.Weight,
				SalePrice = product.Price,
				Cost = product.Cost,
				Quantity = product.Inventory,
				IsInventoryTracked = product.IsInventoryTracked
			};
		}
	}
}
