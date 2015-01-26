using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class ClientStore : MongoDbStore, IClientStore
    {
        private readonly ClientSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<ClientStore>();
        public ClientStore(MongoDatabase db, StoreSettings settings, ClientSerializer serializer) :
            base(db, settings.ClientCollection)
        {
            _serializer = serializer;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            Client result = null;
            BsonDocument loaded = Collection.FindOneById(clientId);
            if (loaded != null)
            {
                result = _serializer.Deserialize(loaded);
            }
            else
            {
                Log.Debug("Client not found with id" + clientId);
            }

            return Task.FromResult(result);
        }
    }
}