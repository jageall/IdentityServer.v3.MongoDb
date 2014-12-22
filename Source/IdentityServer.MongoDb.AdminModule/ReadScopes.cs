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

        public static List<Scope> BuiltInScopes(bool alwaysInclude = false)
        {
            var scopeAccessors =
                typeof (StandardScopes).GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.PropertyType == typeof (Scope)
                    && alwaysInclude == x.Name.EndsWith("AlwaysInclude")
                    );
            var builtin = scopeAccessors.Select(scopeAccessor => (Scope) scopeAccessor.GetValue(null)).ToList();
            return builtin;
        }
    }
}
