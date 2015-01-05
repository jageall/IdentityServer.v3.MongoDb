using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class TokenSerializer
    {
        private readonly IClientStore _clientStore;
        private readonly ClaimSetSerializer _claimsSetSerializer;

        public TokenSerializer(IClientStore clientStore)
        {
            _clientStore = clientStore;
            _claimsSetSerializer = new ClaimSetSerializer();
        }

        public BsonDocument Serialize(string key, Token token)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_expires"] = token.CreationTime.AddSeconds(token.Lifetime).ToBsonDateTime();
            doc["_clientId"] = token.ClientId;
            doc["_subjectId"] = token.SubjectId;
            Serialize(doc, token);
            return doc;
        }

        public void Serialize(BsonDocument doc, Token token)
        {
            doc["_version"] = 1;
            doc["audience"] = token.Audience;
            doc["claims"] = _claimsSetSerializer.Serialize(token.Claims);
            doc["client"] = token.Client.ClientId;
            doc["creationTime"] = token.CreationTime.ToBsonDateTime();
            doc["issuer"] = token.Issuer;
            doc["lifetime"] = token.Lifetime;
            doc["type"] = token.Type;
        }

        public async Task<Token> Deserialize(BsonDocument doc)
        {
            var token = new Token();
            token.Audience = doc.GetValueOrDefault("audience", token.Audience);
            token.Claims = new List<Claim>(doc.GetNestedValueOrDefault("claims", _claimsSetSerializer.Deserialize, new List<Claim>()));
            var clientId = doc.GetValueOrDefault("client", (string) null);
            var client = await _clientStore.FindClientByIdAsync(clientId);
            token.Client = client;
            token.CreationTime = doc.GetValueOrDefault("creationTime", token.CreationTime);
            token.Issuer = doc.GetValueOrDefault("issuer", token.Issuer);
            token.Lifetime = doc.GetValueOrDefault("lifetime", token.Lifetime);
            token.Type = doc.GetValueOrDefault("type", token.Type);
            return token;
        }
    }
}