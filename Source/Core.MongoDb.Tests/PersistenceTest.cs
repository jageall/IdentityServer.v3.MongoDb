
using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Models;

namespace Core.MongoDb.Tests
{
    public abstract class PersistenceTest
    {
        private PersistenceTestFixture _data;

        public StoreSettings Settings
        {
            get { return _data.Settings; }
        }

        public Factory Factory { get { return _data.Factory; }}

        public void SetFixture(PersistenceTestFixture data)
        {
            _data = data;

            Initialize();
        }

        protected abstract void Initialize();

        public void Save(Scope scope)
        {
            BsonDocument doc = new ScopeSerializer().Serialize(scope);
            MongoCollection<BsonDocument> collection = _data.Database.GetCollection(Settings.ScopeCollection);
            var result = collection.Save(doc);
        }

        public void Save(Client client)
        {
            BsonDocument doc = new ClientSerializer().Serialize(client);
            MongoCollection<BsonDocument> collection = _data.Database.GetCollection(Settings.ClientCollection);
            var result = collection.Save(doc);
        }
    }
}