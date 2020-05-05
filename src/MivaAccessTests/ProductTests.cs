using FluentAssertions;
using MivaAccess.Services.Products;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class ProductTests : BaseTest
	{
		private IMivaProductsService _productsService;
		private const string testSku = "MV-testsku1";
		private const string testSku2 = "MV-testsku2";
		private const string notUniqueSku = "MV-testsku3";
		private const string disabledSku = "MV-testsku5";

		[ SetUp ]
		public void Init()
		{
			this._productsService = new MivaProductsService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedProducts()
		{
			var products = await this._productsService.GetProductsUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -1 ), CancellationToken.None );
			products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task GetModifiedProductsUsingSmallPageSize()
		{
			base.Config.ProductsPageSize = 1;
			var products = await this._productsService.GetProductsUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -1 ), CancellationToken.None );
			
			products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task FindProductBySku()
		{
			var products = await this._productsService.FindProductBySku( testSku, CancellationToken.None );
			products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public async Task FindProductBySkuThatDoesntExist()
		{
			var notExistingSku = Guid.NewGuid().ToString();

			var products = await this._productsService.FindProductBySku( notExistingSku, CancellationToken.None );
			products.Should().BeEmpty();
		}

		[ Test ]
		public async Task UpdateProductQuantityBySku()
		{
			int newQuantity = new Random().Next( 1, 100 );
			await this._productsService.UpdateProductQuantityBySkuAsync( testSku, newQuantity, CancellationToken.None );

			var inventory = await this._productsService.FindProductBySku( testSku, CancellationToken.None );
			inventory.FirstOrDefault().Quantity.Should().Be( newQuantity );
		}

		[ Test ]
		public async Task UpdateProductQuantityBySkuToZero()
		{
			await this._productsService.UpdateProductQuantityBySkuAsync( testSku, 0, CancellationToken.None );
			
			var inventory = await this._productsService.FindProductBySku( testSku, CancellationToken.None );
			inventory.FirstOrDefault().Quantity.Should().Be( 0 );
		}

		[ Test ]
		public async Task UpdateProductQuantityWhenSkuIsNotUnique()
		{
			var products = await this._productsService.FindProductBySku( notUniqueSku, CancellationToken.None );
			var newQuantity = new Random().Next( 1, 100 );
			await this._productsService.UpdateProductQuantityBySkuAsync( notUniqueSku, newQuantity, CancellationToken.None );
			
			var updatedProducts = await this._productsService.FindProductBySku( notUniqueSku, CancellationToken.None );
			Assert.IsFalse( updatedProducts.Where( ( p, i ) => p.Quantity != products.FirstOrDefault( p2 => p2.Code == p.Code ).Quantity ).Any() );
		}

		[ Test ]
		public async Task UpdateDisabledProductQuantity()
		{
			var currentSkuQuantity = ( await this._productsService.FindProductBySku( disabledSku, CancellationToken.None ) ).FirstOrDefault().Quantity;
			var newSkuQuantity = new Random().Next( 1, 100 );
			await this._productsService.UpdateProductQuantityBySkuAsync( disabledSku, newSkuQuantity, CancellationToken.None );
			
			var inventory = await this._productsService.FindProductBySku( disabledSku, CancellationToken.None );
			inventory.FirstOrDefault().Quantity.Should().Be( currentSkuQuantity );
		}

		[ Test ]
		public void UpdateProductQuantityThatDoesntExist()
		{
			var notExistingSku = Guid.NewGuid().ToString();
			var newSkuQuantity = new Random().Next( 1, 100 );

			Assert.DoesNotThrow( () =>
			{
				this._productsService.UpdateProductQuantityBySkuAsync( notExistingSku, newSkuQuantity, CancellationToken.None ).Wait();
			} );
		}

		[ Test ]
		public async Task UpdateProductsQuantitiesAsync()
		{
			var randomizer = new Random();
			var inventory = new Dictionary< string, int >()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) }
			};

			await this._productsService.UpdateProductsQuantitiesBySkuAsync( inventory, CancellationToken.None );

			foreach( var skuInventory in inventory )
			{
				var product = await this._productsService.FindProductBySku( skuInventory.Key, CancellationToken.None );
				product.FirstOrDefault().Quantity.Should().Be( skuInventory.Value );
			}
		}

		[ Test ]
		public async Task UpdateProductQuantitiesWithMinBatchSize()
		{
			var randomizer = new Random();
			var inventory = new Dictionary< string, int >()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) }
			};
			base.Config.InventoryUpdateBatchSize = 1;

			await this._productsService.UpdateProductsQuantitiesBySkuAsync( inventory, CancellationToken.None );

			foreach( var skuInventory in inventory )
			{
				var product = await this._productsService.FindProductBySku( skuInventory.Key, CancellationToken.None );
				product.FirstOrDefault().Quantity.Should().Be( skuInventory.Value );
			}
		}

		[ Test ]
		public async Task UpdateProductsQuantitiesWhenOneProductIsNotPossibleToUpdate()
		{
			var randomizer = new Random();
			var inventory = new Dictionary< string, int >()
			{
				{ testSku, randomizer.Next( 1, 100 ) },
				{ testSku2, randomizer.Next( 1, 100 ) },
				{ notUniqueSku, randomizer.Next( 1, 100 ) },
			};

			await this._productsService.UpdateProductsQuantitiesBySkuAsync( inventory, CancellationToken.None );

			foreach( var skuInventory in inventory )
			{
				var product = ( await this._productsService.FindProductBySku( skuInventory.Key, CancellationToken.None ) ).FirstOrDefault();

				if ( product.Sku != notUniqueSku )
				{
					product.Quantity.Should().Be( skuInventory.Value );
				}
			}
		}
	}
}