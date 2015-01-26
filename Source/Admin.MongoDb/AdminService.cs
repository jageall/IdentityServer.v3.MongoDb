using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    internal class AdminService : IAdminService
    {
        private readonly MongoDatabase _db;
        private readonly StoreSettings _settings;
        private readonly ClientSerializer _clientSerializer;
        private static readonly ILog Log = LogProvider.For<AdminService>();

        public AdminService(MongoDatabase db, StoreSettings settings, ClientSerializer clientSerializer)
        {
            _db = db;
            _settings = settings;
            _clientSerializer = clientSerializer;
        }

        public void CreateDatabase(bool expireUsingIndex = true)
        {

            if (!_db.CollectionExists(_settings.ClientCollection))
            {
                var result = _db.CreateCollection(_settings.ClientCollection);
                Log.Debug(result.Response.ToString);
            }
            if (!_db.CollectionExists(_settings.ScopeCollection))
            {
                var result = _db.CreateCollection(_settings.ScopeCollection);
                Log.Debug(result.Response.ToString);
            }
            if (!_db.CollectionExists(_settings.ConsentCollection))
            {
                MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ConsentCollection);
                var result = collection.CreateIndex("subject");
                Log.Debug(result.Response.ToString);
                result = collection.CreateIndex("clientId", "subject");
                Log.Debug(result.Response.ToString);
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
                if (expireUsingIndex)
                {
                    options.SetTimeToLive(TimeSpan.FromSeconds(1));
                }
                MongoCollection<BsonDocument> collection = _db.GetCollection(tokenCollection);
                
                var result = collection.CreateIndex("_clientId", "_subjectId");
                Log.Debug(result.Response.ToString);
                
                result = collection.CreateIndex("_subjectId");
                Log.Debug(result.Response.ToString);
                try
                {
                    result = collection.CreateIndex(keys, options);
                    Log.Debug(result.Response.ToString);
                } catch (WriteConcernException)
                {
                    var cr = collection.DropIndex("_expires");
                    Log.Debug(cr.Response.ToString);
                    result = collection.CreateIndex(keys, options);
                    Log.Debug(result.Response.ToString);
                }
            }
        }

        public void Save(Scope scope)
        {
            BsonDocument doc = new ScopeSerializer().Serialize(scope);
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ScopeCollection);
            var result = collection.Save(doc);
            Log.Debug(result.Response.ToString);
        }

        public void Save(Client client)
        {
            BsonDocument doc = _clientSerializer.Serialize(client);
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ClientCollection);
            var result = collection.Save(doc);
            Log.Debug(result.Response.ToString);
        }

        public void RemoveDatabase()
        {
            _db.Drop();
        }

        public void DeleteClient(string clientId)
        {
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ClientCollection);
            collection.Remove(new QueryWrapper(new{_id = clientId}));
        }

        public void DeleteScope(string scopeName)
        {
            MongoCollection<BsonDocument> collection = _db.GetCollection(_settings.ScopeCollection);
            collection.Remove(new QueryWrapper(new { _id = scopeName}));
        }
    }
}