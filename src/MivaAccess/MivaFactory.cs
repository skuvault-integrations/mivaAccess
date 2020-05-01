using MivaAccess.Configuration;
using MivaAccess.Services.Orders;
using MivaAccess.Services.Products;

namespace MivaAccess
{
	public class MivaFactory : IMivaFactory
	{
		public IMivaOrdersService CreateOrdersService( MivaConfig config )
		{
			return new MivaOrdersService( config );
		}

		public IMivaProductsService CreateProductsService( MivaConfig config )
		{
			return new MivaProductsService( config );
		}
	}
}