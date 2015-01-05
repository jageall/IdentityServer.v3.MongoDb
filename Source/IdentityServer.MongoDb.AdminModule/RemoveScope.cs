using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Remove, "Scope")]
    public class RemoveScope : MongoCmdlet
    {
        [Parameter(Mandatory = true), ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            AdminService.DeleteScope(Name);
        }
    }
}
