using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class TokenSerializer
    {
        private readonly ClaimSetSerializer _claimsSetSerializer;
        private readonly ClientSerializer _clientSerializer;

        public TokenSerializer()
        {
            _claimsSetSerializer = new ClaimSetSerializer();
            _clientSerializer = new ClientSerializer();
        }
        public BsonDocument Serialize(string key, Token token)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            Serialize(doc, token);
            return doc;
        }

        public void Serialize(BsonDocument doc, Token token)
        {
            doc["_version"] = 1;
            doc["audience"] = token.Audience;
            doc["claims"] = _claimsSetSerializer.Serialize(token.Claims);
            doc["client"] = _clientSerializer.Serialize(token.Client);
            doc["creationTime"] = token.CreationTime;
            doc["issuer"] = token.Issuer;
            doc["lifetime"] = token.Lifetime;
            doc["type"] = token.Type;
        }

        public Token Deserialize(BsonDocument doc)
        {
            var token = new Token();
            token.Audience = doc.GetValueOrDefault("audience", token.Audience);
            token.Claims = new List<Claim>(doc.GetNestedValueOrDefault("claims", _claimsSetSerializer.Deserialize, new List<Claim>()));
            token.Client = doc.GetNestedValueOrDefault("client", _clientSerializer.Deserialize, token.Client);
            token.CreationTime = doc.GetValueOrDefault("creationTime", token.CreationTime);
            token.Issuer = doc.GetValueOrDefault("issuer", token.Issuer);
            token.Lifetime = doc.GetValueOrDefault("lifetime", token.Lifetime);
            token.Type = doc.GetValueOrDefault("type", token.Type);
            return token;
        }
    }
}