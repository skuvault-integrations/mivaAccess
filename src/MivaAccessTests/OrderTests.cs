using FluentAssertions;
using MivaAccess.Services.Orders;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class OrderTests : BaseTest
	{
		private IMivaOrdersService _ordersService;
		private string _ordersQueueName = "new_and_updated";

		[ SetUp ]
		public void Init()
		{
			this._ordersService = new MivaOrdersService( base.Config );
		}

		[ Test ]
		public async Task GetModifiedOrdersFromQueue()
		{
			var orders = await this._ordersService.GetModifiedOrdersFromQueueAsync( _ordersQueueName, CancellationToken.None );
			orders.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void AcknowledgeOrder()
		{
			var orderId = 200100;
			Assert.DoesNotThrow( () =>
			{
				this._ordersService.AcknowledgeOrders( new long[] { orderId }, CancellationToken.None ).Wait();
			} ); 
		}
	}
}