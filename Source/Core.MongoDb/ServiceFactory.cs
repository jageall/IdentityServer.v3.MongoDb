using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService)
            : this(userService, DefaultStoreSettings())
        {
        }

        public ServiceFactory(Registration<IUserService> userService,
            StoreSettings storeSettings)
        {
            var client = new MongoClient(DefaultSettings(storeSettings.ConnectionString));
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(storeSettings.Database);
            UserService = userService;
            ClientStore =
                Registration.RegisterSingleton<IClientStore>(new ClientStore(db, storeSettings.ClientCollection));
            ScopeStore = Registration.RegisterSingleton<IScopeStore>(new ScopeStore(db, storeSettings.ScopeCollection));
            ConsentStore =
                Registration.RegisterSingleton<IConsentStore>(new ConsentStore(db, storeSettings.ConsentCollection));
            AuthorizationCodeStore =
                Registration.RegisterSingleton<IAuthorizationCodeStore>(new AuthorizationCodeStore(db,
                    storeSettings.AuthorizationCodeCollection));

            RefreshTokenStore = Registration.RegisterSingleton<IRefreshTokenStore>(
                new RefreshTokenStore(db, storeSettings.RefreshTokenCollection));

            TokenHandleStore = Registration.RegisterSingleton<ITokenHandleStore>(
                new TokenHandleStore(db, storeSettings.TokenHandleCollection));
            AdminService = Registration.RegisterSingleton<IAdminService>(new AdminService(db, storeSettings));
        }

        public Registration<IAdminService> AdminService { get; set; }

        private static MongoClientSettings DefaultSettings(string mongoUrl)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
            settings.GuidRepresentation = GuidRepresentation.Standard;
            return settings;
        }

        public static StoreSettings DefaultStoreSettings()
        {
            return new StoreSettings
            {
                ConnectionString = "mongodb://localhost",
                Database = "identityserver",
                ClientCollection = "clients",
                ScopeCollection = "scopes",
                ConsentCollection = "consents",
                AuthorizationCodeCollection = "authorizationCodes",
                RefreshTokenCollection = "refreshtokens",
                TokenHandleCollection = "tokenhandles"
            };
        }
    }
}