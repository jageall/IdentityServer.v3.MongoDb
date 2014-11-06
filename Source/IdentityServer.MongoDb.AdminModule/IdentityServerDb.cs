using System.Management.Automation;

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
            if (!db.CollectionExists(ScopeCollection))
                db.CreateCollection(ScopeCollection);
        }
    }
}
