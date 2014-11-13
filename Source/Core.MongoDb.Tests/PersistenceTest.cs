using IdentityServer.Core.MongoDb;

namespace Core.MongoDb.Tests
{
    public abstract class PersistenceTest
    {
        protected  ServiceFactory Factory;
        protected  IAdminService AdminService;

        public void SetFixture(RequireAdminService data)
        {
            Factory = data.Factory;
            AdminService = data.AdminService;
            Initialize();
        }

        protected abstract void Initialize();
    }
}