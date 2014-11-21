using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;
using MongoDB.Bson;
using MongoDB.Driver;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;
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
            TokenCleanupService =
                Registration.RegisterSingleton<ICleanupExpiredTokens>(new CleanupExpiredTokens(db, storeSettings));
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
            factory.ClientStore = Registration.RegisterSingleton<IClientStore>(
                new ProtectedClientStore(factory.ClientStore.TypeFactory, protector));

            factory.AdminService = Registration.RegisterSingleton<IAdminService>(
                new ProtectedAdminService(factory.AdminService.TypeFactory, protector));

            factory.AuthorizationCodeStore = Registration.RegisterSingleton<IAuthorizationCodeStore>(
                new ProtectedAuthorizationCodeStore(factory.AuthorizationCodeStore.TypeFactory, protector));

            factory.RefreshTokenStore = Registration.RegisterSingleton<IRefreshTokenStore>(
                new ProtectedRefreshTokenStore(factory.RefreshTokenStore.TypeFactory, protector));

            factory.TokenHandleStore = Registration.RegisterSingleton<ITokenHandleStore>(
                new ProtectedTokenHandleStore(factory.TokenHandleStore.TypeFactory, protector));
        }
        
        class ProtectedClientStore : IClientStore
        {
            private readonly Func<IClientStore> _factory;
            private readonly IDataProtector _protector;

            public ProtectedClientStore(
                Func<IClientStore> factory,
                IDataProtector protector)
            {
                _factory = factory;
                _protector = protector;
            }

            public async Task<Client> FindClientByIdAsync(string clientId)
            {
                var inner = _factory();
                var client = await inner.FindClientByIdAsync(clientId);
                Unprotect(client, _protector);
                return client;
            }
        }

        class ProtectedAdminService : IAdminService
        {
            private readonly Func<IAdminService> _factory;
            private readonly IDataProtector _protector;

            public ProtectedAdminService(Func<IAdminService> factory, IDataProtector protector)
            {
                _factory = factory;
                _protector = protector;
            }

            public void CreateDatabase()
            {
                _factory().CreateDatabase();
            }

            public void Save(Scope scope)
            {
                _factory().Save(scope);
            }

            public void Save(Client client)
            {
                Protect(client, _protector);
                _factory().Save(client);
            }

            public void RemoveDatabase()
            {
                _factory().RemoveDatabase();
            }
        }

        class ProtectedAuthorizationCodeStore : IAuthorizationCodeStore
        {
            private readonly Func<IAuthorizationCodeStore> _inner;
            private readonly IDataProtector _protector;

            public ProtectedAuthorizationCodeStore(Func<IAuthorizationCodeStore> inner, IDataProtector protector)
            {
                _inner = inner;
                _protector = protector;
            }

            public Task StoreAsync(string key, AuthorizationCode value)
            {
                Protect(value.Client, _protector);
                return _inner().StoreAsync(key, value);
            }

            public async Task<AuthorizationCode> GetAsync(string key)
            {
                var result = await _inner().GetAsync(key);
                Unprotect(result.Client, _protector);
                return result;
            }

            public Task RemoveAsync(string key)
            {
                return _inner().RemoveAsync(key);
            }

            public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
            {
                var results = await _inner().GetAllAsync(subject);
                return results
                    .OfType<AuthorizationCode>()
                    .Select(ac =>
                    {
                        Unprotect(ac.Client, _protector);
                        return ac;
                    })
                    .ToArray();

            }

            public Task RevokeAsync(string subject, string client)
            {
                return _inner().RevokeAsync(subject, client);
            }
        }

        class ProtectedTokenHandleStore : ITokenHandleStore
        {
            private readonly Func<ITokenHandleStore> _inner;
            private readonly IDataProtector _protector;

            public ProtectedTokenHandleStore(Func<ITokenHandleStore> inner, IDataProtector protector)
            {
                _inner = inner;
                _protector = protector;
            }

            public Task StoreAsync(string key, Token value)
            {
                Protect(value.Client, _protector);
                return _inner().StoreAsync(key, value);
            }

            public async Task<Token> GetAsync(string key)
            {
                var result = await _inner().GetAsync(key);
                Unprotect(result.Client, _protector);
                return result;
            }

            public Task RemoveAsync(string key)
            {
                return _inner().RemoveAsync(key);
            }

            public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
            {
                var result = await _inner().GetAllAsync(subject);

                return result
                    .OfType<Token>()
                    .Select(t =>
                    {
                        Unprotect(t.Client, _protector);
                        return t;
                    })
                    .ToArray();
            }

            public Task RevokeAsync(string subject, string client)
            {
                return _inner().RevokeAsync(subject, client);
            }
        }

        class ProtectedRefreshTokenStore : IRefreshTokenStore
        {
            private readonly Func<IRefreshTokenStore> _inner;
            private readonly IDataProtector _protector;

            public ProtectedRefreshTokenStore(Func<IRefreshTokenStore> inner, IDataProtector protector)
            {
                _inner = inner;
                _protector = protector;
            }

            public Task StoreAsync(string key, RefreshToken value)
            {
                Protect(value.AccessToken.Client, _protector);
                return _inner().StoreAsync(key, value);
            }

            public async Task<RefreshToken> GetAsync(string key)
            {
                var result = await _inner().GetAsync(key);
                Unprotect(result.AccessToken.Client, _protector);
                return result;
            }

            public Task RemoveAsync(string key)
            {
                return _inner().RemoveAsync(key);
            }

            public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
            {
                var result = await _inner().GetAllAsync(subject);

                return result
                    .OfType<Token>()
                    .Select(t =>
                    {
                        Unprotect(t.Client, _protector);
                        return t;
                    })
                    .ToArray();
            }

            public Task RevokeAsync(string subject, string client)
            {
                return _inner().RevokeAsync(subject, client);
            }
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

        static void Protect(Client client, IDataProtector protector)
        {
            var decoded = Convert.FromBase64String(client.ClientSecret);
            var bytes = protector.Protect(decoded);
            client.ClientSecret = Convert.ToBase64String(bytes);
        }

        static void Unprotect(Client client, IDataProtector protector)
        {
            var decoded = Convert.FromBase64String(client.ClientSecret);
            var bytes = protector.Unprotect(decoded);
            client.ClientSecret = Convert.ToBase64String(bytes);
        }
    }
}