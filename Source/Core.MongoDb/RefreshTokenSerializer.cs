using System.Threading.Tasks;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class RefreshTokenSerializer
    {
        private readonly TokenSerializer _tokenSerializer;

        public RefreshTokenSerializer(IClientStore clientStore)
        {
            _tokenSerializer = new TokenSerializer(clientStore);
        }
        public BsonDocument Serialize(string key, RefreshToken value)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_expires"] = value.CreationTime.AddSeconds(value.LifeTime).ToBsonDateTime();
            doc["_clientId"] = value.ClientId;
            doc["_subjectId"] = value.SubjectId;
            var accessToken = new BsonDocument();
            _tokenSerializer.Serialize(accessToken ,value.AccessToken);
            doc["accessToken"] = accessToken;
            doc["creationTime"] = value.CreationTime.ToBsonDateTime();
            doc["lifetime"] = value.LifeTime;
            doc["version"] = value.Version;
            return doc;
        }

        public async Task<RefreshToken> Deserialize(BsonDocument doc)
        {
            var token = new RefreshToken();
            BsonValue at;
            if (doc.TryGetValue("accessToken", out at))
            {

               token.AccessToken = await _tokenSerializer.Deserialize(at.AsBsonDocument);
            }
            token.CreationTime = doc.GetValueOrDefault("creationTime", token.CreationTime);
            token.LifeTime = doc.GetValueOrDefault("lifetime", token.LifeTime);
            token.Version = doc.GetValueOrDefault("version", token.Version);
            return token;
        }
    }
}