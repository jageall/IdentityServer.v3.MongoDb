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

using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer3.MongoDb
{
    abstract class MongoDbStore
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _db;
        protected static readonly UpdateOptions PerformUpsert = new UpdateOptions() {IsUpsert = true};

        protected MongoDbStore(IMongoDatabase db, string collectionName)
        {
            _db = db;
            _collectionName = collectionName;
        }

        protected IMongoCollection<BsonDocument> Collection
        {
            get { return _db.GetCollection<BsonDocument>(_collectionName); }
        }
    }
}