using Thinktecture.IdentityServer.Core.Configuration;

namespace IdentityServer.Core.MongoDb
{
    public class StoreSettings
    {
        public string Database { get; set; }
        public string ClientCollection { get; set; }
        public string ScopeCollection { get; set; }
        public string ConsentCollection { get; set; }
        public string AuthorizationCodeCollection { get; set; }
        public string ConnectionString { get; set; }
        public string RefreshTokenCollection { get; set; }
        public string TokenHandleCollection { get; set; }
    }
}