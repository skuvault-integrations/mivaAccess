using Newtonsoft.Json;

namespace MivaAccess.Models.Infrastructure
{
	public class MivaResponse < T >
	{
		[ JsonProperty( "success" ) ]
		public int Success { get; set; }
		[ JsonProperty( "data" ) ]
		public MivaResponseData< T > Data { get; set; }
	}

	public class MivaResponseData< T >
	{
		[ JsonProperty( "total_count" ) ]
		public int TotalCount { get; set; }
		[ JsonProperty( "start_offset" ) ]
		public int StartOffset { get; set; }
		[ JsonProperty( "data" ) ]
		public T Data { get; set; }
	}
}