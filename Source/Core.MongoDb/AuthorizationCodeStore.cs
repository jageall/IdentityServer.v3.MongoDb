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

        public AuthorizationCodeStore(MongoDatabase db, string collectionName) : base(db, collectionName)
        {
            _serializer = new AuthorizationCodeSerializer();
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
            return Task.FromResult(_serializer.Deserialize(doc));
        }

        public Task RemoveAsync(string key)
        {
            Collection.Remove(new QueryWrapper(new {_id = key}));
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            AuthorizationCode[] results = Collection.Find(new QueryWrapper(new {_subjectId = subject}))
                .Select(_serializer.Deserialize).ToArray();
            return Task.FromResult<IEnumerable<ITokenMetadata>>(results);
        }

        public Task RevokeAsync(string subject, string client)
        {
            Collection.Remove(new QueryWrapper(new {_clientId = client, _subjectId = subject}));
            return Task.FromResult(0);
        }
    }
}