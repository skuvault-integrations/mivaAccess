using MivaAccess.Configuration;
using MivaAccess.Services.Products;

namespace MivaAccess
{
	public class MivaFactory : IMivaFactory
	{
		public IMivaProductsService CreateProductsService( MivaConfig config )
		{
			return new MivaProductsService( config );
		}
	}
}