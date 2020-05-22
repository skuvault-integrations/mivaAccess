using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MivaAccess.Models.Infrastructure
{
	public abstract class MivaQuery : MivaRequest
	{
		[ JsonProperty( "Count" ) ]
		public int Count { get; protected set; }

		[ JsonProperty( "Offset" ) ]
		public int Offset { get; protected set; }

		[ JsonProperty( "Filter" ) ]
		public IEnumerable< MivaRequestFilter > Filters { get; set; }

		protected MivaQuery( MivaCredentials credentials, string functionName ) : base( credentials, functionName )
		{
			this.SetPage( 0, 1 );
		}

		public void SetPage( int page, int pageSize )
		{
			Condition.Requires( page, "page" ).IsGreaterOrEqual( 0 );
			Condition.Requires( pageSize, "pageSize" ).IsGreaterThan( 0 );

			this.Count = pageSize;
			this.Offset = pageSize * page;
			
			UpdateTimestamp();
		}
	}

	public abstract class MivaModuleQuery : MivaQuery
	{
		[ JsonProperty( "Module_Code" ) ]
		public string ModuleCode { get; private set; }
		
		[ JsonProperty( "Module_Function" ) ]
		public string ModuleFunction { get; private set; }

		protected MivaModuleQuery( MivaCredentials credentials, string moduleCode, string moduleFunction ) : base( credentials, "Module" )
		{
			Condition.Requires( moduleCode, "moduleCode" ).IsNotNullOrEmpty();
			Condition.Requires( moduleFunction, "moduleFunction" ).IsNotNullOrEmpty();

			this.ModuleCode = moduleCode;
			this.ModuleFunction = moduleFunction;
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
		[ JsonProperty( "value" ) ]
		public IEnumerable< string > Columns { get; private set; }

		public MivaRequestOnDemandFilter( IEnumerable< string > columns ) : base( "ondemandcolumns" )
		{
			Condition.Requires( columns, "columns" ).IsNotEmpty();

			this.Columns = columns;
		}
	}
}
