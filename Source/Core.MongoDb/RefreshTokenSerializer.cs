using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class RefreshTokenSerializer
    {
        private readonly TokenSerializer _tokenSerializer;

        public RefreshTokenSerializer(ClientSerializer clientSerializer)
        {
            _tokenSerializer = new TokenSerializer(clientSerializer);
        }
        public BsonDocument Serialize(string key, RefreshToken value)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_expires"] = value.CreationTime.AddSeconds(value.LifeTime).ToBsonDateTime();
            var accessToken = new BsonDocument();
            _tokenSerializer.Serialize(accessToken ,value.AccessToken);
            doc["accessToken"] = accessToken;
            doc["clientId"] = value.ClientId;
            doc["creationTime"] = value.CreationTime.ToBsonDateTime();
            doc["lifetime"] = value.LifeTime;
            return doc;
        }

        public RefreshToken Deserialize(BsonDocument doc)
        {
            var token = new RefreshToken();
            token.AccessToken = doc.GetNestedValueOrDefault(
                "accessToken", 
                _tokenSerializer.Deserialize,
                token.AccessToken);

            token.ClientId = doc.GetValueOrDefault("clientId", token.ClientId);
            token.CreationTime = doc.GetValueOrDefault("creationTime", token.CreationTime);
            token.LifeTime = doc.GetValueOrDefault("lifetime", token.LifeTime);
            return token;
        }
    }
}