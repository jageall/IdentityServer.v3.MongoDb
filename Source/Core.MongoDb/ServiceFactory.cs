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
            Func<IDependencyResolver, ClientSerializer> resolveClientSerializer = di => new ClientSerializer(di.Resolve<IProtectClientSecrets>());

            var client = new MongoClient(MongoClientSettings(storeSettings.ConnectionString));
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(storeSettings.Database);
            UserService = userService;
            ClientStore =
                Registration.RegisterFactory<IClientStore>(di => new ClientStore(
                    db, storeSettings.ClientCollection,
                    resolveClientSerializer(di)));
            ScopeStore = Registration.RegisterSingleton<IScopeStore>(new ScopeStore(db, storeSettings.ScopeCollection));
            ConsentStore =
                Registration.RegisterSingleton<IConsentStore>(new ConsentStore(db, storeSettings.ConsentCollection));
            
            AuthorizationCodeStore =
                Registration.RegisterFactory<IAuthorizationCodeStore>(di => new AuthorizationCodeStore(db,
                    storeSettings.AuthorizationCodeCollection, resolveClientSerializer(di)));

            RefreshTokenStore = Registration.RegisterFactory<IRefreshTokenStore>(di =>
                new RefreshTokenStore(db, storeSettings.RefreshTokenCollection, resolveClientSerializer(di)));

            TokenHandleStore = Registration.RegisterFactory<ITokenHandleStore>(di =>
                new TokenHandleStore(db, storeSettings.TokenHandleCollection, new ClientSerializer(di.Resolve<IProtectClientSecrets>())));
            AdminService = Registration.RegisterFactory<IAdminService>(di => new AdminService(db, storeSettings, resolveClientSerializer(di)));
            TokenCleanupService =
                Registration.RegisterSingleton<ICleanupExpiredTokens>(new CleanupExpiredTokens(db, storeSettings));
            Register(Registration.RegisterFactory<IProtectClientSecrets>(di => new DoNotProtectClientSecrets()));
        }

        public Registration<IAdminService> AdminService { get; set; }
        public Registration<ICleanupExpiredTokens> TokenCleanupService { get; set; }

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

            factory.Register(Registration.RegisterFactory<IProtectClientSecrets>(di => new ProtectClientSecretWithDataProtector(protector)));
            
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