using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{

    public class AuthorizationCodeSerializer
    {
        private readonly ClientSerializer _clientSerializer;
        private readonly ScopeSerializer _scopeSerializer;

        public AuthorizationCodeSerializer()
        {
            _clientSerializer = new ClientSerializer();
            _scopeSerializer = new ScopeSerializer();
        }
        public BsonDocument Serialize(string key, AuthorizationCode code)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_clientId"] = code.ClientId;
            doc["_subjectId"] = code.SubjectId;
            doc["_expiry"] = code.CreationTime.AddSeconds(code.Client.AuthorizationCodeLifetime);
            doc["creationTime"] = code.CreationTime;
            doc["isOpenId"] = code.IsOpenId;
            doc["redirectUri"] = code.RedirectUri.ToString();
            doc["wasConsentShown"] = code.WasConsentShown;
            var subject = SerializeIdentities(code);
            doc["subject"] = subject;
            doc["client"] = _clientSerializer.Serialize(code.Client);
            var requestedScopes = new BsonArray();
            foreach (var scope in code.RequestedScopes)
            {
                requestedScopes.Add(_scopeSerializer.Serialize(scope));
            }
            doc["requestedScopes"] = requestedScopes;
            return doc;
        }

        private static BsonArray SerializeIdentities(AuthorizationCode code)
        {
            var subject = new BsonArray();
            foreach (var claimsIdentity in code.Subject.Identities)
            {
                var identity = new BsonDocument();

                identity["authenticationType"] = claimsIdentity.AuthenticationType;
                var claims = new BsonArray();
                foreach (var claim in claimsIdentity.Claims)
                {
                    var c = new BsonDocument();
                    c["type"] = claim.Type;
                    c["value"] = claim.Value;
                    claims.Add(c);
                }

                identity["claims"] = claims;
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
            var identities = doc.GetValueOrDefault("subject", sub =>
            {
                var authenticationType = sub.GetValueOrDefault("authenticationType", (string)null);
                var claims = sub.GetValueOrDefault(
                    "claims",
                    c => new Claim(c["type"].AsString, c["value"].AsString),
                    new Claim[] { });
                var identity = authenticationType == null ?
                    new ClaimsIdentity(claims) :
                    new ClaimsIdentity(claims, authenticationType);
                return identity;
            }, new ClaimsIdentity[] { });
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
