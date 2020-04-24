using MivaAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccess.Services.Products
{
	public interface IMivaProductsService
	{
		Task< IEnumerable< MivaProduct > > GetProductsCreatedOrUpdatedAfterAsync( DateTime startDateUtc, CancellationToken token );
	}
}