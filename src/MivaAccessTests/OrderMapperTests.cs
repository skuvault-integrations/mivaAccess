using FluentAssertions;
using MivaAccess.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class OrderMapperTests
	{
		[ Test ]
		public void ToSVOrder()
		{
			var order = new Order()
			{
				Id = 12345,
				Status = 100,
				PaymentStatus = 200,
				OrderDate = 1588314459,
				ShipFName = "Peter Jason",
				ShipLName = "Quill",
				ShipEmail = "integrations@skuvault.com",
				ShipComp = "SkuVault",
				ShipPhone = "89215929292",
				ShipAddr1 = "Pulkovskoye sh.",
				ShipCity = "Saint Petersburg",
				ShipState = "NW",
				ShipCountry = "RUSSIA",
				ShipMethod = "DHL",
				ShipZip = "188517",
				Notes = new OrderNote[] { new OrderNote() { NoteText = "Fragile cargo", UserId = 1 } },
				Items = new OrderItem[] { 
					new OrderItem()
					{
						Code = "MV-testsku1",
						Name = "MV-testsku1",
						Sku = "MV-testsku1",
						Quantity = 10,
						Price = 5,
						Discounts = new OrderItemDiscount[]
						{
							new OrderItemDiscount()
							{
								Amount = 1
							}
						}
					}
				},
				Charges = new OrderCharge[]
				{
					new OrderCharge()
					{
						Type = "TAX",
						Amount = 4
					},
					new OrderCharge()
					{
						Type = "DISCOUNT",
						Amount = 10
					}
				},
				Coupons = new OrderCoupon[]
				{
					new OrderCoupon()
					{
						Code = "XYZ12345",
						Total = 2
					}
				},
				TotalShip = 7,
				Total = 48
			};

			var svOrder = order.ToSVOrder();

			svOrder.Id.Should().Be( order.Id.ToString() );
			svOrder.Status.Should().Be( MivaOrderStatusEnum.Processing );
			svOrder.PaymentStatus.Should().Be( MivaOrderPaymentStatusEnum.Captured );
			svOrder.Note.Should().Be( order.Notes.First().NoteText );
			svOrder.OrderDateUtc.Should().Be( new DateTime( 2020, 05, 01, 06, 27, 39 ) );
			
			svOrder.ShippingInfo.ShippingCharge.Should().Be( order.TotalShip );
			svOrder.ShippingInfo.Carrier.Should().Be( order.ShipMethod );
			
			svOrder.ShippingInfo.Address.Line1.Should().Be( order.ShipAddr1 );
			svOrder.ShippingInfo.Address.Line2.Should().Be( order.ShipAddr2 );
			svOrder.ShippingInfo.Address.City.Should().Be( order.ShipCity );
			svOrder.ShippingInfo.Address.State.Should().Be( order.ShipState );
			svOrder.ShippingInfo.Address.Country.Should().Be( order.ShipCountry );
			svOrder.ShippingInfo.Address.PostalCode.Should().Be( order.ShipZip );

			svOrder.ShippingInfo.ContactInfo.CompanyName.Should().Be( order.ShipComp );
			svOrder.ShippingInfo.ContactInfo.FirstName.Should().Be( order.ShipFName );
			svOrder.ShippingInfo.ContactInfo.LastName.Should().Be( order.ShipLName );
			svOrder.ShippingInfo.ContactInfo.EmailAddress.Should().Be( order.ShipEmail );
			svOrder.ShippingInfo.ContactInfo.PhoneNumber.Should().Be( order.ShipPhone );

			svOrder.Items.Count().Should().Be( order.Items.Count() );
			svOrder.Items.First().Sku.Should().Be( order.Items.First().Sku );
			svOrder.Items.First().Quantity.Should().Be( order.Items.First().Quantity );
			svOrder.Items.First().UnitPrice.Should().Be( order.Items.First().Price );
			svOrder.Items.First().Discount.Should().Be( order.Items.First().Discounts.First().Amount );

			svOrder.Tax.Should().Be( order.Charges.First( c => c.Type == "TAX" ).Amount );
			svOrder.Discount.Should().Be( order.Charges.First( c => c.Type == "DISCOUNT" ).Amount );

			svOrder.Promotions.First().Code.Should().Be( order.Coupons.First().Code );
			svOrder.Promotions.First().Amount.Should().Be( order.Coupons.First().Total );

			svOrder.Total.Should().Be( order.Total );
		}

		[ Test ]
		public void ToSVOrderWithUnknownStatus()
		{
			var order = new Order()
			{
				Id = 12345,
				OrderDate = 1588314459,
				Status = 700,
				PaymentStatus = 300
			};

			var svOrder = order.ToSVOrder();

			svOrder.Status.Should().Be( order.Status );
			svOrder.PaymentStatus.Should().Be( order.PaymentStatus );
		}

		[ Test ]
		public void ToSVOrderWithItemShipmentData()
		{
			var order = new Order()
			{
				Id = 12345,
				OrderDate = 1588314459,
				Items = new OrderItem[]
				{
					new OrderItem()
					{
						Sku = "MV-testsku1",
						Quantity = 5,
						Price = 1.5M,
						Shipment = new OrderItemShipment()
						{
							Id = 1,
							Cost = 0.5M,
							ShipDate = 1588763364,
							Status = 200,
							TrackLink = "https://ups.com/track?num=12345",
							TrackNum = "12345",
							TrackType = "UPS", 
							Weight = 2.1M
						}
					}
				}
			};

			var svOrder = order.ToSVOrder();

			svOrder.Items.Count().Should().Be( 1 );
			var shipmentInfo = svOrder.Items.First().ShipmentInfo;
			shipmentInfo.Id.Should().Be( order.Items.First().Shipment.Id );
			shipmentInfo.ShipDate.Should().Be( new DateTime( 2020, 05, 06, 11, 09, 24 ) );
			shipmentInfo.Status.Should().Be( MivaOrderItemShipmentStatus.Shipped );
			shipmentInfo.TrackingNumber.Should().Be( order.Items.First().Shipment.TrackNum );
			shipmentInfo.TrackingUrl.Should().Be( order.Items.First().Shipment.TrackLink );
			shipmentInfo.Carrier.Should().Be( order.Items.First().Shipment.TrackType );
			shipmentInfo.Weight.Should().Be( order.Items.First().Shipment.Weight );
			shipmentInfo.Cost.Should().Be( order.Items.First().Shipment.Cost );
		}
	}
}