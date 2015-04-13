using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Models;

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

        public void Save(Scope scope)
        {
            SaveAsync(scope).Wait();
        }

        private async Task SaveAsync(Scope scope)
        {
            BsonDocument doc = new ScopeSerializer().Serialize(scope);
            IMongoCollection<BsonDocument> collection = _data.Database.GetCollection<BsonDocument>(Settings.ScopeCollection);
            var result = await collection.ReplaceOneAsync(
                Filter.ById(scope.Name),
                doc,
                PerformUpsert
                ).ConfigureAwait(false);
        }
        public void Save(Client client)
        {
            SaveAsync(client).Wait();
        }

        private async Task SaveAsync(Client client)
        {
            BsonDocument doc = new ClientSerializer().Serialize(client);
            IMongoCollection<BsonDocument> collection = _data.Database.GetCollection<BsonDocument>(Settings.ClientCollection);
            var result = await collection.ReplaceOneAsync(
                Filter.ById(client.ClientId),
                doc,
                PerformUpsert
                ).ConfigureAwait(false);
        }
    }
}