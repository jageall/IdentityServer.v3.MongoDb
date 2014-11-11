using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    internal class AdminService : IAdminService
    {
        private readonly MongoDatabase _db;
        private readonly StoreSettings _settings;

        public AdminService(MongoDatabase db, StoreSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public void CreateDatabase()
        {
            if (!_db.CollectionExists(_settings.ClientCollection))
                _db.CreateCollection(_settings.ClientCollection);
            if (!_db.CollectionExists(_settings.ScopeCollection))
                _db.CreateCollection(_settings.ScopeCollection);
            if (!_db.CollectionExists(_settings.ConsentCollection))
            {
                MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ConsentCollection);
                collection.CreateIndex("subject");
            }

            var tokenCollections = new[]
            {
                _settings.AuthorizationCodeCollection,
                _settings.RefreshTokenCollection,
                _settings.TokenHandleCollection
            };
            foreach (string tokenCollection in tokenCollections)
            {
                if (!_db.CollectionExists(tokenCollection))
                {
                    MongoCollection<BsonDocument> collection = _db.GetCollection(tokenCollection);
                    collection.CreateIndex("_clientId", "_subjectId");
                }
            }
        }

        public void Save(Scope scope)
        {
            BsonDocument doc = new ScopeSerializer().Serialize(scope);
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ScopeCollection);
            collection.Save(doc);
        }

        public void Save(Client client)
        {
            BsonDocument doc = new ClientSerializer().Serialize(client);
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ClientCollection);
            collection.Save(doc);
        }

        public void RemoveDatabase()
        {
            // _db.Drop();
        }
    }
}