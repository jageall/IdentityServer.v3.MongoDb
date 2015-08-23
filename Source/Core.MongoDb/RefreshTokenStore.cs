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
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.MongoDb.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer3.MongoDb
{
    class RefreshTokenStore : MongoDbStore, IRefreshTokenStore
    {
        private readonly RefreshTokenSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<RefreshTokenStore>();
        public RefreshTokenStore(IMongoDatabase db, StoreSettings settings, IClientStore clientStore) : base(db, settings.RefreshTokenCollection)
        {
            _serializer = new RefreshTokenSerializer(clientStore);
        }

        public async Task StoreAsync(string key, RefreshToken value)
        {
            var result = await Collection.ReplaceOneAsync(
                Filter.ById(key),
                _serializer.Serialize(key, value),
                PerformUpsert
                ).ConfigureAwait(false);

            Log.Debug(result.ToString);
        }

        public async Task<RefreshToken> GetAsync(string key)
        {
            var result = await Collection.FindOneByIdAsync(key).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            return await _serializer.Deserialize(result).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key)
        {
            var result = await Collection.DeleteOneAsync(Filter.ById(key));
            Log.Debug(result.ToString);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var filter = new ObjectFilterDefinition<BsonDocument>(
                new
                {
                    _subjectId = subject
                });
            var results = await Collection.Find(filter).ToListAsync().ConfigureAwait(false);

            var deserializers = results.Select(_serializer.Deserialize);
            return await Task.WhenAll(deserializers).ConfigureAwait(false);
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var filter = new ObjectFilterDefinition<BsonDocument>(
                new
                {
                    _subjectId = subject, 
                    _clientId = client
                });
            var result = await Collection.DeleteManyAsync(filter).ConfigureAwait(false);
            Log.Debug(result.ToString);
        }
    }
}
