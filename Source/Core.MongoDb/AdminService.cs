using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    internal class AdminService : IAdminService
    {
        private readonly MongoDatabase _db;
        private readonly StoreSettings _settings;
        private readonly ClientSerializer _clientSerializer;

        public AdminService(MongoDatabase db, StoreSettings settings, ClientSerializer clientSerializer)
        {
            _db = db;
            _settings = settings;
            _clientSerializer = clientSerializer;
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
                var options = new IndexOptionsBuilder();
                var keys = new IndexKeysBuilder();
                keys.Ascending("_expires");
                options.SetTimeToLive(TimeSpan.FromSeconds(1));
                MongoCollection<BsonDocument> collection = _db.GetCollection(tokenCollection);
                collection.CreateIndex("_clientId", "_subjectId");
                collection.CreateIndex(keys, options);
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
            BsonDocument doc = _clientSerializer.Serialize(client);
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ClientCollection);
            collection.Save(doc);
        }

        public void RemoveDatabase()
        {
            _db.Drop();
        }
    }
}