using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Internal;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsLifecycle.Install, "IdentityServerDb")]
    public class IdentityServerDb : MongoCmdlet
    {
        public IdentityServerDb():base(true)
        {
            
        }
        protected override void ProcessRecord()
        {
            var db = OpenDatabase();
            if (!db.CollectionExists(ClientCollection))
                db.CreateCollection(ClientCollection);

        }
    }
}
