﻿using CuttingEdge.Conditions;
using MivaAccess.Configuration;
using MivaAccess.Exceptions;
using MivaAccess.Models.Commands;
using MivaAccess.Models.Infrastructure;
using MivaAccess.Shared;
using MivaAccess.Throttling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MivaAccess.Services
{
	public abstract class ServiceBase
	{
		protected MivaConfig Config { get; private set; }
		protected Throttler Throttler { get; private set; }
		protected HttpClient HttpClient { get; private set; }
		protected Func< string > _additionalLogInfo;

		private const string _authorizationHeaderName = "X-Miva-API-Authorization";

		/// <summary>
		///	Extra logging information
		/// </summary>
		public Func< string > AdditionalLogInfo
		{
			get { return this._additionalLogInfo ?? ( () => string.Empty ); }
			set => _additionalLogInfo = value;
		}

		public ServiceBase( MivaConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this.Config = config;
			this.Throttler = new Throttler( config.ThrottlingOptions.MaxRequestsPerTimeInterval, config.ThrottlingOptions.TimeIntervalInSec, config.ThrottlingOptions.MaxRetryAttempts );

			HttpClient = new HttpClient()
			{
				BaseAddress = new Uri( Config.ApiBaseUrl ) 
			};
		}

		protected async Task< T > GetAsync< T >( MivaCommand command, CancellationToken cancellationToken, Mark mark = null, [ CallerMemberName ] string methodName = "" )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( cancellationToken.IsCancellationRequested )
			{
				var exceptionDetails = this.CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() );
				throw new MivaException( string.Format( "{0}. Task was cancelled", exceptionDetails ) );
			}

			var responseContent = await this.ThrottleRequestAsync( command, mark, methodName, HttpMethod.Get, async ( token ) =>
			{
				this.SetAuthHeader( command );
				var httpResponse = await HttpClient.GetAsync( command.Url ).ConfigureAwait( false );
				var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait( false );

				ThrowIfError( httpResponse, content );

				return content;
			}, cancellationToken ).ConfigureAwait( false );

			var response = JsonConvert.DeserializeObject< T >( responseContent );

			return response;
		}

		protected async Task< T > PostAsync< T >( MivaCommand command, CancellationToken cancellationToken, Mark mark = null, [ CallerMemberName ] string methodName = "" )
		{
			if ( mark == null )
				mark = Mark.CreateNew();

			if ( cancellationToken.IsCancellationRequested )
			{
				var exceptionDetails = this.CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo() );
				throw new MivaException( string.Format( "{0}. Task was cancelled", exceptionDetails ) );
			}

			var responseContent = await this.ThrottleRequestAsync( command, mark, methodName, HttpMethod.Post, async ( token ) =>
			{
				this.SetAuthHeader( command );
				var payload = new StringContent( command.Payload.ToJson(), Encoding.UTF8, "application/json" );
				payload.Headers.ContentType = MediaTypeHeaderValue.Parse( "application/json" );
				var httpResponse = await HttpClient.PostAsync( command.Url, payload ).ConfigureAwait( false );
				var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait( false );

				ThrowIfError( httpResponse, content );

				return content;
			}, cancellationToken ).ConfigureAwait( false );

			var response = JsonConvert.DeserializeObject< MivaResponse< T > >( responseContent );

			return response.Data.Data;
		}

		private void SetAuthHeader( MivaCommand command )
		{
			this.HttpClient.DefaultRequestHeaders.Remove( _authorizationHeaderName );
			var headerValue = new Authenticator( this.Config.Credentials ).GetAuthorizationHeaderValue( command.Payload.ToJson() );
			this.HttpClient.DefaultRequestHeaders.Add( _authorizationHeaderName, headerValue );
		}

		protected void ThrowIfError( HttpResponseMessage response, string message )
		{
			if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				throw new MivaUnauthorizedException( message );
			}

			MivaErrorResponse error = null;
			try
			{
				error = JsonConvert.DeserializeObject< MivaErrorResponse >( message );
			}
			catch { }
				
			if ( error != null && error.Success == 0 )
			{
				throw new MivaException( error.ErrorMessage ) {  ErrorCode = error.ErrorCode };
			}
		}

		private Task< T > ThrottleRequestAsync< T >( MivaCommand command, Mark mark, string methodName, HttpMethod methodType, Func< CancellationToken, Task< T > > processor, CancellationToken token )
		{
			return Throttler.ExecuteAsync( () =>
			{
				return new ActionPolicy( Config.NetworkOptions.RetryAttempts, Config.NetworkOptions.DelayBetweenFailedRequestsInSec, Config.NetworkOptions.DelayFailRequestRate )
					.ExecuteAsync( async () =>
					{
						Misc.InitSecurityProtocol();

						using( var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
						{
							MivaLogger.LogStarted( this.CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName ) );
							linkedTokenSource.CancelAfter( Config.NetworkOptions.RequestTimeoutMs );

							var result = await processor( linkedTokenSource.Token ).ConfigureAwait( false );

							MivaLogger.LogEnd( this.CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, responseBodyRaw: result.ToString(), additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName ) );

							return result;
						}
					}, 
					( exception, timeSpan, retryCount ) =>
					{
						string retryDetails = this.CreateMethodCallInfo( command.Url, mark, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName );
						MivaLogger.LogTraceRetryStarted( timeSpan.Seconds, retryCount, retryDetails );
					},
					( ex ) => CreateMethodCallInfo( command.Url, mark, methodType, payload: command.Payload, additionalInfo: this.AdditionalLogInfo(), libMethodName: methodName, errors: ex.Message ),
					MivaLogger.LogTraceException );
			} );
		}

		protected string CreateMethodCallInfo( string url = null, Mark mark = null, HttpMethod methodType = null, string errors = null, string responseBodyRaw = null, string additionalInfo = null, object payload = null, string libMethodName = null )
		{
			JObject responseBody = null;
			try
			{
				responseBody = JObject.Parse( responseBodyRaw );
			}
			catch { }

			return new CallInfo()
			{
				Mark = mark?.ToString() ?? "Unknown",
				Endpoint = url,
				Method = methodType?.ToString() ?? "Uknown",
				Body = payload,
				LibMethodName = libMethodName,
				AdditionalInfo = additionalInfo,
				Response = (object)responseBody ?? responseBodyRaw,
				Errors = errors
			}.ToJson();
		}
	}
}