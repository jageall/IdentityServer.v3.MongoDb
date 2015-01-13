using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class AuthorizationCodeStore : MongoDbStore, IAuthorizationCodeStore
    {
        private readonly AuthorizationCodeSerializer _serializer;

        public AuthorizationCodeStore(MongoDatabase db, StoreSettings settings, IClientStore clientStore, IScopeStore scopeStore)
            : base(db, settings.AuthorizationCodeCollection)
        {
            _serializer = new AuthorizationCodeSerializer(clientStore, scopeStore);
        }

        public Task StoreAsync(string key, AuthorizationCode value)
        {
            BsonDocument doc = _serializer.Serialize(key, value);
            Collection.Save(doc);
            return Task.FromResult(0);
        }

        public Task<AuthorizationCode> GetAsync(string key)
        {
            BsonDocument doc = Collection.FindOneById(key);
            if (doc == null) return Task.FromResult<AuthorizationCode>(null);
            return _serializer.Deserialize(doc);
        }

        public Task RemoveAsync(string key)
        {
            Collection.Remove(new QueryWrapper(new {_id = key}));
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var results = Collection.Find(new QueryWrapper(new {_subjectId = subject}))
                .Select(x=>_serializer.Deserialize(x)).ToArray();
            return Task.WhenAll(results).ContinueWith(x=>x.Result.OfType<ITokenMetadata>());
        }

        public Task RevokeAsync(string subject, string client)
        {
            Collection.Remove(new QueryWrapper(new {_clientId = client, _subjectId = subject}));
            return Task.FromResult(0);
        }
    }
}