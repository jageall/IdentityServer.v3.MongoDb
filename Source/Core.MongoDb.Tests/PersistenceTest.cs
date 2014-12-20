using System.Linq;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace Core.MongoDb.Tests
{
    public abstract class PersistenceTest
    {
        private RequireAdminService _data;

        protected ServiceFactory Factory
        {
            get { return _data.Factory; }
        }

        protected IAdminService AdminService
        {
            get { return _data.AdminService; }
        }

        protected IDependencyResolver DependencyResolver
        {
            get
            {
                return _data.DependencyResolver;
            }
        }
        public void SetFixture(RequireAdminService data)
        {
            _data = data;


            Initialize();
        }

        protected abstract void Initialize();
    }
}