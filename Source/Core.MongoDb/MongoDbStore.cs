using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Core.MongoDb
{
    abstract class MongoDbStore
    {
        private readonly string _collectionName;
        private readonly MongoDatabase _db;

        protected MongoDbStore(MongoDatabase db, string collectionName)
        {
            _db = db;
            _collectionName = collectionName;
        }

        protected MongoCollection<BsonDocument> Collection
        {
            get { return _db.GetCollection(_collectionName); }
        }
    }
}