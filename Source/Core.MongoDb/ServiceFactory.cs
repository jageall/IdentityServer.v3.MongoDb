using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Core.MongoDb
{
    public class ServiceFactory
    {
        private MongoDatabase _db;

        public ServiceFactory(): this("mongodb://localhost")
        {}
        public ServiceFactory(string mongoUrl):this(DefaultSettings(mongoUrl), 
            DefaultStoreSettings())
        {
            
        }
        public ServiceFactory(MongoClientSettings settings, StoreSettings storeSettings)
        {
            var client = new MongoClient(settings);
            var server = client.GetServer();
            _db = server.GetDatabase(storeSettings.Database);
        }

        public static MongoClientSettings DefaultSettings(string mongoUrl)
        {
            return new MongoClientSettings
            {
                GuidRepresentation = GuidRepresentation.Standard,
                Server = new MongoServerAddress(mongoUrl)
            };
        }

        public static StoreSettings DefaultStoreSettings()
        {
            return new StoreSettings
            {
                Database = "identityserver", 
                ClientCollection = "clients"
            };
        }
    }
}
