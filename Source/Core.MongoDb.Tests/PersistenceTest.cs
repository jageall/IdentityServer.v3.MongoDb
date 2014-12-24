using System.Linq;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace Core.MongoDb.Tests
{
    public abstract class PersistenceTest
    {
        private PersistenceTestFixture _data;

        protected Factory Factory
        {
            get { return _data.Factory; }
        }

        protected IAdminService AdminService
        {
            get { return _data.AdminService; }
        }

        public void SetFixture(PersistenceTestFixture data)
        {
            _data = data;


            Initialize();
        }

        protected abstract void Initialize();
    }
}