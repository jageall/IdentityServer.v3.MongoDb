using System;
using System.Management.Automation;
using MongoDB.Driver;

namespace IdentityServer.MongoDb.AdminModule
{
    public abstract class MongoCmdlet: PSCmdlet
    {
        private readonly bool _createDb;

        protected MongoCmdlet(bool createDb = false)
        {
            _createDb = createDb;
        }

        [Parameter]
        public string ConnectionString { get; set; }
        
        [Parameter]
        public string Database { get; set; }
        [Parameter]
        public string ClientCollection { get; set; }

        protected override void BeginProcessing()
        {
            ConnectionString = ConnectionString ?? "mongodb://localhost";

            Database = Database ?? "identityserver";
            ClientCollection = ClientCollection ?? "clients";
            
            base.BeginProcessing();
        }

        protected MongoDatabase OpenDatabase()
        {
            var client = new MongoClient(ConnectionString);
            var server = client.GetServer();
            if(!server.DatabaseExists(Database) && !_createDb) throw new InvalidOperationException("Database does not exist");
            return server.GetDatabase(Database);
        }
    }
}