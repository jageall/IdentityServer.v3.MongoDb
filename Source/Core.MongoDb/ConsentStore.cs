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
    class ConsentStore : MongoDbStore, IConsentStore
    {
        private readonly ConsentSerializer _serializer;
        private static readonly ILog Log = LogProvider.For<ConsentStore>();
        public ConsentStore(MongoDatabase db, StoreSettings settings) :
            base(db, settings.ConsentCollection)
        {
            _serializer = new ConsentSerializer();
        }

        public Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            Consent[] result = Collection.Find(new QueryWrapper(
                new {subject})).Select(_serializer.Deserialize).ToArray();
            return Task.FromResult<IEnumerable<Consent>>(result);
        }

        public Task RevokeAsync(string subject, string client)
        {
            var result = Collection.Remove(QueryByClientAndSubject(subject, client));

            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        public Task<Consent> LoadAsync(string subject, string client)
        {
            BsonDocument found = Collection.FindOne(QueryByClientAndSubject(subject, client));

            if (found == null) return Task.FromResult<Consent>(null);
            Consent result = _serializer.Deserialize(found);

            return Task.FromResult(result);
        }

        public Task UpdateAsync(Consent consent)
        {
            var result = Collection.Save(_serializer.Serialize(consent));
            Log.Debug(result.Response.ToString);
            return Task.FromResult(0);
        }

        private QueryWrapper QueryByClientAndSubject(string subject, string client)
        {
            return new QueryWrapper(new {_id = ConsentSerializer.GetId(client, subject)});
        }
    }
}