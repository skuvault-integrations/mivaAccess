using MivaAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MivaAccess.Models.Infrastructure
{
	public class MivaBatchRequest : MivaRequest
	{
		public IEnumerable< IMivaBatchIteration > Iterations { get; protected set; }

		public MivaBatchRequest( MivaCredentials credentials, string functionName ) : base( credentials, functionName )
		{
		}
	}

	public interface IMivaBatchIteration
	{
	}
}
