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

using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using IdentityServer3.Core.Models;
using IdentityServer3.MongoDb;

namespace Core.MongoDb.Tests
{
    public abstract class PersistenceTest
    {
        private readonly PersistenceTestFixture _data;
        protected static readonly UpdateOptions PerformUpsert = new UpdateOptions() { IsUpsert = true };
        public PersistenceTest(PersistenceTestFixture data)
        {
            _data = data;
        }

        public StoreSettings Settings
        {
            get { return _data.Settings; }
        }

        public Factory Factory { get { return _data.Factory; }}

        protected async Task SaveAsync(Scope scope)
        {
            BsonDocument doc = new ScopeSerializer().Serialize(scope);
            IMongoCollection<BsonDocument> collection = _data.Database.GetCollection<BsonDocument>(Settings.ScopeCollection);
            var result = await collection.ReplaceOneAsync(
                Filter.ById(scope.Name),
                doc,
                PerformUpsert
                ).ConfigureAwait(false);
            Debug.WriteLine(result);
        }

        protected async Task SaveAsync(Client client)
        {
            BsonDocument doc = new ClientSerializer().Serialize(client);
            IMongoCollection<BsonDocument> collection = _data.Database.GetCollection<BsonDocument>(Settings.ClientCollection);
            var result = await collection.ReplaceOneAsync(
                Filter.ById(client.ClientId),
                doc,
                PerformUpsert
                ).ConfigureAwait(false);
            Debug.WriteLine(result);
        }
    }
}