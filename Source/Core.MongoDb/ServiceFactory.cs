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
            AdminService = new Registration<IAdminService>(typeof(AdminService));
            TokenCleanupService =
                new Registration<ICleanupExpiredTokens>(typeof (CleanupExpiredTokens));
            ClientSecretProtector = new Registration<IProtectClientSecrets>(di => new DoNotProtectClientSecrets());
            Register(new Registration<ClientSerializer>(typeof(ClientSerializer)));
        }

        public Registration<IAdminService> AdminService { get; set; }

        public Registration<ICleanupExpiredTokens> TokenCleanupService { get; set; }

        public Registration<IProtectClientSecrets> ClientSecretProtector { get; set; }

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

    public class DoNotProtectClientSecrets : IProtectClientSecrets
    {
        public string Protect(string clientId, string clientSecret)
        {
            return clientSecret;
        }

        public string Unprotect(string clientId, string clientSecret)
        {
            return clientSecret;
        }
    }

    //TODO: this class will die once identity server provides client secret protection
    public static class PatchingExtensionMethods
    {
        public static void ProtectClientSecretWithHostProtection(this IAppBuilder app, ServiceFactory factory)
        {
            var protector = app.GetDataProtectionProvider();
            if (protector != null)
            {
                factory.ProtectClientSecretWith(new WrappedDataProtector(protector));
            }    
        }

        public static void ProtectClientSecretWith(this ServiceFactory factory, IDataProtector protector)
        {
            factory.ClientSecretProtector = new Registration<IProtectClientSecrets>(di => new ProtectClientSecretWithDataProtector(protector));
        }
        
        internal class WrappedDataProtector : IDataProtector
        {
            private readonly IDataProtectionProvider _provider;

            public WrappedDataProtector(IDataProtectionProvider provider)
            {
                _provider = provider;
            }

            public byte[] Protect(byte[] data, string entropy = "")
            {
                var protector = _provider.Create(entropy);
                return protector.Protect(data);
            }

            public byte[] Unprotect(byte[] data, string entropy = "")
            {
                var protector = _provider.Create(entropy);
                return protector.Unprotect(data);
            }
        }
    }
}