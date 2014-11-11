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
                IdentityProviderRestrictions = new[] {"idpr"},
                IdentityTokenLifetime = 40,
                IdentityTokenSigningKeyType = SigningKeyTypes.ClientSecret,
                LogoUri = new Uri("uri:logo"),
                PostLogoutRedirectUris = {new Uri("uri:logout")},
                RedirectUris = {new Uri("uri:redirect")},
                RefreshTokenExpiration = TokenExpiration.Sliding,
                RefreshTokenUsage = TokenUsage.ReUse,
                RequireConsent = true,
                ScopeRestrictions = {"restriction"},
                SlidingRefreshTokenLifetime = 50
            };
        }

        public static Scope ScopeAllProperties()
        {
            return new Scope
            {
                Name = "name",
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
                Name = "name",
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
                    new[]
                    {
                        new Claim("sub", "foo"),
                        new Claim("name", "bar"),
                        new Claim("email", "baz@qux.com")
                    }, "authtype"
                    ));
        }
    }
}