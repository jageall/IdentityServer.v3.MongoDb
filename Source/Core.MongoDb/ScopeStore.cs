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
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer3.MongoDb
{
    class ScopeStore : MongoDbStore, IScopeStore
    {
        private readonly ScopeSerializer _serializer;

        public ScopeStore(IMongoDatabase db, StoreSettings settings) :
            base(db, settings.ScopeCollection)
        {
            _serializer = new ScopeSerializer();
        }

        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {

            var results = await Collection.Find(
                Builders<BsonDocument>.Filter.In("_id", new BsonArray(scopeNames))).ToListAsync().ConfigureAwait(false);
                
                
            return results.Select(x => _serializer.Deserialize(x)).ToArray();
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            List<BsonDocument> results;
            if (publicOnly)
            {
                results =
                    await
                        Collection.Find(new ObjectFilterDefinition<BsonDocument>(new {showInDiscoveryDocument = true}))
                            .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                results = await Collection.Find(new BsonDocument()).ToListAsync();
            }
            return results.Select(x => _serializer.Deserialize(x)).ToArray();
        }
    }
}