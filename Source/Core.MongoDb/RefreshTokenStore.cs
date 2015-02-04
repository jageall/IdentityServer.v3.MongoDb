/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
    class RefreshTokenStore : MongoDbStore, IRefreshTokenStore
    {
        private readonly RefreshTokenSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<RefreshTokenStore>();
        public RefreshTokenStore(MongoDatabase db, StoreSettings settings, IClientStore clientStore) : base(db, settings.RefreshTokenCollection)
        {
            _serializer = new RefreshTokenSerializer(clientStore);
        }

        public Task StoreAsync(string key, RefreshToken value)
        {
            var result = Collection.Save(_serializer.Serialize(key, value));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public Task<RefreshToken> GetAsync(string key)
        {
            var result = Collection.FindOneById(key);
            if (result == null)
            {
                return Task.FromResult<RefreshToken>(null);
            }
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
            var filter = new QueryWrapper(
                new
                {
                    _subjectId = subject
                });
            var results = Collection.Find(filter)
                .Select(_serializer.Deserialize);
            var result = await Task.WhenAll(results);
            return result;
        }

        public Task RevokeAsync(string subject, string client)
        {
            var filter = new QueryWrapper(
                new
                {
                    _subjectId = subject, 
                    _clientId = client
                });
            var result = Collection.Remove(filter);
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }
    }
}
