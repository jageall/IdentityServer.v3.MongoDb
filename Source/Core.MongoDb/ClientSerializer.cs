using System;
using System.Linq;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class ClientSerializer
    {
        private static readonly Client DefaultValues = new Client();

        public BsonDocument Serialize(Client client)
        {
            var doc = new BsonDocument();
            doc["_id"] = client.ClientId;
            doc["_version"] = 1;
            doc["absoluteRefreshTokenLifetime"] = client.AbsoluteRefreshTokenLifetime;
            doc["accessTokenLifetime"] = client.AccessTokenLifetime;
            doc["accessTokenType"] = client.AccessTokenType.ToString();
            doc["allowLocalLogin"] = client.AllowLocalLogin;
            doc["allowRememberConsent"] = client.AllowRememberConsent;
            doc["authorizationCodeLifetime"] = client.AuthorizationCodeLifetime;
            doc["clientName"] = client.ClientName;
            doc.SetIfNotNull("clientSecret", client.ClientSecret);
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
            doc["identityTokenSigningKeyType"] = client.IdentityTokenSigningKeyType.ToString();
            doc.SetIfNotNull("logoUri", client.LogoUri);
            var postLogoutRedirectUris = new BsonArray();
            foreach (Uri uri in client.PostLogoutRedirectUris)
            {
                postLogoutRedirectUris.Add(uri.ToString());
            }

            var redirectUris = new BsonArray();
            foreach (Uri uri in client.RedirectUris)
            {
                redirectUris.Add(uri.ToString());
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
            return doc;
        }

        public Client Deserialize(BsonDocument doc)
        {
            var client = new Client();
            //TODO: check document version

            //Required
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

            client.AllowLocalLogin = doc.GetValueOrDefault(
                "allowLocalLogin",
                DefaultValues.AllowLocalLogin);

            client.AllowRememberConsent = doc.GetValueOrDefault(
                "allowRememberConsent", DefaultValues.AllowRememberConsent);
            client.AuthorizationCodeLifetime =
                doc.GetValueOrDefault("authorizationCodeLifetime",
                    DefaultValues.AuthorizationCodeLifetime);

            client.ClientSecret = doc.GetValueOrDefault(
                "clientSecret",
                DefaultValues.ClientSecret);

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
                doc["identityProviderRestrictions"].AsBsonArray.Select(x => x.AsString);
            client.IdentityTokenLifetime = doc.GetValueOrDefault(
                "identityTokenLifetime",
                DefaultValues.IdentityTokenLifetime);

            client.IdentityTokenSigningKeyType = doc.GetValueOrDefault(
                "identityTokenSigningKeyType",
                DefaultValues.IdentityTokenSigningKeyType);

            client.LogoUri = doc.GetValueOrDefault(
                "logoUri",
                DefaultValues.LogoUri);

            client.PostLogoutRedirectUris.AddRange(
                doc["postLogoutRedirectUris"].AsBsonArray.Select(x => new Uri(x.AsString)));

            client.RedirectUris.AddRange(
                doc["redirectUris"].AsBsonArray.Select(x => new Uri(x.AsString)));


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
            return client;
        }
    }
}