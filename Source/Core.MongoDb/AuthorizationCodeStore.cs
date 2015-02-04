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
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class AuthorizationCodeStore : MongoDbStore, IAuthorizationCodeStore
    {
        private static readonly ILog Log = LogProvider.For<AuthorizationCodeStore>();
        private readonly AuthorizationCodeSerializer _serializer;

        public AuthorizationCodeStore(MongoDatabase db, StoreSettings settings, IClientStore clientStore, IScopeStore scopeStore)
            : base(db, settings.AuthorizationCodeCollection)
        {
            _serializer = new AuthorizationCodeSerializer(clientStore, scopeStore);
        }

        public Task StoreAsync(string key, AuthorizationCode value)
        {
            Log.Debug("Storing authorization code with key" + key);
            BsonDocument doc = _serializer.Serialize(key, value);
            var result = Collection.Save(doc);
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public Task<AuthorizationCode> GetAsync(string key)
        {
            BsonDocument doc = Collection.FindOneById(key);
            if (doc == null)
            {
                Log.Debug("No authorization code found for key" + key);
                return Task.FromResult<AuthorizationCode>(null);
            }
            Log.Debug("Authorization code found for key " + key +". Deserializing...");
            return _serializer.Deserialize(doc);
        }

        public Task RemoveAsync(string key)
        {
            var result = Collection.Remove(new QueryWrapper(new {_id = key}));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var results = Collection.Find(new QueryWrapper(new {_subjectId = subject}))
                .Select(x=>_serializer.Deserialize(x)).ToArray();
            Log.Debug(()=> string.Format("Found {0} authorization codes for subject {1}", results.Length, subject));
            return Task.WhenAll(results).ContinueWith(x=>x.Result.OfType<ITokenMetadata>());
        }

        public Task RevokeAsync(string subject, string client)
        {
            var result = Collection.Remove(new QueryWrapper(new {_clientId = client, _subjectId = subject}));
            Log.Debug(result.Response.ToString);            
            return Task.FromResult(0);
        }
    }
}