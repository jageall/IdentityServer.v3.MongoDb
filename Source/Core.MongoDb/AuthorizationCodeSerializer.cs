using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    class AuthorizationCodeSerializer
    {
        private readonly IClientStore _clientStore;
        private readonly IScopeStore _scopeStore;
        private readonly static ClaimSetSerializer ClaimSetSerializer = new ClaimSetSerializer() ;
        private static readonly IReadOnlyDictionary<int, Func<BsonDocument, IClientStore, IScopeStore, Task<AuthorizationCode>>> Deserializers =
            new Dictionary<int, Func<BsonDocument, IClientStore, IScopeStore, Task<AuthorizationCode>>>
            {
                {1, Version1}
            };
        public AuthorizationCodeSerializer(IClientStore clientStore, IScopeStore scopeStore)
        {
            _clientStore = clientStore;
            _scopeStore = scopeStore;
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
                var claims = ClaimSetSerializer.Serialize(enumerable);

                identity["claimSet"] = claims;
                subject.Add(identity);
            }

            return subject;
        }
        
        public Task<AuthorizationCode> Deserialize(BsonDocument doc)
        {
            int version = doc["_version"].AsInt32;
            Func<BsonDocument, IClientStore, IScopeStore, Task<AuthorizationCode>> deserializer;
            if (Deserializers.TryGetValue(version, out deserializer))
            {
                return deserializer(doc, _clientStore, _scopeStore);
            }
            throw new InvalidOperationException("No deserializers available for authorization code version " + version);
        }

        private static async Task<AuthorizationCode> Version1(BsonDocument doc, IClientStore clientStore,
            IScopeStore scopeStore)
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
                string authenticationType = sub.GetValueOrDefault("authenticationType", (string)null);
                var claims = sub.GetNestedValueOrDefault("claimSet", ClaimSetSerializer.Deserialize, new Claim[] { });
                ClaimsIdentity identity = authenticationType == null
                    ? new ClaimsIdentity(claims)
                    : new ClaimsIdentity(claims, authenticationType);
                return identity;
            }, new ClaimsIdentity[] { });
            claimsPrincipal.AddIdentities(identities);
            code.Subject = claimsPrincipal;

            code.Client = await clientStore.FindClientByIdAsync(doc["_clientId"].AsString);

            var scopes = doc.GetValueOrDefault(
                "requestedScopes",
                (IEnumerable<string>)new string[] { });
            code.RequestedScopes = await scopeStore.FindScopesAsync(scopes);
            return code;
        }
    }
}