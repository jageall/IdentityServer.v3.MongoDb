using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Core.MongoDb
{
    public abstract class MongoDbStore
    {
        private readonly MongoDatabase _db;
        private readonly string _collectionName;

        protected MongoDbStore(MongoDatabase db, string collectionName)
        {
            _db = db;
            _collectionName = collectionName;
        }

        public MongoCollection<BsonDocument> Collection { get { return _db.GetCollection(_collectionName); } }
    }
}