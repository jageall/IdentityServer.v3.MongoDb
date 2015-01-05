using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.Internal;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class DeleteDatabase : IUseFixture<PowershellAdminModuleFixture>
    {
        private string _database;
        private MongoServer _server;
        private PowerShell _ps;

        [Fact]
        public void DatabaseShouldBeDeleted()
        {
            Assert.True(_server.DatabaseExists(_database));
            _ps.Invoke();
            Assert.False(_server.DatabaseExists(_database));
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            var admin = data.Factory.Resolve<IAdminService>();
            admin.CreateDatabase();
            _ps = data.PowerShell;
            _database = data.Database;
            _server = data.Server; 
            var script = data.LoadScript(this);
            _ps.AddScript(script).AddParameter("Database", _database);
           
        }
    }
}
