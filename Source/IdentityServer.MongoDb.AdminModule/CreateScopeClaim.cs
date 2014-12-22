using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.New, "ScopeClaim")]
    public class CreateScopeClaim : PSCmdlet
    {
        [Parameter(Mandatory = true),
         ValidateNotNullOrEmpty]
        public string Name { get; set; }
        [Parameter]
        public bool? AlwaysIncludeInIdToken { get; set; }
        [Parameter]
        public string Description { get; set; }
        protected override void ProcessRecord()
        {
            var scopeClaim = new ScopeClaim();
            scopeClaim.Name = Name;
            scopeClaim.AlwaysIncludeInIdToken = AlwaysIncludeInIdToken.GetValueOrDefault(scopeClaim.AlwaysIncludeInIdToken);
            scopeClaim.Description = Description;
            base.WriteObject(scopeClaim);
        }
    }
}