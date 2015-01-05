using System.Management.Automation;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsLifecycle.Install, "IdentityServerDb")]
    public class InstallDatabase : MongoCmdlet
    {
        public InstallDatabase()
            : base(true)
        {

        }

        protected override void ProcessRecord()
        {
            AdminService.CreateDatabase();
        }
    }
}
