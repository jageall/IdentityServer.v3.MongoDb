using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

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
            var results = Collection.Find(Query.In("_id", new BsonArray(scopeNames)));
            return Task.FromResult(
                results.Select(x => _serializer.Deserialize(x)));
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {

            if (publicOnly)
            {
                var results = Collection.Find(Query.EQ("showInDiscoveryDocument", new BsonBoolean(true)));
                return Task.FromResult(
                    results.Select(x => _serializer.Deserialize(x)));
            }

            return Task.FromResult(Collection.FindAll()
                .Select(x => _serializer.Deserialize(x)));
        }
    }
}