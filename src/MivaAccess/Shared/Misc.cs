using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public static List< Dictionary< K, V > > SplitToChunks< K, V >( this Dictionary< K, V > source, int chunkSize )
		{
			var i = 0;
			var chunks = new List< Dictionary< K, V > >();
			
			while( i < source.Count() )
			{
				var temp = source.Skip( i ).Take( chunkSize ).ToDictionary( x => x.Key, x => x.Value );
				chunks.Add( temp );
				i += chunkSize;
			}
			return chunks;
		}
	}
}