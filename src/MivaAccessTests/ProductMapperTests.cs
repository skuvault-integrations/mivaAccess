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

		[ Test ]
		public void GetImagesUrlsOrderedByPriorityWhereOneOfTheImagesIsMain()
		{
			var product = new Product()
			{
				Sku = "MV-testsku1",
				Images = new ProductImage[]
				{
					new ProductImage()
					{
						Id = 1,
						Code = "none",
						DisplayOrder = 1,
						Url = "/images/1.jpg"
					},
					new ProductImage()
					{
						Id = 2,
						Code = "main",
						DisplayOrder = 2,
						Url = "/images/2.png"
					},
					new ProductImage()
					{
						Id = 3,
						Code = "none",
						DisplayOrder = 3,
						Url = "/images/3.jpg"
					}
				}
			};

			var svProduct = product.ToSVProduct( base.Config.Credentials );

			svProduct.ImagesUrls.First().Should().Contain( product.Images.ElementAt( 1 ).Url );
		}

		[ Test ]
		public void GetImagesUrlsOrderedByPriorityWhereImagesAreNotMain()
		{
			var product = new Product()
			{
				Sku = "MV-testsku1",
				Images = new ProductImage[]
				{
					new ProductImage()
					{
						Id = 1,
						Code = "none",
						DisplayOrder = 2,
						Url = "/images/1.png"
					},
					new ProductImage()
					{
						Id = 2,
						Code = "none",
						DisplayOrder = 3,
						Url = "/images/2.jpg"
					},
					new ProductImage()
					{
						Id = 3,
						Code = "none",
						DisplayOrder = 1,
						Url = "/images/3.jpg"
					}
				}
			};

			var svProduct = product.ToSVProduct( base.Config.Credentials );

			svProduct.ImagesUrls.First().Should().Contain( product.Images.ElementAt( 2 ).Url );
		}
	}
}