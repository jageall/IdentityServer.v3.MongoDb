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
using IdentityServer3.Core.Models;
using MongoDB.Bson;

namespace IdentityServer3.MongoDb
{
    class ClientSerializer
    {
        private static readonly Client DefaultValues = new Client();
        private static readonly ClaimSetSerializer ClaimSetSerializer = new ClaimSetSerializer();
        private static readonly Dictionary<int, Func<BsonDocument, Client>> Deserializers = new Dictionary<int, Func<BsonDocument, Client>>
        {
            {1, Version1},
            {2, Version2}
        };

        private static readonly IEnumerable<string> EmptyStringSet = new string[] {};

        public BsonDocument Serialize(Client client)
        {
            var doc = new BsonDocument();
            doc["_id"] = client.ClientId;
            doc["_version"] = 2;
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
                secret.SetIfNotNull("type", clientSecret.Type);
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
            
            foreach (string restriction in client.AllowedScopes)
            {
                scopeRestrictions.Add(restriction);
            }
            doc["allowedScopes"] = scopeRestrictions;
            doc["slidingRefreshTokenLifetime"] = client.SlidingRefreshTokenLifetime;
            doc["includeJwtId"] = client.IncludeJwtId;
            var clientClaims = new BsonDocument();
            doc["clientClaims"] = clientClaims;
            ClaimSetSerializer.Serialize(client.Claims, clientClaims);
            doc["alwaysSendClientClaims"] = client.AlwaysSendClientClaims;
            doc["PrefixClientClaims"] = client.PrefixClientClaims;
            var grantRestrictions = new BsonArray();
            
            foreach (string restriction in client.AllowedCustomGrantTypes)
            {
                grantRestrictions.Add(restriction);
            }
            doc["allowedCustomGrantTypes"] = grantRestrictions;
            doc["allowClientCredentialsOnly"] = client.AllowClientCredentialsOnly;
            doc["updateAccessTokenClaimsOnRefresh"] = client.UpdateAccessTokenClaimsOnRefresh;
            doc["updateAccessTokenClaimsOnRefresh"] = client.UpdateAccessTokenClaimsOnRefresh;
            var allowedCorsOrigins = new BsonArray();
            foreach (var origin in client.AllowedCorsOrigins)
            {
                if(!string.IsNullOrEmpty(origin))
                    allowedCorsOrigins.Add(origin);
            }
            doc["allowedCorsOrigins"] = allowedCorsOrigins;
            doc["allowAccessToAllScopes"] = client.AllowAccessToAllScopes;
            doc["allowAccessToAllCustomGrantTypes"] = client.AllowAccessToAllCustomGrantTypes;
            doc["allowClientCredentialsOnly"] = client.AllowClientCredentialsOnly;
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
            var client = Unchanged(doc);
            
            //CHANGE
            client.AllowedScopes.AddRange(doc.GetValueOrDefault("scopeRestrictions", EmptyStringSet));
            //CHANGE
            client.AllowedCustomGrantTypes.AddRange(doc.GetValueOrDefault("customGrantRestrictions", EmptyStringSet));

            client.Claims.AddRange(ClaimSetSerializer.Deserialize(doc));

            return client;
        }

        private static Client Version2(BsonDocument doc)
        {
            var client = Unchanged(doc);
            //CHANGE
            client.AllowedScopes.AddRange(doc.GetValueOrDefault("allowedScopes", EmptyStringSet));
            //CHANGE
            client.AllowedCustomGrantTypes.AddRange(doc.GetValueOrDefault("allowedCustomGrantTypes", EmptyStringSet));
            client.AllowAccessToAllScopes = doc.GetValueOrDefault("allowAccessToAllScopes",
                client.AllowAccessToAllScopes);
            client.AllowAccessToAllCustomGrantTypes = doc.GetValueOrDefault("allowAccessToAllCustomGrantTypes",
                client.AllowAccessToAllCustomGrantTypes);
            //client.AllowClientCredentialsOnly = doc.GetValueOrDefault("allowClientCredentialsOnly",
            //    client.AllowClientCredentialsOnly);

            client.Claims.AddRange(doc.GetNestedValueOrDefault("clientClaims", ClaimSetSerializer.Deserialize,
                new Claim[] {}));
            return client;
        }

        private static Client Unchanged(BsonDocument doc)
        {
            var client = new Client
            {
                ClientId = doc["_id"].AsString,
                ClientName = doc["clientName"].AsString,
                AbsoluteRefreshTokenLifetime = doc.GetValueOrDefault(
                    "absoluteRefreshTokenLifetime",
                    DefaultValues.AbsoluteRefreshTokenLifetime),
                AccessTokenLifetime = doc.GetValueOrDefault(
                    "accessTokenLifetime",
                    DefaultValues.AccessTokenLifetime),
                AccessTokenType = doc.GetValueOrDefault(
                    "accessTokenType",
                    DefaultValues.AccessTokenType),
                EnableLocalLogin = doc.GetValueOrDefault(
                    "enableLocalLogin",
                    DefaultValues.EnableLocalLogin),
                AllowRememberConsent = doc.GetValueOrDefault(
                    "allowRememberConsent", DefaultValues.AllowRememberConsent),
                AuthorizationCodeLifetime = doc.GetValueOrDefault("authorizationCodeLifetime",
                    DefaultValues.AuthorizationCodeLifetime),
                ClientSecrets = doc.GetValueOrDefault(
                "clientSecrets",
                d =>
                {
                    var value = d.GetValueOrDefault("value", "");
                    var description = d.GetValueOrDefault("description", (string)null);
                    var expiration = d.GetValueOrDefault("expiration", (DateTimeOffset?)null);
                    var type = d.GetValueOrDefault("type", (string)null);
                    return new Secret(value, description, expiration) { Type = type };
                }
                , new Secret[] { }).ToList(),
                ClientUri = doc.GetValueOrDefault(
                    "clientUri",
                    DefaultValues.ClientUri),
                Enabled = doc.GetValueOrDefault(
                    "enabled",
                    DefaultValues.Enabled),
                Flow = doc.GetValueOrDefault(
                    "flow",
                    DefaultValues.Flow),
                IdentityProviderRestrictions =
                    doc["identityProviderRestrictions"].AsBsonArray.Select(x => x.AsString).ToList(),
                IdentityTokenLifetime = doc.GetValueOrDefault(
                    "identityTokenLifetime",
                    DefaultValues.IdentityTokenLifetime),
                LogoUri = doc.GetValueOrDefault(
                    "logoUri",
                    DefaultValues.LogoUri),
                RefreshTokenExpiration = doc.GetValueOrDefault(
                    "refreshTokenExpiration",
                    DefaultValues.RefreshTokenExpiration),
                RefreshTokenUsage = doc.GetValueOrDefault(
                    "refreshTokenUsage",
                    DefaultValues.RefreshTokenUsage),
                RequireConsent = doc.GetValueOrDefault(
                    "requireConsent",
                    DefaultValues.RequireConsent),
                SlidingRefreshTokenLifetime = doc.GetValueOrDefault(
                    "slidingRefreshTokenLifetime",
                    DefaultValues.SlidingRefreshTokenLifetime),
                IncludeJwtId = doc.GetValueOrDefault("includeJwtId", DefaultValues.IncludeJwtId),
                AlwaysSendClientClaims =
                    doc.GetValueOrDefault("alwaysSendClientClaims", DefaultValues.AlwaysSendClientClaims),
                PrefixClientClaims = doc.GetValueOrDefault("PrefixClientClaims", DefaultValues.PrefixClientClaims)
            };

            client.PostLogoutRedirectUris.AddRange(
                doc.GetValueOrDefault("postLogoutRedirectUris", EmptyStringSet));

            client.RedirectUris.AddRange(doc.GetValueOrDefault("redirectUris", EmptyStringSet));

            client.AllowClientCredentialsOnly = doc.GetValueOrDefault("allowClientCredentialsOnly",
                DefaultValues.AllowClientCredentialsOnly);
            client.UpdateAccessTokenClaimsOnRefresh = doc.GetValueOrDefault("updateAccessTokenClaimsOnRefresh",
                DefaultValues.UpdateAccessTokenClaimsOnRefresh);
            client.AllowedCorsOrigins.AddRange(doc.GetValueOrDefault("allowedCorsOrigins", EmptyStringSet));

            return client;
        }
    }
}