using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MivaAccess.Services.Orders;
using MivaAccess.Shared;
using NUnit.Framework;

namespace MivaAccessTests
{
	[TestFixture]
	public class OrderTests : BaseTest
	{
		private IMivaOrdersService _ordersService;
		private const string OrdersQueueName = "new_and_updated";

		[SetUp]
		public void Init()
		{
			this._ordersService = new MivaOrdersService(base.Config);
		}

		[Explicit]
		[Test]
		public async Task GetModifiedOrdersFromQueueAsync_ReturnsValidResult()
		{
			var orders = await this._ordersService.GetModifiedOrdersFromQueueAsync(OrdersQueueName, Mark.Blank(), CancellationToken.None);

			orders.Should().NotBeNullOrEmpty();
		}

		[Explicit]
		[Test]
		public void AcknowledgeOrdersAsync_DoesNotThrowError_WhenIsOnlyOneOrder()
		{
			var orderId = 200100;

			Assert.DoesNotThrow(() =>
			{
				this._ordersService.AcknowledgeOrdersAsync(new long[] { orderId }, Mark.Blank(), CancellationToken.None).Wait();
			});
		}

		[Explicit]
		[Test]
		public void AcknowledgeOrdersAsync_DoesNotThrowError_WhenOrdersMoreThanBatchLimit()
		{
			var ordersIds = GenerateRandomOrderIds(MivaOrdersService.AcknowledgeOrdersBatchLimit + 5);

			Assert.DoesNotThrow(() =>
			{
				this._ordersService.AcknowledgeOrdersAsync(ordersIds, Mark.Blank(), CancellationToken.None).Wait();
			});
		}

		private static IEnumerable<long> GenerateRandomOrderIds(int number)
		{
			Random rnd = new Random();
			for (int i = 0; i < number; i ++)
			{
				yield return (long)rnd.Next(-999999, -100000);
			}
		}
	}
}