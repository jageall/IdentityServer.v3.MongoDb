using System.Collections.Generic;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class ScopeSerializer
    {
        static readonly Scope Default = new Scope();
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
                        new ScopeClaim[] {}
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