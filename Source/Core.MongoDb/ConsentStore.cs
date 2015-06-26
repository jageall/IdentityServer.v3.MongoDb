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
    class ConsentStore : MongoDbStore, IConsentStore
    {
        private readonly ConsentSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<ConsentStore>();

        public ConsentStore(IMongoDatabase db, StoreSettings settings) :
            base(db, settings.ConsentCollection)
        {
            _serializer = new ConsentSerializer();
        }

        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var docs = await Collection.Find(new ObjectFilterDefinition<BsonDocument>(
                new { subject })).ToListAsync().ConfigureAwait(false);
            
            return docs.Select(_serializer.Deserialize).ToArray();
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var result = await Collection.DeleteOneByIdAsync(ConsentSerializer.GetId(client, subject)).ConfigureAwait(false);

            Log.Debug(result.ToString);
        }

        public async Task<Consent> LoadAsync(string subject, string client)
        {
            BsonDocument found = await Collection.FindOneByIdAsync(ConsentSerializer.GetId(client, subject)).ConfigureAwait(false);

            if (found == null) return null;
            Consent result = _serializer.Deserialize(found);

            return result;
        }

        public async Task UpdateAsync(Consent consent)
        {
            var result = await Collection.ReplaceOneAsync(
                Filter.ById(ConsentSerializer.GetId(consent.ClientId, consent.Subject)),
                _serializer.Serialize(consent),
                PerformUpsert).ConfigureAwait(false);
            Log.Debug(result.ToString);
        }
    }
}