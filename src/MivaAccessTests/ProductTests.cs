using FluentAssertions;
using MivaAccess.Services.Products;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class ProductTests : BaseTest
	{
		private IMivaProductsService _productsService;

		[ SetUp ]
		public void Init()
		{
			this._productsService = new MivaProductsService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedProducts()
		{
			var products = await this._productsService.GetProductsCreatedOrUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -1 ), CancellationToken.None );
			products.Should().NotBeNullOrEmpty();
		}
	}
}