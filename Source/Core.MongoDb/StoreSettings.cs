namespace IdentityServer.Core.MongoDb
{
    public class StoreSettings
    {
        public string Database { get; set; }
        public string ClientCollection { get; set; }
        public string ScopeCollection { get; set; }
        public string ConsentCollection { get; set; }
        public string AuthorizationCodeCollection { get; set; }
    }
}