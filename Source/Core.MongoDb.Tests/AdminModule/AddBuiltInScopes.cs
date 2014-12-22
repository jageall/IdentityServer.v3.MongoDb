using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class AddBuiltInScopes : IUseFixture<PowershellAdminModuleFixture>
    {
        private IScopeStore _scopeStore;
        private PowerShell _ps;
        private string _script;
        private string _database;

        [Fact]
        public void VerifyAllBuiltInScopes()
        {
            _ps.Invoke();

            Assert.Equal(
                ReadScopes.BuiltInScopes()
                    .OrderBy(x => x.Name)
                    .Select(TestData.ToTestableString), 
                _scopeStore.GetScopesAsync(false).Result
                    .OrderBy(x=>x.Name)
                    .Select(TestData.ToTestableString)
                );
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            _scopeStore = data.Factory.ScopeStore.TypeFactory(data.DependencyResolver);
            var adminService = data.Factory.AdminService.TypeFactory(data.DependencyResolver);
            adminService.CreateDatabase();
        }
    }
}
