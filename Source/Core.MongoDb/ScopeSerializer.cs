using System.Collections.Generic;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    public class ScopeSerializer
    {
        public BsonDocument Serialize(Scope scope)
        {
            var doc = new BsonDocument();
            doc["_id"] = scope.Name;
            doc["DisplayName"] = scope.DisplayName;
            var claims = new BsonArray();
            foreach (var scopeClaim in scope.Claims)
            {
                var claim = new BsonDocument();
                claim["Name"] = scopeClaim.Name;
                claim["AlwaysIncludeInIdToken"] = scopeClaim.AlwaysIncludeInIdToken;
                claim.SetIfNotNull("Description", scopeClaim.Description);
                claims.Add(claim);
            }
            doc["Claims"] = claims;
            doc.SetIfNotNull("ClaimsRule", scope.ClaimsRule);
            doc.SetIfNotNull("Description", scope.Description);
            doc["Emphasize"] = scope.Emphasize;
            doc["Enabled"] = scope.Enabled;
            doc["IncludeAllClaimsForUser"] = scope.IncludeAllClaimsForUser;
            doc["Required"] = scope.Required;
            doc["ShowInDiscoveryDocument"] = scope.ShowInDiscoveryDocument;
            doc["Type"] = scope.Type.ToString();
            return doc;
        }

        public Scope Deserialize(BsonDocument doc)
        {
            var scope = new Scope
            {
                Name = doc["_id"].AsString,
                DisplayName = doc["DisplayName"].AsString,
                Claims = new List<ScopeClaim>(
                    doc.GetValueOrDefault(
                    "Claims",
                        claimDoc =>
                        {
                            var claim = new ScopeClaim();
                            claim.Name = claimDoc.GetValueOrDefault("Name", claim.Name);
                            claim.AlwaysIncludeInIdToken = claimDoc.GetValueOrDefault("AlwaysIncludeInIdToken", claim.AlwaysIncludeInIdToken);
                            claim.Description = claimDoc.GetValueOrDefault("Description", claim.Description);
                            return claim;
                        },
                        new ScopeClaim[] { }
                        )),

            };
            scope.ClaimsRule = doc.GetValueOrDefault("ClaimsRule", scope.ClaimsRule);
            scope.Description = doc.GetValueOrDefault("Description", scope.Description);
            scope.Emphasize = doc.GetValueOrDefault("Emphasize", scope.Emphasize);
            scope.Enabled = doc.GetValueOrDefault("Enabled", scope.Enabled);
            scope.IncludeAllClaimsForUser = doc.GetValueOrDefault("IncludeAllClaimsForUser", scope.IncludeAllClaimsForUser);
            scope.Required = doc.GetValueOrDefault("Required", scope.Required);
            scope.ShowInDiscoveryDocument = doc.GetValueOrDefault("ShowInDiscoveryDocument",
                scope.ShowInDiscoveryDocument);
            scope.Type = doc.GetValueOrDefault("Type", scope.Type);
            return scope;
        }
    }

}
