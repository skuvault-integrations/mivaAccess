using CsvHelper;
using CsvHelper.Configuration;
using MivaAccess.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MivaAccessTests
{
	public abstract class BaseTest
	{
		protected MivaConfig Config { get; private set; }

		public BaseTest()
		{
			var testCredentials = this.LoadTestSettings< TestCredentials >( @"\..\..\credentials.csv" );
			this.Config = new MivaConfig( new MivaCredentials( testCredentials.StoreUrl, testCredentials.StoreCode, testCredentials.AccessToken, testCredentials.PrivateKey, testCredentials.RootDirectory ) );
		}

		protected T LoadTestSettings< T >( string filePath )
		{
			string basePath = new Uri( Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase ) ).LocalPath;

			using( var streamReader = new StreamReader( basePath + filePath ) )
			{
				var csvConfig = new Configuration()
				{
					Delimiter = ",", 
					HasHeaderRecord = true
				};

				using( var csvReader = new CsvReader( streamReader, csvConfig ) )
				{
					var credentials = csvReader.GetRecords< T >();

					return credentials.FirstOrDefault();
				}
			}
		}
	}
}