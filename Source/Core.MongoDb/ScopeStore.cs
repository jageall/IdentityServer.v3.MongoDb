using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace IdentityServer.Core.MongoDb
{
    class ScopeStore : MongoDbStore, IScopeStore
    {
        private readonly ScopeSerializer _serializer;

        public ScopeStore(MongoDatabase db, string collectionName) : base(db, collectionName)
        {
            _serializer = new ScopeSerializer();
        }

        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var scopesAsBson = scopeNames
                .Select(s => BsonValue.Create(s));

            return Task.FromResult(
                Collection.AsQueryable()
                    .Where(doc => scopesAsBson.Contains(doc["_id"]))
                    .Select(doc => _serializer.Deserialize(doc))
                    .AsEnumerable()
            );
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            return Task.FromResult(Collection.FindAll()
                .Select(x => _serializer.Deserialize(x)));
        }
    }
}