using System;
using System.Collections.Generic;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class DeleteScope : IUseFixture<PowershellAdminModuleFixture>
    {
        private IScopeStore _scopeStore;
        private PowerShell _ps;
        private const string ScopeName = "removethisscope";

        [Fact]
        public void ScopeIsRemoved()
        {
            Assert.NotEmpty(_scopeStore.FindScopesAsync(new[] { ScopeName }).Result);
            _ps.Invoke();
            Assert.Empty(_scopeStore.FindScopesAsync(new[] { ScopeName }).Result);
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            var admin = data.Factory.Resolve<IAdminService>();
            Scope scope = TestData.ScopeMandatoryProperties();
            scope.Name = ScopeName;
            admin.Save(scope);
            _ps = data.PowerShell;
            _ps.AddScript(data.LoadScript(this))
                .AddParameter("Database", data.Database)
                .AddParameter("Name", ScopeName);
            _scopeStore = data.Factory.Resolve<IScopeStore>();
        }
    }
}
