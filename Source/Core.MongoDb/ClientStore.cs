using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    public class ClientStore : MongoDbStore, IClientStore
    {
        private readonly ClientSerializer _serializer;

        public ClientStore(MongoDatabase db, string collectionName) : base(db, collectionName)
        {
            _serializer = new ClientSerializer();
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            Client result = null;
            var loaded = Collection.FindOneById(clientId);
            if (loaded != null)
            {
                result = _serializer.Deserialize(loaded);
            }

            return Task.FromResult(result);
        }
    }
}
