using System;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.MongoDb.AdminModule
{
    public abstract class MongoCmdlet: PSCmdlet
    {
        private readonly bool _createDb;
        private IAdminService _adminService;
        private IScopeStore _scopeStore;
        private ICleanupExpiredTokens _tokenCleanupService;
        private SimpleResolver _resolver;

        protected MongoCmdlet(bool createDb = false)
        {
            _createDb = createDb;
            _resolver = new SimpleResolver();
            Register<IProtectClientSecrets>(new DoNotProtectClientSecrets());
        }

        [Parameter]
        public string ConnectionString { get; set; }
        
        [Parameter]
        public string Database { get; set; }
        
        [Parameter]
        public string ClientCollection { get; set; }
        [Parameter]
        public string ScopeCollection { get; set; }
        [Parameter]
        public string TokenHandleCollection { get; set; }
        [Parameter]
        public string RefreshTokenCollection { get; set; }
        [Parameter]
        public string ConsentCollection { get; set; }
        [Parameter]
        public string AuthorizationCodeCollection { get; set; }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }
        
        public IScopeStore ScopeStore
        {
            get { return _scopeStore; }
        }

        public ICleanupExpiredTokens TokenCleanupService
        {
            get { return _tokenCleanupService; }
        }

        protected override void BeginProcessing()
        {
            var storeSettings = ServiceFactory.DefaultStoreSettings();
            storeSettings.ConnectionString = ConnectionString ?? storeSettings.ConnectionString;
            storeSettings.Database = Database ?? storeSettings.Database;
            storeSettings.ClientCollection = ClientCollection ?? storeSettings.ClientCollection;
            storeSettings.ScopeCollection = ScopeCollection ?? storeSettings.ScopeCollection;
            storeSettings.ConsentCollection = ConsentCollection ?? storeSettings.ConsentCollection;
            storeSettings.AuthorizationCodeCollection = AuthorizationCodeCollection ?? storeSettings.AuthorizationCodeCollection;
            storeSettings.RefreshTokenCollection = RefreshTokenCollection ?? storeSettings.RefreshTokenCollection;
            storeSettings.TokenHandleCollection = TokenHandleCollection ?? storeSettings.TokenHandleCollection;
            CanCreateDatabase(storeSettings);
            
            var serviceFactory = new ServiceFactory(null, storeSettings);
            
            _adminService = serviceFactory.AdminService.TypeFactory(_resolver);
            _tokenCleanupService = serviceFactory.TokenCleanupService.TypeFactory(_resolver);
            _scopeStore = serviceFactory.ScopeStore.TypeFactory(_resolver);
            base.BeginProcessing();
        }

        void CanCreateDatabase(StoreSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var server = client.GetServer();
            if(!server.DatabaseExists(settings.Database) && !_createDb) throw new InvalidOperationException("Database does not exist");
        }

        protected void Register<T>(T instance)
        {
            _resolver.Register(instance);
        }
    }
}