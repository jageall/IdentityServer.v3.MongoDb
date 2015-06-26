/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using MongoDB.Bson;

namespace IdentityServer3.MongoDb
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
            doc.SetIfNotNull("nonce", code.Nonce);
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

        private static async Task<AuthorizationCode> Version1(
            BsonDocument doc, 
            IClientStore clientStore,
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

            var clientId = doc["_clientId"].AsString;
            code.Client = await clientStore.FindClientByIdAsync(clientId);
            if (code.Client == null)
            {
                throw new InvalidOperationException("Client not found when deserializing authorization code. Client id: " + clientId); 
            }

            var scopes = doc.GetValueOrDefault(
                "requestedScopes",
                (IEnumerable<string>)new string[] { }).ToArray();
            code.RequestedScopes = await scopeStore.FindScopesAsync(scopes);
            if (scopes.Count() > code.RequestedScopes.Count())
            {
                throw new InvalidOperationException("Scopes not found when deserializing authorization code. Scopes: " + string.Join(", ",scopes.Except(code.RequestedScopes.Select(x=>x.Name)))); 
            }
            return code;
        }
    }
}