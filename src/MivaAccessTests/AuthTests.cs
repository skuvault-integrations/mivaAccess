using FluentAssertions;
using MivaAccess.Models.Requests;
using MivaAccess.Shared;
using NUnit.Framework;
using System;

namespace MivaAccessTests
{
	[ TestFixture ]
	public class AuthTests : BaseTest
	{
		[ Test ]
		public void GenerateAuthHeaderValue()
		{
			var authenticator = new Authenticator( base.Config.Credentials );
			var requestBody = new GetModifiedProductsRequestBody( base.Config.Credentials, DateTime.UtcNow.AddMonths( -1 ) );

			var headerValue = authenticator.GetAuthorizationHeaderValue( requestBody.ToJson() );

			headerValue.Should().NotBeNullOrEmpty();
		}
	}
}
