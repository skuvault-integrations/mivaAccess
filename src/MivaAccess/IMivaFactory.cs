using MivaAccess.Configuration;
using MivaAccess.Services.Products;

namespace MivaAccess
{
	public interface IMivaFactory
	{
		IMivaProductsService CreateProductsService( MivaConfig config );
	}
}