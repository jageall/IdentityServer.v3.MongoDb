using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Set, "Client")]
    public class SaveClient : MongoCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateNotNull]
        public Client Client { get; set; }
        
        protected override void ProcessRecord()
        {
            AdminService.Save(Client);
        }
    }
}