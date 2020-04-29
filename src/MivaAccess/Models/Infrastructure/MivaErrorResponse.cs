using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MivaAccess.Models.Infrastructure
{
	public class MivaErrorResponse
	{
		[ JsonProperty( "success" ) ]
		public int Success { get; set; }
		[ JsonProperty( "error_code" ) ]
		public string ErrorCode { get; set; }
		[ JsonProperty( "error_message" ) ]
		public string ErrorMessage { get; set; }
	}
}