using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class AuthorizationCodeSerializer
    {
        private readonly IClientStore _clientStore;
        private readonly IScopeStore _scopeStore;
        private readonly ClaimSetSerializer _claimSetSerializer;

        public AuthorizationCodeSerializer(IClientStore clientStore, IScopeStore scopeStore)
        {
            _clientStore = clientStore;
            _scopeStore = scopeStore;
            _claimSetSerializer = new ClaimSetSerializer();
        }

        public BsonDocument Serialize(string key, AuthorizationCode code)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_clientId"] = code.ClientId;
            doc["_subjectId"] = code.SubjectId;
            doc["_expires"] = code.CreationTime.AddSeconds(code.Client.AuthorizationCodeLifetime).ToBsonDateTime();
            doc["creationTime"] = code.CreationTime.ToBsonDateTime();
            doc["isOpenId"] = code.IsOpenId;
            doc["redirectUri"] = code.RedirectUri;
            doc["wasConsentShown"] = code.WasConsentShown;
            doc["nonce"] = code.Nonce;
            doc["subject"] = SerializeIdentities(code);
            var requestedScopes = new BsonArray();
            foreach (var scope in code.RequestedScopes.Select(x=>x.Name))
            {
                requestedScopes.Add(scope);
            }
            doc["requestedScopes"] = requestedScopes;
            return doc;
        }

        private BsonArray SerializeIdentities(AuthorizationCode code)
        {
            var subject = new BsonArray();
            foreach (ClaimsIdentity claimsIdentity in code.Subject.Identities)
            {
                var identity = new BsonDocument();

                identity["authenticationType"] = claimsIdentity.AuthenticationType;
                var enumerable = claimsIdentity.Claims;
                var claims = _claimSetSerializer.Serialize(enumerable);

                identity["claimSet"] = claims;
                subject.Add(identity);
            }

            return subject;
        }
        
        public async Task<AuthorizationCode> Deserialize(BsonDocument doc)
        {
            var code = new AuthorizationCode();
            code.CreationTime = doc.GetValueOrDefault("creationTime", code.CreationTime);
            code.IsOpenId = doc.GetValueOrDefault("isOpenId", code.IsOpenId);
            code.RedirectUri = doc.GetValueOrDefault("redirectUri", code.RedirectUri);
            code.WasConsentShown = doc.GetValueOrDefault("wasConsentShown", code.WasConsentShown);
            code.Nonce = doc.GetValueOrDefault("nonce", code.Nonce);
            var claimsPrincipal = new ClaimsPrincipal();
            IEnumerable<ClaimsIdentity> identities = doc.GetValueOrDefault("subject", sub =>
            {
                string authenticationType = sub.GetValueOrDefault("authenticationType", (string) null);
                var claims = sub.GetNestedValueOrDefault("claimSet", _claimSetSerializer.Deserialize, new Claim[]{});
                ClaimsIdentity identity = authenticationType == null
                    ? new ClaimsIdentity(claims)
                    : new ClaimsIdentity(claims, authenticationType);
                return identity;
            }, new ClaimsIdentity[] {});
            claimsPrincipal.AddIdentities(identities);
            code.Subject = claimsPrincipal;

            code.Client = await _clientStore.FindClientByIdAsync(doc["_clientId"].AsString);
            
            var scopes = doc.GetValueOrDefault(
                "requestedScopes",
                (IEnumerable<string>)new string[]{});
            code.RequestedScopes = await _scopeStore.FindScopesAsync(scopes);
            return code;
        }
    }
}