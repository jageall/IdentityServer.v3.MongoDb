using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class DeleteClient : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        private string _database;
        private IClientStore _store;
        private string _clientId;

        [Fact]
        public void RemoveClient()
        {
            Assert.NotNull(_store.FindClientByIdAsync(_clientId).Result);
            _ps.AddParameter("ClientId", _clientId);
            _ps.Invoke();
            
            Assert.Null(_data.GetPowershellErrors());
            Assert.Null(_store.FindClientByIdAsync(_clientId).Result);
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            _database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", _database);
            _store = data.Factory.Resolve<IClientStore>();
            var am = data.Factory.Resolve<IAdminService>();

            var client = TestData.ClientAllProperties();
            _clientId = client.ClientId;
            am.Save(client);
        }
    }
}
