using System;

namespace MivaAccess.Exceptions
{
	public class MivaNetworkException : MivaException
	{
		public MivaNetworkException( string message, Exception innerException ) : base( message, innerException) { }
		public MivaNetworkException( string message ) : base( message ) { }
	}

	public class MivaUnauthorizedException : MivaException
	{
		public MivaUnauthorizedException( string message ) : base( message) { }
	}
}