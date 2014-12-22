using System;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class InstallDatabase : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _script;
        private string _database;
        private MongoServer _server;

        [Fact]
        public void CreateDatabase()
        {
            var defaults = ServiceFactory.DefaultStoreSettings();
            Assert.False(_server.DatabaseExists(_database));
            _ps.Invoke();
            Assert.True(_server.DatabaseExists(_database));
            var db = _server.GetDatabase(_database);
            Assert.True(db.CollectionExists(defaults.AuthorizationCodeCollection));
            Assert.True(db.CollectionExists(defaults.ClientCollection));
            Assert.True(db.CollectionExists(defaults.ConsentCollection));
            Assert.True(db.CollectionExists(defaults.RefreshTokenCollection));
            Assert.True(db.CollectionExists(defaults.ScopeCollection));
            Assert.True(db.CollectionExists(defaults.TokenHandleCollection));
            //TODO: verify indexes maybe?
            _server.DropDatabase(_database);
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            _server = data.Server;
        }
    }
}
