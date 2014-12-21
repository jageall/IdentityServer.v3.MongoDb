using System.Linq;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace Core.MongoDb.Tests
{
    public class PersistenceTestFixture
    {
        private readonly ServiceFactory _factory;
        private readonly IAdminService _adminService;
        private readonly IDependencyResolver _dependencyResolver;

        public PersistenceTestFixture()
        {
            var storeSettings = ServiceFactory.DefaultStoreSettings();
            storeSettings.Database = "testidentityserver";
            _factory = new ServiceFactory(
                null,
                storeSettings);
            var protector = new ReverseDataProtector();
            _factory.ProtectClientSecretWith(protector);
            var resolver = new SimpleResolver();
            resolver.Register<IProtectClientSecrets>(new ProtectClientSecretWithDataProtector(protector));
            _dependencyResolver = resolver;
            _adminService = Factory.AdminService.TypeFactory(resolver);
            _adminService.CreateDatabase(expireUsingIndex:false);
        }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }

        public ServiceFactory Factory
        {
            get { return _factory; }
        }

        public IDependencyResolver DependencyResolver
        {
            get { return _dependencyResolver; }
        }

        class ReverseDataProtector : IDataProtector
        {
            public byte[] Protect(byte[] data, string entropy = "")
            {
                return data.Reverse().ToArray();
            }

            public byte[] Unprotect(byte[] data, string entropy = "")
            {
                return data.Reverse().ToArray();
            }
        }
    }
}