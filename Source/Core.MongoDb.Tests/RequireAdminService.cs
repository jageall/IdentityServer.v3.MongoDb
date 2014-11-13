using System;
using IdentityServer.Core.MongoDb;

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
            _adminService = Factory.AdminService.TypeFactory();
            _adminService.RemoveDatabase();
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
    }
}