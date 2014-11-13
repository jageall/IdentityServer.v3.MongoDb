using System;
using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.IdentityServer.Core.Models;

namespace Core.MongoDb.Tests
{
    public static class TestData
    {
        public static Client ClientAllProperties()
        {
            return new Client
            {
                AbsoluteRefreshTokenLifetime = 10,
                AccessTokenLifetime = 20,
                AccessTokenType = AccessTokenType.Reference,
                AllowLocalLogin = false,
                AllowRememberConsent = true,
                AuthorizationCodeLifetime = 30,
                ClientId = "123",
                ClientName = "TEST",
                ClientSecret = "secret",
                ClientUri = "clientUri",
                Enabled = true,
                Flow = Flows.AuthorizationCode,
                IdentityProviderRestrictions = new[] { "idpr" },
                IdentityTokenLifetime = 40,
                IdentityTokenSigningKeyType = SigningKeyTypes.ClientSecret,
                LogoUri = new Uri("uri:logo"),
                PostLogoutRedirectUris = { new Uri("uri:logout") },
                RedirectUris = { new Uri("uri:redirect") },
                RefreshTokenExpiration = TokenExpiration.Sliding,
                RefreshTokenUsage = TokenUsage.ReUse,
                RequireConsent = true,
                ScopeRestrictions = { "restriction" },
                SlidingRefreshTokenLifetime = 50
            };
        }

        public static Scope ScopeAllProperties()
        {
            return new Scope
            {
                Name = "all",
                DisplayName = "displayName",
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim
                    {
                        Name = "claim1",
                        AlwaysIncludeInIdToken = false,
                        Description = "claim1 description"
                    },
                    new ScopeClaim
                    {
                        Name = "claim2",
                        AlwaysIncludeInIdToken = true,
                        Description = "claim2 description"
                    },
                },
                ClaimsRule = "claimsRule",
                Description = "Description",
                Emphasize = true,
                Enabled = false,
                IncludeAllClaimsForUser = true,
                Required = true,
                ShowInDiscoveryDocument = false,
                Type = ScopeType.Identity
            };
        }

        public static Scope ScopeMandatoryProperties()
        {
            return new Scope
            {
                Name = "mandatory",
                DisplayName = "displayName"
            };
        }

        public static AuthorizationCode AuthorizationCode()
        {
            return new AuthorizationCode
            {
                IsOpenId = true,
                CreationTime = new DateTime(2000, 1, 1, 1, 1, 1, 0),
                Client = Client(),
                RedirectUri = new Uri("uri:redirect"),
                RequestedScopes = Scopes(),
                Subject = Subject(),
                WasConsentShown = true
            };
        }

        private static Client Client()
        {
            return ClientAllProperties();
        }

        private static IEnumerable<Scope> Scopes()
        {
            yield return ScopeAllProperties();
            yield return ScopeMandatoryProperties();
        }

        private static ClaimsPrincipal Subject()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    Claims(), "authtype"
                    ));
        }

        private static List<Claim> Claims()
        {
            return new List<Claim>
            {
                new Claim("sub", "foo"),
                new Claim("name", "bar"),
                new Claim("email", "baz@qux.com"),
                new Claim("scope", "scope1"),
                new Claim("scope", "scope2"),
            };
        }

        public static RefreshToken RefreshToken()
        {
            return new RefreshToken()
            {
                AccessToken = Token(),
                ClientId = "clientId",
                CreationTime = new DateTime(2000, 1, 1, 1, 1, 1, 0),
                LifeTime = 100,
            };
        }

        public static Token Token()
        {
            return new Token
            {
                Audience = "audience",
                Claims = Claims(),
                Client = ClientAllProperties(),
                CreationTime = new DateTime(2000, 1, 1, 1, 1, 1, 0),
                Issuer = "issuer",
                Lifetime = 200,
                Type = "tokenType"
            };
        }
    }
}