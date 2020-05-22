using System;

namespace MivaAccess.Exceptions
{
	public class MivaException : Exception
	{
		public string ErrorCode { get; set; }

		public MivaException( string message, Exception innerException, string code ) : base( message, innerException ) 
		{
			this.ErrorCode = code;
		}

		public MivaException( string message, string code ) : this( message, null, code ) { }
		public MivaException( string message ) : this ( message, null, null ) { }
	}
}