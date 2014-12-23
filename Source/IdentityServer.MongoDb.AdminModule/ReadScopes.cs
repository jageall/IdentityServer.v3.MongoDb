using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Get, "Scopes")]
    public class ReadScopes : MongoCmdlet
    {
        [Parameter(HelpMessage = "Gets the predefined standard scopes from identity server. These need to be persisted into the database using Set-Scope if you want them available to the application at runtime")]
        public SwitchParameter Predefined { get; set; }

        protected override void BeginProcessing()
        {
            if(!Predefined)
                base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<Scope> scopes;
            if (Predefined)
            {
                var builtin = BuiltInScopes();
                scopes = builtin;
            }
            else
            {
                scopes = ScopeStore.GetScopesAsync().Result;
            }

            foreach (var scope in scopes)
            {
                WriteObject(scope);
            }
        }

        public static IEnumerable<Scope> BuiltInScopes()
        {
            foreach (var scope in StandardScopes.All)
            {
                yield return scope;
            }
            yield return StandardScopes.AllClaims;
            yield return StandardScopes.OfflineAccess;
            yield return StandardScopes.Roles;
        }
    }
}
