using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Remove, "Client")]
    public class RemoveClient : MongoCmdlet
    {
        [Parameter(Mandatory = true), ValidateNotNullOrEmpty]
        public string ClientId { get; set; }

        protected override void ProcessRecord()
        {
            AdminService.DeleteClient(ClientId);
        }
    }
}
