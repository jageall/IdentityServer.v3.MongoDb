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
using MongoDB.Driver.Builders;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class ScopeStore : MongoDbStore, IScopeStore
    {
        private readonly ScopeSerializer _serializer;

        public ScopeStore(MongoDatabase db, StoreSettings settings) :
            base(db, settings.ScopeCollection)
        {
            _serializer = new ScopeSerializer();
        }

        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var results = Collection.Find(Query.In("_id", new BsonArray(scopeNames)));
            return Task.FromResult(
                results.Select(x => _serializer.Deserialize(x)));
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            if (publicOnly)
            {
                var results = Collection.Find(Query.EQ("showInDiscoveryDocument", new BsonBoolean(true)));
                return Task.FromResult(
                    results.Select(x => _serializer.Deserialize(x)));
            }

            return Task.FromResult(Collection.FindAll()
                .Select(x => _serializer.Deserialize(x)));
        }
    }
}