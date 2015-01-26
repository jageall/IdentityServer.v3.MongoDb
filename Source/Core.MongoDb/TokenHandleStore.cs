using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class TokenHandleStore : MongoDbStore, ITokenHandleStore{
        private readonly TokenSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<TokenHandleStore>();
        public TokenHandleStore(MongoDatabase db, 
            StoreSettings settings, 
            IClientStore clientStore) 
            : base(db, settings.TokenHandleCollection)
        {
            _serializer = new TokenSerializer(clientStore);
        }

        public Task StoreAsync(string key, Token value)
        {
            var result = Collection.Save(_serializer.Serialize(key, value));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public Task<Token> GetAsync(string key)
        {
            var result = Collection.FindOneById(key);
            if (result == null) return Task.FromResult<Token>(null);
            return _serializer.Deserialize(result);
        }

        public Task RemoveAsync(string key)
        {
            var result = Collection.Remove(new QueryWrapper(new {_id = key}));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var results = Collection.Find(new QueryWrapper(new {_subjectId = subject})).Select(_serializer.Deserialize).ToArray();

            var result = await Task.WhenAll(results);
            return result;
        }

        public Task RevokeAsync(string subject, string client)
        {
            var result = Collection.Remove(new QueryWrapper(
                new
                {
                    _subjectId = subject, 
                    _clientId = client
                }));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }
    }
}