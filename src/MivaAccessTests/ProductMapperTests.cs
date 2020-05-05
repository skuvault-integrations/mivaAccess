using FluentAssertions;
using MivaAccess.Models;
using NUnit.Framework;
using System.Linq;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class ProductMapperTests : BaseTest
	{
		[ Test ]
		public void ToSVProduct()
		{
			var product = new Product()
			{
				Sku = "MV-testsku1",
				Code = "MV-testsku1",
				Name = "TestSku",
				Price = 5.7M,
				Cost = 4.3M,
				Weight = 5.0M,
				Description = "Some sku description here",
				Images = new ProductImage[] { 
					new ProductImage() { 
						Code = "main", 
						DisplayOrder = 1, 
						Url = "graphics/1.jpg" 
					} 
				},
				Categories = new ProductCategory[]
				{
					new ProductCategory()
					{
						IsActive = true,
						Code = "CAT_CODE",
						Name = "Apparel"
					}
				}
			};

			var svProduct = product.ToSVProduct( base.Config.Credentials );

			svProduct.Sku.Should().Be( product.Sku );
			svProduct.Code.Should().Be( product.Code );
			svProduct.Name.Should().Be( product.Name );
			svProduct.SalePrice.Should().Be( product.Price );
			svProduct.Cost.Should().Be( product.Cost );
			svProduct.Weight.Should().Be( product.Weight );
			svProduct.Description.Should().Be( product.Description );
			svProduct.ImagesUrls.First().Should().Be( base.Config.Credentials.StoreUrl 
											+ "/" + base.Config.Credentials.RootDirectory 
											+ "/" + product.Images.First().Url );
			svProduct.Categories.First().Should().Be( product.Categories.First().Name );
		}
	}
}