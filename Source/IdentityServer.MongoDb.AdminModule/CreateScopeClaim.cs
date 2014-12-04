using System;
using System.Collections.Generic;
using System.Linq;
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

    [Cmdlet(VerbsCommon.Set, "ScopeClaim")]
    public class SetScopeClaim : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Scope Scope { get; set; }

        [Parameter]
        public ScopeClaim[] Claims { get; set; }

        [Parameter]
        public SwitchParameter ReplaceExisting { get; set; }

        protected override void ProcessRecord()
        {
            var existing = Scope.Claims.ToList();
            foreach (var scopeClaim in Claims)
            {
                if (Claims.Any(x => String.Equals(x.Name, scopeClaim.Name, StringComparison.Ordinal) && x != scopeClaim))
                {
                    throw new ArgumentException("Claims cannot be specified more than once");
                }
            }
            var updated = new List<ScopeClaim>();
            if (!ReplaceExisting)
            {
                updated.AddRange(existing.Where(scopeClaim => !Claims.Any(x => String.Equals(x.Name, scopeClaim.Name, StringComparison.Ordinal))));
            }
            updated.AddRange(Claims);

            Scope.Claims = updated;

            WriteObject(Scope);
        }
    }
}