using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Set, "Client")]
    public class SaveClient : MongoCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateNotNull]
        public Client Client { get; set; }
        
        [Parameter]
        public IDataProtector ClientSecretProtector { get; set; }



        protected override void BeginProcessing()
        {
            if (ClientSecretProtector != null)
                base.ProtectClientSecrets(ClientSecretProtector);
            else
            {
                WriteWarning("No client secret protector set");
            }

    base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            AdminService.Save(Client);
        }


    }
}