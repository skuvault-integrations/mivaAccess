using MivaAccess.Configuration;
using MivaAccess.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "descrip" ) ]
		public string Description { get; set; }
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
		[ JsonProperty( "categories" ) ]
		public IEnumerable< ProductCategory > Categories { get; set; }
		[ JsonProperty( "productimagedata" ) ]
		public IEnumerable< ProductImage > Images { get; set; }
	}

	public class ProductCategory
	{
		[ JsonProperty( "id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "active" ) ]
		public bool IsActive { get; set; }
	}

	public class ProductImage
	{
		[ JsonProperty( "image_id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
		[ JsonProperty( "disp_order" ) ]
		public int DisplayOrder { get; set; }
		[ JsonProperty( "image" ) ]
		public string Url { get; set; }
	}

	public class MivaProduct
	{
		public long Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDateUtc { get; set; }
		public DateTime UpdatedDateUtc { get; set; }
		public string Code { get; set; }
		public string Sku { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal SalePrice { get; set; }
		public decimal Cost { get; set; }
		public decimal Weight { get; set; }
		public int Quantity { get; set; }
		public bool IsInventoryTracked { get; set; }
		public IEnumerable< string > Categories { get; set; }
		public IEnumerable< string > ImagesUrls { get; set; }
	}

	public static class ProductExtensions
	{
		public static MivaProduct ToSVProduct( this Product product, MivaCredentials credentials )
		{
			return new MivaProduct()
			{
				Id = product.Id,
				IsActive = product.Active,
				CreatedDateUtc = product.CreatedDateTimestamp.FromEpochTime(),
				UpdatedDateUtc = product.UpdatedDateTimestamp.FromEpochTime(),
				Name = product.Name,
				Description = product.Description,
				Sku = product.Sku,
				Code = product.Code,
				Weight = product.Weight,
				SalePrice = product.Price,
				Cost = product.Cost,
				Quantity = product.Inventory,
				IsInventoryTracked = product.IsInventoryTracked,
				Categories = product.Categories?.Where( c => c.IsActive ).OrderBy( c => c.Name ).Select( c => c.Name ),
				ImagesUrls = GetImagesUrlsByPriority( product.Images, credentials )
			};
		}

		private static IEnumerable< string > GetImagesUrlsByPriority( IEnumerable< ProductImage > images, MivaCredentials credentials )
		{
			if ( images == null || !images.Any() )
				return Array.Empty< string >();

			return images.OrderBy( i => i.Code == "main" ? 0 : 1 )
						.OrderBy( i => i.DisplayOrder )
						.Select( i => GetImageAbsoluteUrl( i.Url, credentials ) );
		}

		private static string GetImageAbsoluteUrl( string relativeUrl, MivaCredentials credentials )
		{
			return $"{ credentials.StoreUrl }/{ credentials.RootDirectory }/{ relativeUrl }";
		}
	}
}
