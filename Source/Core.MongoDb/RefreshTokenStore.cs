using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class RefreshTokenStore : MongoDbStore, IRefreshTokenStore
    {
        private readonly RefreshTokenSerializer _serializer;

        public RefreshTokenStore(MongoDatabase db, string collectionName) : base(db, collectionName)
        {
            _serializer = new RefreshTokenSerializer();
        }

        public Task StoreAsync(string key, RefreshToken value)
        {
            Collection.Save(_serializer.Serialize(key, value));
            return Task.FromResult(0);
        }

        public Task<RefreshToken> GetAsync(string key)
        {
            var result = Collection.FindOneById(key);
            if (result == null) return Task.FromResult<RefreshToken>(null);
            return Task.FromResult(_serializer.Deserialize(result));
        }

        public Task RemoveAsync(string key)
        {
            Collection.Remove(new QueryWrapper(new {_id = key}));
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var result = Collection.Find(new QueryWrapper(new {subject})).Select(_serializer.Deserialize).ToArray();
            return Task.FromResult<IEnumerable<ITokenMetadata>>(result);
        }

        public Task RevokeAsync(string subject, string client)
        {
            Collection.Remove(new QueryWrapper(new { subject, client}));
            return Task.FromResult(0);
        }
    }
}
