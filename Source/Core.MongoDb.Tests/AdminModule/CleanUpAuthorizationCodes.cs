using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Microsoft.Owin.Security.DataProtection;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class CleanUpAuthorizationCodes : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _script;
        private string _database;
        private ICleanupExpiredTokens _tokenCleanup;
        private IAuthorizationCodeStore _acStore;
        private IRefreshTokenStore _rtStore;
        private ITokenHandleStore _thStore;
        private const string Subject = "expired";


        [Fact]
        public void AuthorizationCodesAreDeleted()
        {
            Assert.NotEmpty(_acStore.GetAllAsync(Subject).Result);
            
            _ps.AddParameter("tokenType", TokenTypes.AuthorizationCode);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _acStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));
            
        }

        [Fact]
        public void RefreshTokensAreDeleted()
        {}

        [Fact]
        public void TokenHandlesAreDeleted()
        {}

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);
            _tokenCleanup = data.Factory.Resolve<ICleanupExpiredTokens>();
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            _acStore = factory.Resolve<IAuthorizationCodeStore>();
            _acStore.StoreAsync("ac", TestData.AuthorizationCode(Subject)).Wait();


        }

    }

    public class CleanUpRefreshTokens
    {
        public CleanUpRefreshTokens()
        {

            //_rtStore = factory.Resolve<IRefreshTokenStore>();
            //_thStore = factory.Resolve<ITokenHandleStore>();
            //_rtStore.StoreAsync("rt", TestData.RefreshToken(Subject)).Wait();
            //_thStore.StoreAsync("th", TestData.Token(Subject)).Wait();
        }
    }
}
