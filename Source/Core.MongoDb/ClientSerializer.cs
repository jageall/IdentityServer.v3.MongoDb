using System;
using System.Linq;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    public class ClientSerializer
    {
        static readonly Client DefaultValues = new Client();
        public BsonDocument Serialize(Client client)
        {
            var doc = new BsonDocument();
            doc["_id"] = client.ClientId;
            doc["_version"] = 1;
            doc["AbsoluteRefreshTokenLifetime"] = client.AbsoluteRefreshTokenLifetime;
            doc["AccessTokenLifetime"] = client.AccessTokenLifetime;
            doc["AccessTokenType"] = client.AccessTokenType.ToString();
            doc["AllowLocalLogin"] = client.AllowLocalLogin;
            doc["AllowRememberConsent"] = client.AllowRememberConsent;
            doc["AuthorizationCodeLifetime"] = client.AuthorizationCodeLifetime;
            doc["ClientName"] = client.ClientName;
            doc.SetIfNotNull("ClientSecret", client.ClientSecret);
            if (client.ClientUri != null)
                doc.SetIfNotNull("ClientUri", client.ClientUri);
            doc["Enabled"] = client.Enabled;
            doc["Flow"] = client.Flow.ToString();
            var idpr = new BsonArray();
            foreach (var restriction in client.IdentityProviderRestrictions)
            {
                idpr.Add(restriction);
            }
            doc["IdentityProviderRestrictions"] = idpr;
            doc["IdentityTokenLifetime"] = client.IdentityTokenLifetime;
            doc["IdentityTokenSigningKeyType"] = client.IdentityTokenSigningKeyType.ToString();
            doc.SetIfNotNull("LogoUri", client.LogoUri);
            var postLogoutRedirectUris = new BsonArray();
            foreach (var uri in client.PostLogoutRedirectUris)
            {
                postLogoutRedirectUris.Add(uri.ToString());
            }

            var redirectUris = new BsonArray();
            foreach (var uri in client.RedirectUris)
            {
                redirectUris.Add(uri.ToString());
            }
            doc["RedirectUris"] = redirectUris;
            doc["PostLogoutRedirectUris"] = postLogoutRedirectUris;
            doc["RefreshTokenExpiration"] = client.RefreshTokenExpiration.ToString();
            doc["RefreshTokenUsage"] = client.RefreshTokenUsage.ToString();
            doc["RequireConsent"] = client.RequireConsent;
            var scopeRestrictions = new BsonArray();
            foreach (var restriction in client.ScopeRestrictions)
            {
                scopeRestrictions.Add(restriction);
            }
            doc["ScopeRestrictions"] = scopeRestrictions;
            doc["SlidingRefreshTokenLifetime"] = client.SlidingRefreshTokenLifetime;
            return doc;
        }

        public Client Deserialize(BsonDocument doc)
        {
            var client = new Client();
            //TODO: check document version

            //Required
            client.ClientId = doc["_id"].AsString;
            client.ClientName = doc["ClientName"].AsString;

            client.AbsoluteRefreshTokenLifetime = doc.GetValueOrDefault(
                "AbsoluteRefreshTokenLifetime",
                DefaultValues.AbsoluteRefreshTokenLifetime);
            client.AbsoluteRefreshTokenLifetime = doc.GetValueOrDefault(
                "AbsoluteRefreshTokenLifetime",
                DefaultValues.AbsoluteRefreshTokenLifetime);
            client.AccessTokenLifetime = doc.GetValueOrDefault(
                "AccessTokenLifetime",
                DefaultValues.AccessTokenLifetime);

            client.AccessTokenType = doc.GetValueOrDefault(
                "AccessTokenType",
                DefaultValues.AccessTokenType);

            client.AllowLocalLogin = doc.GetValueOrDefault(
                "AllowLocalLogin",
                DefaultValues.AllowLocalLogin);

            client.AllowRememberConsent = doc.GetValueOrDefault(
                "AllowRememberConsent", DefaultValues.AllowRememberConsent);
            client.AuthorizationCodeLifetime =
                doc.GetValueOrDefault("AuthorizationCodeLifetime",
                DefaultValues.AuthorizationCodeLifetime);

            client.ClientSecret = doc.GetValueOrDefault(
                "ClientSecret",
                DefaultValues.ClientSecret);

            client.ClientUri = doc.GetValueOrDefault(
                "ClientUri",
                DefaultValues.ClientUri);
            client.Enabled = doc.GetValueOrDefault(
                "Enabled",
                DefaultValues.Enabled);

            client.Flow = doc.GetValueOrDefault(
                "Flow",
                DefaultValues.Flow);
            client.IdentityProviderRestrictions =
                doc["IdentityProviderRestrictions"].AsBsonArray.Select(x => x.AsString);
            client.IdentityTokenLifetime = doc.GetValueOrDefault(
                "IdentityTokenLifetime",
                DefaultValues.IdentityTokenLifetime);

            client.IdentityTokenSigningKeyType = doc.GetValueOrDefault(
                "IdentityTokenSigningKeyType",
                DefaultValues.IdentityTokenSigningKeyType);

            client.LogoUri = doc.GetValueOrDefault(
                "LogoUri",
                DefaultValues.LogoUri);

            client.PostLogoutRedirectUris.AddRange(
                doc["PostLogoutRedirectUris"].AsBsonArray.Select(x => new Uri(x.AsString)));

            client.RedirectUris.AddRange(
                doc["RedirectUris"].AsBsonArray.Select(x => new Uri(x.AsString)));


            client.RefreshTokenExpiration = doc.GetValueOrDefault(
                "RefreshTokenExpiration", 
                DefaultValues.RefreshTokenExpiration);
            TokenUsage tokenUsage;
            if (Enum.TryParse(doc["RefreshTokenUsage"].AsString, out tokenUsage))
                client.RefreshTokenUsage = doc.GetValueOrDefault(
                    "RefreshTokenUsage", 
                    DefaultValues.RefreshTokenUsage);

            client.RequireConsent = doc.GetValueOrDefault(
                "RequireConsent", 
                DefaultValues.RequireConsent);
            client.ScopeRestrictions.AddRange(doc["ScopeRestrictions"].AsBsonArray.Select(x => x.AsString));
            client.SlidingRefreshTokenLifetime = doc.GetValueOrDefault(
                "SlidingRefreshTokenLifetime", 
                DefaultValues.SlidingRefreshTokenLifetime);
            return client;

        }
    }
}
