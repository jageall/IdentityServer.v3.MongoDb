using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class CleanUpTokenHandles : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _script;
        private string _database;
        private ITokenHandleStore _thStore;
        private const string Subject = "expired";


        [Fact]
        public void RefreshTokensAreDeleted()
        {
            Assert.NotEmpty(_thStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _thStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));

        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            _thStore = factory.Resolve<ITokenHandleStore>();
            var admin = factory.Resolve<IAdminService>();
            var token = TestData.Token(Subject);
            admin.Save(token.Client);
            _thStore.StoreAsync("ac", token).Wait();
        }
    }
}