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
    class AuthorizationCodeStore : MongoDbStore, IAuthorizationCodeStore
    {
        private static readonly ILog Log = LogProvider.For<AuthorizationCodeStore>();
        private readonly AuthorizationCodeSerializer _serializer;

        public AuthorizationCodeStore(IMongoDatabase db, StoreSettings settings, IClientStore clientStore, IScopeStore scopeStore)
            : base(db, settings.AuthorizationCodeCollection)
        {
            _serializer = new AuthorizationCodeSerializer(clientStore, scopeStore);
        }

        public async Task StoreAsync(string key, AuthorizationCode value)
        {
            Log.Debug("Storing authorization code with key" + key);
            BsonDocument doc = _serializer.Serialize(key, value);
            var result = await Collection.ReplaceOneAsync(
                Filter.ById(key),
                doc,
                PerformUpsert).ConfigureAwait(false);
            Log.Debug(result.ToString);
        }

        public async Task<AuthorizationCode> GetAsync(string key)
        {
            BsonDocument doc = await Collection.FindOneByIdAsync(key).ConfigureAwait(false);
            if (doc == null)
            {
                Log.Debug("No authorization code found for key" + key);
                return null;
            }
            Log.Debug("Authorization code found for key " + key +". Deserializing...");
            return await _serializer.Deserialize(doc);
        }

        public async Task RemoveAsync(string key)
        {
            var result = await Collection.DeleteOneByIdAsync(key).ConfigureAwait(false);
            Log.Debug(result.ToString);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var docs = await Collection.Find(new ObjectFilterDefinition<BsonDocument>(new { _subjectId = subject })).ToListAsync().ConfigureAwait(false);
            var tokens = docs
                .Select(doc => _serializer.Deserialize(doc)
                    .ContinueWith(ac=>(ITokenMetadata)ac.Result));
            var results = (await Task.WhenAll(tokens)).ToArray();
            Log.Debug(() => string.Format("Found {0} authorization codes for subject {1}", results.Length, subject));
            return results;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var result = await Collection.DeleteManyAsync(new ObjectFilterDefinition<BsonDocument>(new { _clientId = client, _subjectId = subject })).ConfigureAwait(false);
            Log.Debug(result.ToString); 
        }
    }
}