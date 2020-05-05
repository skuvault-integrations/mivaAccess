using Newtonsoft.Json;

namespace MivaAccess.Models.Infrastructure
{
	public class MivaResponse
	{
		[ JsonProperty( "success" ) ]
		public int Success { get; set; }
		[ JsonProperty( "error_code" ) ]
		public string ErrorCode { get; set; }
		[ JsonProperty( "error_message" ) ]
		public string ErrorMessage { get; set; }
		[ JsonProperty( "processed" ) ]
		public int? Processed { get; set; }
	}

	public class MivaDataResponse < T > : MivaResponse where T : class
	{
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