using MivaAccess.Configuration;
using MivaAccess.Models.Infrastructure;

namespace MivaAccess.Models.Requests
{
	public class FindProductBySkuRequest : MivaQuery
	{
		public FindProductBySkuRequest( MivaCredentials credentials, string sku ) : base( credentials, "ProductList_Load_Query" )
		{
			this.Filters = new MivaRequestFilter[]
			{
				new MivaRequestSearchFilter( 
					new MivaRequestFilterSearchValue[] { 
						new MivaRequestFilterSearchValue()
						{
							Field = "sku",
							Operator = "EQ",
							Value = sku
						}
					} )
			};
		}
	}
}