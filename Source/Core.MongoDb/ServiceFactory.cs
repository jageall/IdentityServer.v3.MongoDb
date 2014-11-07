using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService)
            : this("mongodb://localhost", userService)
        { }

        public ServiceFactory(string mongoUrl, Registration<IUserService> userService)
            : this(mongoUrl, userService, DefaultStoreSettings())
        {}

        public ServiceFactory(string mongoUrl, Registration<IUserService> userService, 
            StoreSettings storeSettings)
        {
            var client = new MongoClient(DefaultSettings(mongoUrl));
            var server = client.GetServer();
            var db = server.GetDatabase(storeSettings.Database);
            UserService = userService;
            ClientStore =
                Registration.RegisterSingleton<IClientStore>(new ClientStore(db, storeSettings.ClientCollection));
            ScopeStore = Registration.RegisterSingleton<IScopeStore>(new ScopeStore(db, storeSettings.ScopeCollection));
            ConsentStore =
                Registration.RegisterSingleton<IConsentStore>(new ConsentStore(db, storeSettings.ConsentCollection));
        }

        private static MongoClientSettings DefaultSettings(string mongoUrl)
        {
            var settings = MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
            settings.GuidRepresentation = GuidRepresentation.Standard;
            return settings;
        }

        public static StoreSettings DefaultStoreSettings()
        {
            return new StoreSettings
            {
                Database = "identityserver",
                ClientCollection = "clients",
                ScopeCollection = "scopes",
                ConsentCollection = "consent"
            };
        }
    }
}
