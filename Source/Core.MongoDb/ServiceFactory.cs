using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        private MongoDatabase _db;

        public ServiceFactory(Registration<IUserService> userService)
            : this("mongodb://localhost", userService)
        {}
        public ServiceFactory(string mongoUrl, Registration<IUserService> userService)
            : this(DefaultSettings(mongoUrl), 
            DefaultStoreSettings(),userService)
        {
            
        }
        public ServiceFactory(MongoClientSettings settings, StoreSettings storeSettings, Registration<IUserService> userService)
        {
            var client = new MongoClient(settings);
            var server = client.GetServer();
            _db = server.GetDatabase(storeSettings.Database);
            UserService = userService;
            ClientStore =
                Registration.RegisterSingleton<IClientStore>(new ClientStore(_db, storeSettings.ClientCollection));
            
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
