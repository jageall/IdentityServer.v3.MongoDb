using System.Collections.Generic;
using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Get, "Scopes")]
    public class ReadScopes : MongoCmdlet
    {
        [Parameter(HelpMessage = "Gets the predefined standard scopes from identity server. These need to be persisted into the database using Set-Scope if you want them available to the application at runtime")]
        public SwitchParameter Predefined { get; set; }

        protected override void ProcessRecord()
        {
            IEnumerable<Scope> scopes;
            if (Predefined)
            {
                scopes = StandardScopes.All;
            } else
            {
                scopes = ScopeStore.GetScopesAsync().Result;
            }

            foreach (var scope in scopes)
            {
                WriteObject(scope);
            }
        }
    }
}
