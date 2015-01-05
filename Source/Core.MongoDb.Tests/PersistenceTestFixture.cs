using System.Linq;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace Core.MongoDb.Tests
{
    public class PersistenceTestFixture
    {
        private readonly Factory _factory;
        private readonly IAdminService _adminService;

        public PersistenceTestFixture()
        {
            var storeSettings = ServiceFactory.DefaultStoreSettings();
            storeSettings.Database = "testidentityserver";
            var config = new ServiceFactory(
                null,
                storeSettings);
            _factory = new Factory(config);
            _adminService = _factory.Resolve<IAdminService>();
            _adminService.CreateDatabase(expireUsingIndex:false);
        }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }

        public Factory Factory
        {
            get { return _factory; }
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