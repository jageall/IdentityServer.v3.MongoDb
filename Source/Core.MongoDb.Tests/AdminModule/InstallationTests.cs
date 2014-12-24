using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.MongoDb.AdminModule;
using Xunit;

namespace Core.MongoDb.Tests.AdminModule
{
    public class InstallationTests
    {
        [Fact]
        public void CanInstallDatabase()
        {
            using (var ps = PowerShell.Create())
            {
                ps.AddCommand("Import-Module").AddArgument(typeof (IdentityServerDb).Assembly.Location);
                ps.AddCommand("Install-IdentityServerDb").AddParameter("Database", "IdentityServerAdminScriptTests");

                ps.Invoke();
                Assert.Empty(ps.Streams.Error);
            }
        }
    }
}
