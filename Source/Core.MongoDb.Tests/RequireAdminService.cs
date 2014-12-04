using System;
using System.Linq;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;

namespace Core.MongoDb.Tests
{
    public class RequireAdminService : IDisposable
    {
        private readonly ServiceFactory _factory;
        private readonly IAdminService _adminService;
        public RequireAdminService()
        {
            var storeSettings = ServiceFactory.DefaultStoreSettings();
            storeSettings.Database = "testidentityserver";
            _factory = new ServiceFactory(
                null,
                storeSettings);
            _factory.ProtectClientSecretWith(new ReverseDataProtector());

            _adminService = Factory.AdminService.TypeFactory();
            _adminService.CreateDatabase();
        }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }

        public ServiceFactory Factory
        {
            get { return _factory; }
        }

        public void Dispose()
        {
            //_adminService.RemoveDatabase();
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