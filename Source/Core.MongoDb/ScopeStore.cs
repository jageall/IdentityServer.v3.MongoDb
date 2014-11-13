using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
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

        public Task<IEnumerable<Scope>> GetScopesAsync()
        {
            return Task.FromResult(Collection.FindAll()
                .Select(x => _serializer.Deserialize(x)));
        }
    }
}