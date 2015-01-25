using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class ClientSerializer
    {
        private static readonly Client DefaultValues = new Client();
        private static readonly ClaimSetSerializer ClaimSetSerializer = new ClaimSetSerializer();
        private static readonly Dictionary<int, Func<BsonDocument, Client>> Deserializers = new Dictionary<int, Func<BsonDocument, Client>>()
        {
            {1, Version1}
        };

        public BsonDocument Serialize(Client client)
        {
            var doc = new BsonDocument();
            doc["_id"] = client.ClientId;
            doc["_version"] = 1;
            doc["absoluteRefreshTokenLifetime"] = client.AbsoluteRefreshTokenLifetime;
            doc["accessTokenLifetime"] = client.AccessTokenLifetime;
            doc["accessTokenType"] = client.AccessTokenType.ToString();
            doc["enableLocalLogin"] = client.EnableLocalLogin;
            doc["allowRememberConsent"] = client.AllowRememberConsent;
            doc["authorizationCodeLifetime"] = client.AuthorizationCodeLifetime;
            doc["clientName"] = client.ClientName;
            var secrets = new BsonArray();
            foreach (var clientSecret in client.ClientSecrets)
            {
                var secret = new BsonDocument();
                secret.SetIfNotNull("description", clientSecret.Description);
                secret.SetIfNotNull("value", clientSecret.Value);
                secret.SetIfNotNull("expiration", clientSecret.Expiration);
                secrets.Add(secret);
            }
            doc["clientSecrets"] = secrets;
            if (client.ClientUri != null)
                doc.SetIfNotNull("clientUri", client.ClientUri);
            doc["enabled"] = client.Enabled;
            doc["flow"] = client.Flow.ToString();
            var idpr = new BsonArray();
            foreach (string restriction in client.IdentityProviderRestrictions)
            {
                idpr.Add(restriction);
            }
            doc["identityProviderRestrictions"] = idpr;
            doc["identityTokenLifetime"] = client.IdentityTokenLifetime;
            doc.SetIfNotNull("logoUri", client.LogoUri);
            var postLogoutRedirectUris = new BsonArray();
            foreach (var uri in client.PostLogoutRedirectUris)
            {
                postLogoutRedirectUris.Add(uri);
            }

            var redirectUris = new BsonArray();
            foreach (var uri in client.RedirectUris)
            {
                redirectUris.Add(uri);
            }
            doc["redirectUris"] = redirectUris;
            doc["postLogoutRedirectUris"] = postLogoutRedirectUris;
            doc["refreshTokenExpiration"] = client.RefreshTokenExpiration.ToString();
            doc["refreshTokenUsage"] = client.RefreshTokenUsage.ToString();
            doc["requireConsent"] = client.RequireConsent;
            var scopeRestrictions = new BsonArray();
            foreach (string restriction in client.ScopeRestrictions)
            {
                scopeRestrictions.Add(restriction);
            }
            doc["scopeRestrictions"] = scopeRestrictions;
            doc["slidingRefreshTokenLifetime"] = client.SlidingRefreshTokenLifetime;
            doc["includeJwtId"] = client.IncludeJwtId;
            ClaimSetSerializer.Serialize(client.Claims, doc);
            doc["alwaysSendClientClaims"] = client.AlwaysSendClientClaims;
            doc["PrefixClientClaims"] = client.PrefixClientClaims;
            var grantRestrictions = new BsonArray();
            foreach (string restriction in client.CustomGrantTypeRestrictions)
            {
                grantRestrictions.Add(restriction);
            }
            doc["customGrantRestrictions"] = grantRestrictions;
            return doc;
        }

        public Client Deserialize(BsonDocument doc)
        {
            int version = doc["_version"].AsInt32;
            Func<BsonDocument, Client> deserializer;
            if (Deserializers.TryGetValue(version, out deserializer))
            {
                return deserializer(doc);
            }
            throw new InvalidOperationException("No deserializers available for client version " + version);
        }

        private static Client Version1(BsonDocument doc)
        {
            var client = new Client();

            client.ClientId = doc["_id"].AsString;
            
            client.ClientName = doc["clientName"].AsString;

            client.AbsoluteRefreshTokenLifetime = doc.GetValueOrDefault(
                "absoluteRefreshTokenLifetime",
                DefaultValues.AbsoluteRefreshTokenLifetime);
            client.AbsoluteRefreshTokenLifetime = doc.GetValueOrDefault(
                "absoluteRefreshTokenLifetime",
                DefaultValues.AbsoluteRefreshTokenLifetime);
            client.AccessTokenLifetime = doc.GetValueOrDefault(
                "accessTokenLifetime",
                DefaultValues.AccessTokenLifetime);

            client.AccessTokenType = doc.GetValueOrDefault(
                "accessTokenType",
                DefaultValues.AccessTokenType);

            client.EnableLocalLogin = doc.GetValueOrDefault(
                "enableLocalLogin",
                DefaultValues.EnableLocalLogin);

            client.AllowRememberConsent = doc.GetValueOrDefault(
                "allowRememberConsent", DefaultValues.AllowRememberConsent);
            client.AuthorizationCodeLifetime =
                doc.GetValueOrDefault("authorizationCodeLifetime",
                    DefaultValues.AuthorizationCodeLifetime);

            client.ClientSecrets = doc.GetValueOrDefault(
                "clientSecrets",
                d =>
                {
                    var value = d.GetValueOrDefault("value", "");
                    var description = d.GetValueOrDefault("description", (string)null);
                    var expiration = d.GetValueOrDefault("expiration", (DateTimeOffset?)null);
                    return new ClientSecret(value, description, expiration);
                }
                , new ClientSecret[] { }).ToList();

            client.ClientUri = doc.GetValueOrDefault(
                "clientUri",
                DefaultValues.ClientUri);
            client.Enabled = doc.GetValueOrDefault(
                "enabled",
                DefaultValues.Enabled);

            client.Flow = doc.GetValueOrDefault(
                "flow",
                DefaultValues.Flow);
            client.IdentityProviderRestrictions =
                doc["identityProviderRestrictions"].AsBsonArray.Select(x => x.AsString).ToList();
            client.IdentityTokenLifetime = doc.GetValueOrDefault(
                "identityTokenLifetime",
                DefaultValues.IdentityTokenLifetime);

            client.LogoUri = doc.GetValueOrDefault(
                "logoUri",
                DefaultValues.LogoUri);

            client.PostLogoutRedirectUris.AddRange(
                doc["postLogoutRedirectUris"].AsBsonArray.Select(x => x.AsString));

            client.RedirectUris.AddRange(
                doc["redirectUris"].AsBsonArray.Select(x => x.AsString));


            client.RefreshTokenExpiration = doc.GetValueOrDefault(
                "refreshTokenExpiration",
                DefaultValues.RefreshTokenExpiration);

            client.RefreshTokenUsage = doc.GetValueOrDefault(
                "refreshTokenUsage",
                DefaultValues.RefreshTokenUsage);

            client.RequireConsent = doc.GetValueOrDefault(
                "requireConsent",
                DefaultValues.RequireConsent);
            client.ScopeRestrictions.AddRange(doc["scopeRestrictions"].AsBsonArray.Select(x => x.AsString));
            client.SlidingRefreshTokenLifetime = doc.GetValueOrDefault(
                "slidingRefreshTokenLifetime",
                DefaultValues.SlidingRefreshTokenLifetime);

            client.IncludeJwtId = doc.GetValueOrDefault("includeJwtId", DefaultValues.IncludeJwtId);
            var claims = ClaimSetSerializer.Deserialize(doc);
            client.Claims = (claims ?? new List<Claim> { }).ToList();

            client.AlwaysSendClientClaims = doc.GetValueOrDefault("alwaysSendClientClaims", DefaultValues.AlwaysSendClientClaims);
            client.PrefixClientClaims = doc.GetValueOrDefault("PrefixClientClaims", DefaultValues.PrefixClientClaims);

            client.CustomGrantTypeRestrictions.AddRange(doc["customGrantRestrictions"].AsBsonArray.Select(x => x.AsString));

            return client;
        }
    }
}