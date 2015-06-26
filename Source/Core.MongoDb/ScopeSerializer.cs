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
using IdentityServer3.Core.Models;
using MongoDB.Bson;

namespace IdentityServer3.MongoDb
{
    class ScopeSerializer
    {
        static readonly Scope Default = new Scope();
        static readonly IReadOnlyDictionary<int, Func<BsonDocument, Scope>> Deserializers =
            new Dictionary<int, Func<BsonDocument, Scope>>
            {
                {1, Version1}
            };
        public BsonDocument Serialize(Scope scope)
        {
            var doc = new BsonDocument();
            doc["_id"] = scope.Name;
            doc["_version"] = 1;
            doc.SetIfNotNull("displayName", scope.DisplayName);
            var claims = new BsonArray();
            foreach (ScopeClaim scopeClaim in scope.Claims)
            {
                var claim = new BsonDocument();
                claim["name"] = scopeClaim.Name;
                claim["alwaysIncludeInIdToken"] = scopeClaim.AlwaysIncludeInIdToken;
                claim.SetIfNotNull("description", scopeClaim.Description);
                claims.Add(claim);
            }
            doc["claims"] = claims;
            doc.SetIfNotNull("claimsRule", scope.ClaimsRule);
            doc.SetIfNotNull("description", scope.Description);
            doc["emphasize"] = scope.Emphasize;
            doc["enabled"] = scope.Enabled;
            doc["includeAllClaimsForUser"] = scope.IncludeAllClaimsForUser;
            doc["required"] = scope.Required;
            doc["showInDiscoveryDocument"] = scope.ShowInDiscoveryDocument;
            doc["type"] = scope.Type.ToString();
            return doc;
        }

        public Scope Deserialize(BsonDocument doc)
        {
            int version = doc["_version"].AsInt32;
            Func<BsonDocument, Scope> deserializer;
            if (Deserializers.TryGetValue(version, out deserializer))
            {
                return deserializer(doc);
            }
            throw new InvalidOperationException("No deserializers available for scope version " + version);
        }

        private static Scope Version1(BsonDocument doc)
        {
            var scope = new Scope
            {
                Name = doc["_id"].AsString,
                DisplayName = doc.GetValueOrDefault("displayName", Default.DisplayName),
                Claims = new List<ScopeClaim>(
                    doc.GetValueOrDefault(
                        "claims",
                        claimDoc =>
                        {
                            var claim = new ScopeClaim();
                            claim.Name = claimDoc.GetValueOrDefault("name", claim.Name);
                            claim.AlwaysIncludeInIdToken = claimDoc.GetValueOrDefault("alwaysIncludeInIdToken",
                                claim.AlwaysIncludeInIdToken);
                            claim.Description = claimDoc.GetValueOrDefault("description", claim.Description);
                            return claim;
                        },
                        new ScopeClaim[] { }
                        )),
            };
            scope.ClaimsRule = doc.GetValueOrDefault("claimsRule", scope.ClaimsRule);
            scope.Description = doc.GetValueOrDefault("description", scope.Description);
            scope.Emphasize = doc.GetValueOrDefault("emphasize", scope.Emphasize);
            scope.Enabled = doc.GetValueOrDefault("enabled", scope.Enabled);
            scope.IncludeAllClaimsForUser = doc.GetValueOrDefault("includeAllClaimsForUser",
                scope.IncludeAllClaimsForUser);
            scope.Required = doc.GetValueOrDefault("required", scope.Required);
            scope.ShowInDiscoveryDocument = doc.GetValueOrDefault("showInDiscoveryDocument",
                scope.ShowInDiscoveryDocument);
            scope.Type = doc.GetValueOrDefault("type", scope.Type);
            return scope;
        }
    }
}