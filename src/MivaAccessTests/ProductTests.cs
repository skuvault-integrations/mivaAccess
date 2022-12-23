using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MivaAccess.Exceptions;
using MivaAccess.Services.Products;
using NUnit.Framework;

namespace MivaAccessTests
{
	[TestFixture]
	public class ProductTests : BaseTest
	{
		private IMivaProductsService _productsService;
		private const string testSku = "MV-testsku1";
		private const string testSku2 = "MV-testsku2";
		private const string notUniqueSku = "MV-testsku3";
		private const string disabledSku = "MV-testsku5";

		[SetUp]
		public void Init()
		{
			this._productsService = new MivaProductsService(base.Config);
		}

		[Explicit]
		[Test]
		public async Task GetModifiedProducts()
		{
			var products = await this._productsService.GetProductsUpdatedAfterAsync(DateTime.UtcNow.AddMonths(-1), CancellationToken.None);
			products.Should().NotBeNullOrEmpty();
		}

		[Explicit]
		[Test]
		public async Task GetModifiedProductsUsingSmallPageSize()
		{
			base.Config.ProductsPageSize = 1;
			var products = await this._productsService.GetProductsUpdatedAfterAsync(DateTime.UtcNow.AddMonths(-1), CancellationToken.None);

			products.Should().NotBeNullOrEmpty();
		}

		[Explicit]
		[Test]
		public async Task FindProductBySku()
		{
			var products = await this._productsService.FindProductBySku(testSku, CancellationToken.None);
			products.Should().NotBeNullOrEmpty();
		}

		[Explicit]
		[Test]
		public async Task FindProductBySkuThatDoesntExist()
		{
			var notExistingSku = Guid.NewGuid().ToString();

			var products = await this._productsService.FindProductBySku(notExistingSku, CancellationToken.None);
			products.Should().BeEmpty();
		}

		[Explicit]
		[Test]
		public async Task UpdateProductQuantityBySku()
		{
			int newQuantity = new Random().Next(1, 100);
			await this._productsService.UpdateProductQuantityBySkuAsync(testSku, newQuantity, CancellationToken.None);

			var inventory = await this._productsService.FindProductBySku(testSku, CancellationToken.None);
			inventory.FirstOrDefault().Quantity.Should().Be(newQuantity);
		}

		[Explicit]
		[Test]
		public async Task UpdateProductQuantityBySkuToZero()
		{
			await this._productsService.UpdateProductQuantityBySkuAsync(testSku, 0, CancellationToken.None);

			var inventory = await this._productsService.FindProductBySku(testSku, CancellationToken.None);
			inventory.FirstOrDefault().Quantity.Should().Be(0);
		}

		[Explicit]
		[Test]
		public async Task UpdateProductQuantityWhenSkuIsNotUnique()
		{
			var products = await this._productsService.FindProductBySku(notUniqueSku, CancellationToken.None);
			var newQuantity = new Random().Next(1, 100);
			await this._productsService.UpdateProductQuantityBySkuAsync(notUniqueSku, newQuantity, CancellationToken.None);

			var updatedProducts = await this._productsService.FindProductBySku(notUniqueSku, CancellationToken.None);
			Assert.IsFalse(updatedProducts.Where((p, i) => p.Quantity != products.FirstOrDefault(p2 => p2.Code == p.Code).Quantity).Any());
		}

		[Explicit]
		[Test]
		public async Task UpdateDisabledProductQuantity()
		{
			var currentSkuQuantity = (await this._productsService.FindProductBySku(disabledSku, CancellationToken.None)).FirstOrDefault().Quantity;
			var newSkuQuantity = new Random().Next(1, 100);
			await this._productsService.UpdateProductQuantityBySkuAsync(disabledSku, newSkuQuantity, CancellationToken.None);

			var inventory = await this._productsService.FindProductBySku(disabledSku, CancellationToken.None);
			inventory.FirstOrDefault().Quantity.Should().Be(currentSkuQuantity);
		}

		[Explicit]
		[Test]
		public void UpdateProductQuantityThatDoesntExist()
		{
			var notExistingSku = Guid.NewGuid().ToString();
			var newSkuQuantity = new Random().Next(1, 100);

			Assert.DoesNotThrow(() =>
			{
				this._productsService.UpdateProductQuantityBySkuAsync(notExistingSku, newSkuQuantity, CancellationToken.None).Wait();
			});
		}

		[Explicit]
		[Test]
		public async Task UpdateProductsQuantitiesAsync()
		{
			var randomizer = new Random();
			var inventory = new Dictionary<string, int>()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) }
			};

			await this._productsService.UpdateProductsQuantitiesBySkuAsync(inventory, CancellationToken.None);

			foreach (var skuInventory in inventory)
			{
				var product = await this._productsService.FindProductBySku(skuInventory.Key, CancellationToken.None);
				product.FirstOrDefault().Quantity.Should().Be(skuInventory.Value);
			}
		}

		[Explicit]
		[Test]
		public async Task UpdateProductQuantitiesWithMinBatchSize()
		{
			var randomizer = new Random();
			var inventory = new Dictionary<string, int>()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) }
			};
			base.Config.InventoryUpdateBatchSize = 1;

			await this._productsService.UpdateProductsQuantitiesBySkuAsync(inventory, CancellationToken.None);

			foreach (var skuInventory in inventory)
			{
				var product = await this._productsService.FindProductBySku(skuInventory.Key, CancellationToken.None);
				product.FirstOrDefault().Quantity.Should().Be(skuInventory.Value);
			}
		}

		[Explicit]
		[Test]
		public async Task UpdateProductsQuantitiesWhenOneProductIsNotPossibleToUpdate()
		{
			var randomizer = new Random();
			var inventory = new Dictionary<string, int>()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) },
				{ notUniqueSku, randomizer.Next( 1, 100 ) },
			};

			await this._productsService.UpdateProductsQuantitiesBySkuAsync(inventory, CancellationToken.None);

			foreach (var skuInventory in inventory)
			{
				var product = (await this._productsService.FindProductBySku(skuInventory.Key, CancellationToken.None)).FirstOrDefault();

				if (product.Sku != notUniqueSku)
				{
					product.Quantity.Should().Be(skuInventory.Value);
				}
			}
		}

		[Explicit]
		[Test(Description = "This test reproduces an issue with creating request signature. " +
			"Should be fixed in ticket https://agileharbor.atlassian.net/browse/PBL-8100")]
		public void UpdateProductsQuantitiesBySku_ThrowsException_WhenSkuWithSpecialChars()
		{
			var randomizer = new Random();
			var inventory = new Dictionary<string, int>
			{
				{ "EtsyVar7б¶“", randomizer.Next(1, 100) }
			};

			var exception = Assert.ThrowsAsync<MivaException>(() =>
				_productsService.UpdateProductsQuantitiesBySkuAsync(inventory, CancellationToken.None));

			Assert.That(exception, Is.Not.Null);
			Assert.That(exception.Message.Contains("Invalid request signature"), Is.True);
		}
	}
}