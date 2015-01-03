using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class AddClient : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        private string _database;
        private IClientStore _store;

        [Fact]
        public void CheckClient()
        {

            _ps.Invoke();
            Assert.Null(_data.GetPowershellErrors());
            var client = _store.FindClientByIdAsync("test").Result;
            Assert.NotNull(client);
            Assert.Equal(10, client.AbsoluteRefreshTokenLifetime);
            Assert.Equal(20, client.AccessTokenLifetime);
            Assert.Equal(AccessTokenType.Reference, client.AccessTokenType);
            Assert.Equal(false, client.AllowLocalLogin);
            Assert.Equal(true, client.AllowRememberConsent);
            Assert.Equal(30, client.AuthorizationCodeLifetime);
            Assert.Equal("unittest", client.ClientName);
            Assert.Equal("5en6G6MezRroT3XKqkdPOmY/BfQ=", client.ClientSecret);
            Assert.Equal(true, client.Enabled);
            Assert.Equal(Flows.AuthorizationCode, client.Flow);
            Assert.Equal(new List<string>{"restriction1", "restriction2"}, client.IdentityProviderRestrictions);
            Assert.Equal(40, client.IdentityTokenLifetime);
            Assert.Equal(SigningKeyTypes.ClientSecret, client.IdentityTokenSigningKeyType);
            Assert.Equal("uri:logo", client.LogoUri);
            Assert.Equal(new List<string>{"uri:logout1", "uri:logout2"}, client.PostLogoutRedirectUris);
            Assert.Equal(new List<string>{"uri:redirect1", "uri:redirect2"}, client.RedirectUris);
            Assert.Equal(TokenExpiration.Sliding, client.RefreshTokenExpiration);
            Assert.Equal(TokenUsage.ReUse, client.RefreshTokenUsage);
            Assert.Equal(true, client.RequireConsent);
            Assert.Equal(new List<string>{"openid", "email", "roles"}, client.ScopeRestrictions);
            Assert.Equal(50, client.SlidingRefreshTokenLifetime);
            
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            _database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", _database);
            _store = data.Factory.Resolve<IClientStore>();
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase();
        }
    }
}
