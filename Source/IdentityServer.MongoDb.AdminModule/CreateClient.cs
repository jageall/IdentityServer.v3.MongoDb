using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Management.Automation;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.New, "Client")]
    public class CreateClient : PSCmdlet
    {
        [Parameter]
        public bool? Enabled { get; set; }
        [Parameter(Mandatory = true)]
        public string ClientId { get; set; }
        [Parameter]
        public string ClientSecret { get; set; }
        [Parameter(Mandatory = true)]
        public string ClientName { get; set; }
        [Parameter]
        public string ClientUri { get; set; }
        [Parameter]
        public string LogoUri { get; set; }

        [Parameter]
        public bool? RequireConsent { get; set; }
        [Parameter]
        public bool? AllowRememberConsent { get; set; }
        [Parameter]
        public bool? AllowLocalLogin { get; set; }

        [Parameter]
        public Flows? Flow { get; set; }

        // in seconds
        [Range(0, Int32.MaxValue)]
        [Parameter]
        public int? IdentityTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        [Parameter]
        public int? AccessTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        [Parameter]
        public int? AuthorizationCodeLifetime { get; set; }

        [Range(0, Int32.MaxValue)]
        [Parameter]
        public int? AbsoluteRefreshTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        [Parameter]
        public int? SlidingRefreshTokenLifetime { get; set; }
        [Parameter]
        public TokenUsage? RefreshTokenUsage { get; set; }
        [Parameter]
        public TokenExpiration? RefreshTokenExpiration { get; set; }

        [Parameter]
        public SigningKeyTypes? IdentityTokenSigningKeyType { get; set; }
        [Parameter]
        public AccessTokenType? AccessTokenType { get; set; }

        [Parameter]
        public string[] IdentityProviderRestrictions { get; set; }
        [Parameter]
        public string[] PostLogoutRedirectUris { get; set; }
        [Parameter]
        public string[] RedirectUris { get; set; }
        [Parameter]
        public string[] ScopeRestrictions { get; set; }

        protected override void ProcessRecord()
        {
            var client = new Client() { ClientId = ClientId, ClientName = ClientName };
            client.AbsoluteRefreshTokenLifetime =
                AbsoluteRefreshTokenLifetime.GetValueOrDefault(client.AbsoluteRefreshTokenLifetime);
            client.AccessTokenLifetime = AccessTokenLifetime.GetValueOrDefault(client.AccessTokenLifetime);
            client.AccessTokenType = AccessTokenType.GetValueOrDefault(client.AccessTokenType);
            client.AllowLocalLogin = AllowLocalLogin.GetValueOrDefault(client.AllowLocalLogin);
            client.AllowRememberConsent = AllowRememberConsent.GetValueOrDefault(client.AllowRememberConsent);
            client.AuthorizationCodeLifetime =
                AuthorizationCodeLifetime.GetValueOrDefault(client.AuthorizationCodeLifetime);
            client.ClientSecret = ClientSecret;
            client.ClientUri = ClientUri;
            client.Enabled = Enabled.GetValueOrDefault(client.Enabled);
            client.Flow = Flow.GetValueOrDefault(client.Flow);
            client.IdentityProviderRestrictions = IdentityProviderRestrictions ?? client.IdentityProviderRestrictions;
            client.IdentityTokenLifetime = IdentityTokenLifetime.GetValueOrDefault(client.IdentityTokenLifetime);
            client.IdentityTokenSigningKeyType =
                IdentityTokenSigningKeyType.GetValueOrDefault(client.IdentityTokenSigningKeyType);
            client.LogoUri = string.IsNullOrEmpty(LogoUri) ? null : new Uri(LogoUri);
            
            client.PostLogoutRedirectUris.AddRange((PostLogoutRedirectUris ?? new string[] { }).Select(x => new Uri(x)));
            client.RedirectUris.AddRange((RedirectUris ?? new string[] { }).Select(x=> new Uri(x)));
            client.RefreshTokenExpiration = RefreshTokenExpiration.GetValueOrDefault(client.RefreshTokenExpiration);
            client.RefreshTokenUsage = RefreshTokenUsage.GetValueOrDefault(client.RefreshTokenUsage);
            client.RequireConsent = RequireConsent.GetValueOrDefault(client.RequireConsent);
            client.ScopeRestrictions.AddRange(ScopeRestrictions ?? new string[]{});
            client.SlidingRefreshTokenLifetime =
                SlidingRefreshTokenLifetime.GetValueOrDefault(client.SlidingRefreshTokenLifetime);
            WriteObject(client);
        }
    }
}