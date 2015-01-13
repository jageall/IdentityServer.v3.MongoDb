using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.New, "Scope")]
    public class CreateScope : PSCmdlet
    {
        static readonly Scope DefaultValues = new Scope();
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public ScopeClaim[] Claims { get; set; }

        [Parameter]
        public string ClaimsRule { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public string DisplayName { get; set; }
        [Parameter]
        public bool? Emphasize { get; set; }
        [Parameter]
        public bool? Enabled { get; set; }
        [Parameter]
        public bool? IncludeAllClaimsForUser { get; set; }
        [Parameter]
        public bool? ShowInDiscoveryDocument { get; set; }
        [Parameter]
        public bool? Required { get; set; }
        [Parameter]
        public ScopeType? Type { get; set; }

        protected override void ProcessRecord()
        {
            var scope = new Scope()
            {
                Claims = (Claims ?? new ScopeClaim[] {}).ToList(),
                ClaimsRule = ClaimsRule,
                Description = Description,
                DisplayName = DisplayName,
                Emphasize = Emphasize.GetValueOrDefault(DefaultValues.Emphasize),
                Enabled = Enabled.GetValueOrDefault(DefaultValues.Enabled),
                IncludeAllClaimsForUser =
                    IncludeAllClaimsForUser.GetValueOrDefault(DefaultValues.IncludeAllClaimsForUser),
                Name = Name,
                Required = Required.GetValueOrDefault(DefaultValues.Required),
                ShowInDiscoveryDocument =
                    ShowInDiscoveryDocument.GetValueOrDefault(DefaultValues.ShowInDiscoveryDocument),
                Type = Type.GetValueOrDefault(DefaultValues.Type)
            };
            
            WriteObject(scope);
        }
    }
}
