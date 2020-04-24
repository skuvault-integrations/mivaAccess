using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net;

namespace MivaAccess.Shared
{
	public static class Misc
	{
		public static string ToJson( this object source )
		{
			try
			{
				if (source == null)
					return "{}";
				else
				{
					var serialized = JsonConvert.SerializeObject( source, new IsoDateTimeConverter() );
					return serialized;
				}
			}
			catch( Exception )
			{
				return "{}";
			}
		}

		public static DateTime FromEpochTime( this long epochTime )
		{
			return new DateTime( 1970, 1, 1 ).AddSeconds( epochTime );
		}

		public static int FromUtcTimeToEpoch( this DateTime date )
		{
			return ( int )( date - new DateTime( 1970, 1, 1 ) ).TotalSeconds;
		}

		public static void InitSecurityProtocol()
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
		}
	}
}