using System;

namespace MivaAccess.Exceptions
{
	public class MivaException : Exception
	{
		public string ErrorCode { get; set; }

		public MivaException( string message, Exception innerException ) : base( message, innerException ) { }
		public MivaException( string message ) : this ( message, null) { }
	}
}