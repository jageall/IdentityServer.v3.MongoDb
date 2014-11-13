using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class ClientStore : MongoDbStore, IClientStore
    {
        private readonly ClientSerializer _serializer;

        public ClientStore(MongoDatabase db, string collectionName) : base(db, collectionName)
        {
            _serializer = new ClientSerializer();
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            Client result = null;
            BsonDocument loaded = Collection.FindOneById(clientId);
            if (loaded != null)
            {
                result = _serializer.Deserialize(loaded);
            }

            return Task.FromResult(result);
        }
    }
}