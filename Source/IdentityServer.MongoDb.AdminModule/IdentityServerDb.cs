using System.Management.Automation;
using IdentityServer.Core.MongoDb;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsLifecycle.Install, "IdentityServerDb")]
    public class IdentityServerDb : MongoCmdlet
    {
        public IdentityServerDb()
            : base(true)
        {

        }

        protected override void ProcessRecord()
        {
            AdminService.CreateDatabase();
        }
    }
}
