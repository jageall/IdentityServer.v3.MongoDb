using System;
using Microsoft.Owin.Security.DataProtection;
using MongoDB.Bson;
using MongoDB.Driver;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;
using IDataProtector = Thinktecture.IdentityServer.Core.Configuration.IDataProtector;

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
            var client = new MongoClient(MongoClientSettings(storeSettings.ConnectionString));
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(storeSettings.Database);
            Register(new Registration<MongoDatabase>(db));
            Register(new Registration<StoreSettings>(storeSettings));
            UserService = userService;
            ClientStore = new Registration<IClientStore>(typeof(ClientStore));
            ScopeStore = new Registration<IScopeStore>(typeof(ScopeStore));
            ConsentStore = new Registration<IConsentStore>(typeof (ConsentStore));
            
            AuthorizationCodeStore = new Registration<IAuthorizationCodeStore>(typeof(AuthorizationCodeStore));

            RefreshTokenStore = new Registration<IRefreshTokenStore>(typeof (RefreshTokenStore));
            TokenHandleStore = new Registration<ITokenHandleStore>(typeof (TokenHandleStore));
            Register(new Registration<ClientSerializer>(typeof(ClientSerializer)));
        }


        private static MongoClientSettings MongoClientSettings(string mongoUrl)
        {
            MongoClientSettings settings = MongoDB.Driver.MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
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