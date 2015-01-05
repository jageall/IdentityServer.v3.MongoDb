using System.Management.Automation;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsLifecycle.Uninstall, "IdentityServerDb")]
    public class UninstallDatabase : MongoCmdlet
    {
        public UninstallDatabase()
            : base(true)
        {

        }

        protected override void ProcessRecord()
        {
            AdminService.RemoveDatabase();
        }
    }
}