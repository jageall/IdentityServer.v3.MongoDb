using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class ConsentStore : MongoDbStore, IConsentStore
    {
        private readonly ConsentSerializer _serializer;

        public ConsentStore(MongoDatabase db, string collectionName) : base(db, collectionName)
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
            Collection.Remove(QueryByClientAndSubject(subject, client));
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
            Collection.Save(_serializer.Serialize(consent));
            return Task.FromResult(0);
        }

        private QueryWrapper QueryByClientAndSubject(string subject, string client)
        {
            return new QueryWrapper(new {_id = _serializer.GetId(client, subject)});
        }
    }
}