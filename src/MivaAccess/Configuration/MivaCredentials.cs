using CuttingEdge.Conditions;

namespace MivaAccess.Configuration
{
	public class MivaCredentials
	{
		public string StoreUrl { get; private set; }
		public string StoreCode { get; private set; }
		public string RootDirectory { get; private set; }
		public string AccessToken { get; private set; }
		public string PrivateKey { get; private set; }

		public MivaCredentials( string storeUrl, string storeCode, string accessToken, string privateKey, string rootDirectory = "mm5" )
		{
			Condition.Requires( storeUrl, "storeUrl" ).IsNotNullOrEmpty();
			Condition.Requires( storeCode, "storeCode" ).IsNotNullOrEmpty();
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrEmpty();
			Condition.Requires( rootDirectory, "folderName" ).IsNotNullOrEmpty();

			this.StoreUrl = storeUrl;
			this.StoreCode = storeCode;
			this.AccessToken = accessToken;
			this.PrivateKey = privateKey;
			this.RootDirectory = rootDirectory;
		}

		public MivaCredentials( string storeUrl, string storeCode, string accessToken, string rootDirectory = "mm5" ) : this( storeUrl, storeCode, accessToken, null, rootDirectory )
		{
		}
	}
}
