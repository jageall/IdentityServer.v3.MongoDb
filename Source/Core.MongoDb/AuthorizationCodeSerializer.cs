using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class AuthorizationCodeSerializer
    {
        private readonly ClientSerializer _clientSerializer;
        private readonly ScopeSerializer _scopeSerializer;
        private readonly ClaimSetSerializer _claimSetSerializer;

        public AuthorizationCodeSerializer()
        {
            _clientSerializer = new ClientSerializer();
            _scopeSerializer = new ScopeSerializer();
            _claimSetSerializer = new ClaimSetSerializer();
        }

        public BsonDocument Serialize(string key, AuthorizationCode code)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_clientId"] = code.ClientId;
            doc["_subjectId"] = code.SubjectId;
            doc["_expires"] = code.CreationTime.AddSeconds(code.Client.AuthorizationCodeLifetime);
            doc["creationTime"] = code.CreationTime;
            doc["isOpenId"] = code.IsOpenId;
            doc["redirectUri"] = code.RedirectUri.ToString();
            doc["wasConsentShown"] = code.WasConsentShown;
            doc["subject"] = SerializeIdentities(code);
            doc["client"] = _clientSerializer.Serialize(code.Client);
            var requestedScopes = new BsonArray();
            foreach (Scope scope in code.RequestedScopes)
            {
                requestedScopes.Add(_scopeSerializer.Serialize(scope));
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

        

        public AuthorizationCode Deserialize(BsonDocument doc)
        {
            var code = new AuthorizationCode();
            code.CreationTime = doc.GetValueOrDefault("creationTime", code.CreationTime);
            code.IsOpenId = doc.GetValueOrDefault("isOpenId", code.IsOpenId);
            code.RedirectUri = doc.GetValueOrDefault("redirectUri", code.RedirectUri);
            code.WasConsentShown = doc.GetValueOrDefault("wasConsentShown", code.WasConsentShown);


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

            code.Client = _clientSerializer.Deserialize(doc["client"].AsBsonDocument);

            code.RequestedScopes = doc.GetValueOrDefault(
                "requestedScopes",
                _scopeSerializer.Deserialize,
                code.RequestedScopes);
            return code;
        }
    }
}