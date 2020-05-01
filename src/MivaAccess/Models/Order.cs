using MivaAccess.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MivaAccess.Models
{
	public class Order
	{
		[ JsonProperty( "id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "status" ) ]
		public int Status { get; set; }
		[ JsonProperty( "pay_status" ) ]
		public int PaymentStatus { get; set; }
		[ JsonProperty( "orderdate" ) ]
		public long OrderDate { get; set; }
		[ JsonProperty( "ship_fname" ) ]
		public string ShipFName { get; set; }
		[ JsonProperty( "ship_lname" ) ]
		public string ShipLName { get; set; }
		[ JsonProperty( "ship_email" ) ]
		public string ShipEmail { get; set; }
		[ JsonProperty( "ship_comp" ) ]
		public string ShipComp { get; set; }
		[ JsonProperty( "ship_phone" ) ]
		public string ShipPhone { get; set; }
		[ JsonProperty( "ship_addr1" ) ]
		public string ShipAddr1 { get; set; }
		[ JsonProperty( "ship_addr2" ) ]
		public string ShipAddr2 { get; set; }
		[ JsonProperty( "ship_city" ) ]
		public string ShipCity { get; set; }
		[ JsonProperty( "ship_state" ) ]
		public string ShipState { get; set; }
		[ JsonProperty( "ship_zip" ) ]
		public string ShipZip { get; set; }
		[ JsonProperty( "ship_cntry" ) ]
		public string ShipCountry { get; set; }
		[ JsonProperty( "ship_method" ) ]
		public string ShipMethod { get; set; }
		[ JsonProperty( "total_ship" ) ]
		public decimal TotalShip { get; set; }
		[ JsonProperty( "total" ) ]
		public decimal Total { get; set; }
		[ JsonProperty( "notes" ) ]
		public IEnumerable< OrderNote > Notes { get; set; }
		[ JsonProperty( "items" ) ]
		public IEnumerable< OrderItem > Items { get; set; }
		[ JsonProperty( "charges" ) ]
		public IEnumerable< OrderCharge > Charges { get; set; }
		[ JsonProperty( "coupons" ) ]
		public IEnumerable< OrderCoupon > Coupons { get; set; }
	}

	public class OrderNote
	{
		[ JsonProperty( "id" ) ]
		public long Id { get; set; }
		[ JsonProperty( "user_id" ) ]
		public long UserId { get; set; }
		[ JsonProperty( "notetext" ) ]
		public string NoteText { get; set; }
	}

	public class OrderCharge
	{
		[ JsonProperty( "order_id" ) ]
		public long OrderId { get; set; }
		[ JsonProperty( "charge_id" ) ]
		public long ChargeId { get; set; }
		[ JsonProperty( "module_id" ) ]
		public long ModuleId { get; set; }
		[ JsonProperty( "type" ) ]
		public string Type { get; set; }
		[ JsonProperty( "descrip" ) ]
		public string Description { get; set; }
		[ JsonProperty( "amount" ) ]
		public decimal Amount { get; set; }
	}

	public class OrderCoupon
	{
		[ JsonProperty( "order_id" ) ]
		public long OrderId { get; set; }
		[ JsonProperty( "coupon_id" ) ]
		public long CouponId { get; set; }
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
		[ JsonProperty( "descrip" ) ]
		public string Description { get; set; }
		[ JsonProperty( "total" ) ]
		public int Total { get; set; }
	}

	public class OrderItem
	{
		[ JsonProperty( "order_id" ) ]
		public long OrderId { get; set; }
		[ JsonProperty( "line_id" ) ]
		public long LineId { get; set; }
		[ JsonProperty( "status" ) ]
		public int Status { get; set; }
		[ JsonProperty( "code" ) ]
		public string Code { get; set; }
		[ JsonProperty( "name" ) ]
		public string Name { get; set; }
		[ JsonProperty( "sku" ) ]
		public string Sku { get; set; }
		[ JsonProperty( "quantity" ) ]
		public int Quantity { get; set; }
		[ JsonProperty( "price" ) ]
		public decimal Price { get; set; }
		[ JsonProperty( "discounts" ) ]
		public IEnumerable< OrderItemDiscount > Discounts { get; set; }
	}

	public class OrderItemDiscount
	{
		[ JsonProperty( "order_id" ) ]
		public long OrderId { get; set; }
		[ JsonProperty( "line_id" ) ]
		public long LineId { get; set; }
		[ JsonProperty( "display" ) ]
		public bool Display { get; set; }
		[ JsonProperty( "description" ) ]
		public string Description { get; set; }
		[ JsonProperty( "discount" ) ]
		public decimal Amount { get; set; }
	}

	public class MivaOrder
	{
		public string Id { get; set; }
		public DateTime OrderDateUtc { get; set; }
		public MivaOrderStatusEnum Status { get; set; }
		public MivaOrderPaymentStatusEnum PaymentStatus { get; set; }
		public MivaShippingInfo ShippingInfo { get; set; }
		public IEnumerable< MivaOrderItem > Items { get; set; }
		public string Note { get; set; }
		public decimal Total { get; set; }
		public decimal Discount { get; set; }
		public decimal Tax { get; set; }
		public decimal ShippingCharge { get; set; }
		public IEnumerable< MivaOrderPromotion > Promotions { get; set; }
	}

	public class MivaShippingInfo
	{
		public MivaShippingContactInfo ContactInfo { get; set; }
		public MivaShippingAddress Address { get; set; }
		public string Carrier { get; set; }
		public decimal ShippingCharge { get; set; }
	}

	public class MivaShippingAddress
	{
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
	}

	public class MivaShippingContactInfo
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CompanyName { get; set; }
		public string PhoneNumber { get; set; }
		public string EmailAddress { get; set; }
	}

	public class MivaOrderItem
	{
		public string Sku { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
	}

	public class MivaOrderPromotion
	{
		public string Code { get; set; }
		public decimal Amount { get; set; }
	}

	public static class OrderExtensions
	{
		public static MivaOrder ToSVOrder(this Order order)
		{
			var svOrder = new MivaOrder()
			{
				Id = order.Id.ToString(),
				OrderDateUtc = order.OrderDate.FromEpochTime(),
				Status = (MivaOrderStatusEnum)order.Status,
				PaymentStatus = (MivaOrderPaymentStatusEnum)order.PaymentStatus,
				ShippingInfo = new MivaShippingInfo()
				{
					ContactInfo = new MivaShippingContactInfo()
					{
						FirstName = order.ShipFName,
						LastName = order.ShipLName,
						EmailAddress = order.ShipEmail,
						CompanyName = order.ShipComp,
						PhoneNumber = order.ShipPhone
					},
					Address = new MivaShippingAddress()
					{
						Line1 = order.ShipAddr1,
						Line2 = order.ShipAddr2,
						City = order.ShipCity,
						Country = order.ShipCountry,
						PostalCode = order.ShipZip,
						State = order.ShipState
					},
					Carrier = order.ShipMethod,
					ShippingCharge = order.TotalShip
				},
				Items = GetOrderItems( order ),
				Total = order.Total
			};

			var notes = order.Notes?.Where( n => !string.IsNullOrEmpty( n.NoteText ) && n.UserId != 0 ).Select( n => n.NoteText );
			if ( notes != null )
			{
				svOrder.Note = string.Join( ";", notes );
			}

			if ( order.Charges != null && order.Charges.Any() )
			{
				FillOrderCharges( order, svOrder );
			}

			if ( order.Coupons != null && order.Coupons.Any() )
			{
				svOrder.Promotions = order.Coupons.Select( c => new MivaOrderPromotion() { Code = c.Code, Amount = c.Total } );
			}

			return svOrder;
		}

		private static IEnumerable< MivaOrderItem > GetOrderItems( Order order )
		{
			if ( order.Items == null || !order.Items.Any() )
				return Array.Empty< MivaOrderItem >();

			var items = new List< MivaOrderItem >();

			foreach( var item in order.Items )
			{
				decimal itemDiscount = 0;

				if ( item.Discounts != null && item.Discounts.Any() )
				{
					itemDiscount = item.Discounts.Sum( d => d.Amount );
				}

				items.Add( new MivaOrderItem()
				{
					Sku = item.Sku,
					Quantity = item.Quantity,
					UnitPrice = item.Price,
					Discount = itemDiscount
				} );
			}
			
			return items;
		}

		private static void FillOrderCharges( Order order, MivaOrder svOrder )
		{
			foreach( var orderCharge in order.Charges )
			{
				switch ( orderCharge.Type )
				{
					case "TAX":
						{
							svOrder.Tax = orderCharge.Amount;
							break;
						}
					case "DISCOUNT":
						{
							svOrder.Discount = orderCharge.Amount;
							break;
						}
				}
			}
		}
	}

	public enum MivaOrderStatusEnum
	{
		Pending = 0,
		Processing = 100,
		Shipped = 200,
		PartiallyShipped = 201,
		Cancelled = 300,
		BackOrdered = 400,
		RMAIssued = 500,
		Returned = 600
	}

	public enum MivaOrderPaymentStatusEnum
	{
		Pending = 0,
		Authorized = 100,
		Captured = 200,
		PartiallyCaptured = 201
	}
}
