using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Set, "Scope")]
    public class SaveScope : MongoCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateNotNull]
        public Scope Scope { get; set; }

        protected override void ProcessRecord()
        {
            AdminService.Save(Scope);
        }
    }
}