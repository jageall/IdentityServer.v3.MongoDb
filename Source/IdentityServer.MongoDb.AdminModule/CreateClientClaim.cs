using System.Management.Automation;
using System.Security.Claims;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.New, "ClientClaim")]
    public class CreateClientClaim : PSCmdlet
    {
        [Parameter, ValidateNotNullOrEmpty]
        public string Type { get; set; }
        [Parameter, ValidateNotNullOrEmpty]
        public string Value { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new Claim(Type, Value));
        }
    }
}