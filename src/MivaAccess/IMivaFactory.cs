using MivaAccess.Configuration;
using MivaAccess.Services.Orders;
using MivaAccess.Services.Products;

namespace MivaAccess
{
	public interface IMivaFactory
	{
		IMivaProductsService CreateProductsService( MivaConfig config );
		IMivaOrdersService CreateOrdersService( MivaConfig config );
	}
}