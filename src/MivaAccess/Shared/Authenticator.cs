using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MivaAccess.Shared
{
	public class Authenticator
	{
		public MivaCredentials Credentials { get; private set; }

		public Authenticator( MivaCredentials credentials )
		{
			Condition.Requires( credentials, "credentials" ).IsNotNull();

			this.Credentials = credentials;
		}

		public string GetAuthorizationHeaderValue( string requestBody )
		{
			if ( string.IsNullOrEmpty( Credentials.PrivateKey ) )
			{
				return $"MIVA { Credentials.AccessToken }";
			}

			return $"MIVA-HMAC-SHA256 { Credentials.AccessToken }:{ this.GenerateSignature( requestBody ) }";
		}

		private string GenerateSignature( string requestBody )
		{
			var decodedPrivateKey = Convert.FromBase64String( RestoreBase64Str( Credentials.PrivateKey ) );
			var hmac = new HMACSHA256( decodedPrivateKey );

			byte[] data = Encoding.ASCII.GetBytes( requestBody );
			using( var stream = new MemoryStream( data ) )
			{
				return Convert.ToBase64String( hmac.ComputeHash( stream ) );
			}
		}

		private string RestoreBase64Str( string str )
		{
			if ( str.Length % 4 != 0 )
			{
				return str + new String( '=', ( 4 - ( str.Length % 4 ) ) );
			}

			return str;
		}
	}
}