using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MivaAccess.Models.Infrastructure
{
	public abstract class MivaRequestBody
	{
		[ JsonProperty( "Store_Code" ) ]
		public string StoreCode { get; private set; }

		[ JsonProperty( "Function" ) ]
		public string FunctionName { get; private set; }

		[ JsonProperty( "Count" ) ]
		public int Count { get; private set; }

		[ JsonProperty( "Offset" ) ]
		public int Offset { get; private set; }

		[ JsonProperty( "Filter" ) ]
		public IEnumerable< MivaRequestFilter > Filters { get; set; }

		[ JsonProperty( "Miva_Request_Timestamp" ) ]
		public int Timestamp { get; private set; }

		public MivaRequestBody( MivaCredentials credentials, string functionName )
		{
			Condition.Requires( credentials, "credentials" ).IsNotNull();
			Condition.Requires( functionName, "functionName" ).IsNotNullOrEmpty();

			this.StoreCode = credentials.StoreCode;
			this.FunctionName = functionName;
			
			UpdateTimestamp();
		}

		public void SetPage( int page, int pageSize )
		{
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 0 );
			Condition.Requires( pageSize, "pageSize" ).IsGreaterThan( 0 );

			this.Count = pageSize;
			this.Offset = pageSize * page;
			
			UpdateTimestamp();
		}

		private void UpdateTimestamp()
		{
			this.Timestamp = DateTime.UtcNow.FromUtcTimeToEpoch();
		}
	}

	public abstract class MivaRequestFilter
	{
		[ JsonProperty( "name" ) ]
		protected string Name { get; set; }

		public MivaRequestFilter( string filterName )
		{
			Condition.Requires( filterName, "filterName" ).IsNotNullOrEmpty();

			this.Name = filterName;
		}
	}

	public class MivaRequestSearchFilter : MivaRequestFilter
	{
		[ JsonProperty( "value" ) ]
		public IEnumerable< MivaRequestFilterSearchValue > Values { get; private set; }

		public MivaRequestSearchFilter( IEnumerable< MivaRequestFilterSearchValue > values ) : base( "search" )
		{
			Condition.Requires( values, "value" ).IsNotEmpty();

			this.Values = values;
		}
	}

	public class MivaRequestFilterSearchValue
	{
		public string Field { get; set; }
		public string Operator { get; set; }
		public string Value { get; set; }
	}

	public class MivaRequestOnDemandFilter : MivaRequestFilter
	{
		public IEnumerable< string > Columns { get; private set; }

		public MivaRequestOnDemandFilter( IEnumerable< string > columns ) : base( "ondemandcolumns" )
		{
			Condition.Requires( columns, "columns" ).IsNotEmpty();

			this.Columns = columns;
		}
	}
}
