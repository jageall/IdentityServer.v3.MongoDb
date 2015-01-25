using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;
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
        public ClientSecret[] ClientSecrets { get; set; }

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
        public bool? EnableLocalLogin { get; set; }

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
        public AccessTokenType? AccessTokenType { get; set; }

        [Parameter]
        public string[] IdentityProviderRestrictions { get; set; }
        [Parameter]
        public string[] PostLogoutRedirectUris { get; set; }
        [Parameter]
        public string[] RedirectUris { get; set; }
        [Parameter]
        public string[] ScopeRestrictions { get; set; }

        [Parameter]
        public bool? IncludeJwtId { get; set; }
        
        [Parameter]
        public bool? AlwaysSendClientClaims { get; set; }

        [Parameter]
        public bool? PrefixClientClaims { get; set; }
        
        [Parameter]
        public string[] CustomGrantTypeRestrictions { get; set; }

        [Parameter]
        public Claim[] Claims { get; set; }

        protected override void ProcessRecord()
        {
            var client = new Client() { ClientId = ClientId, ClientName = ClientName };
            
            client.AbsoluteRefreshTokenLifetime =
                AbsoluteRefreshTokenLifetime.GetValueOrDefault(client.AbsoluteRefreshTokenLifetime);
            client.AccessTokenLifetime = AccessTokenLifetime.GetValueOrDefault(client.AccessTokenLifetime);
            client.AccessTokenType = AccessTokenType.GetValueOrDefault(client.AccessTokenType);
            client.EnableLocalLogin = EnableLocalLogin.GetValueOrDefault(client.EnableLocalLogin);
            client.AllowRememberConsent = AllowRememberConsent.GetValueOrDefault(client.AllowRememberConsent);
            client.AuthorizationCodeLifetime =
                AuthorizationCodeLifetime.GetValueOrDefault(client.AuthorizationCodeLifetime);

            client.ClientSecrets = (ClientSecrets ?? new ClientSecret[]{}).ToList();
            client.ClientUri = ClientUri;
            client.Enabled = Enabled.GetValueOrDefault(client.Enabled);
            client.Flow = Flow.GetValueOrDefault(client.Flow);
            client.IdentityProviderRestrictions = (IdentityProviderRestrictions ?? client.IdentityProviderRestrictions.ToArray()).ToList();
            client.IdentityTokenLifetime = IdentityTokenLifetime.GetValueOrDefault(client.IdentityTokenLifetime);
            client.LogoUri = LogoUri;
            
            client.PostLogoutRedirectUris.AddRange(PostLogoutRedirectUris ?? new string[] { });
            client.RedirectUris.AddRange(RedirectUris ?? new string[] { });
            client.RefreshTokenExpiration = RefreshTokenExpiration.GetValueOrDefault(client.RefreshTokenExpiration);
            client.RefreshTokenUsage = RefreshTokenUsage.GetValueOrDefault(client.RefreshTokenUsage);
            client.RequireConsent = RequireConsent.GetValueOrDefault(client.RequireConsent);
            client.ScopeRestrictions.AddRange(ScopeRestrictions ?? new string[]{});
            client.SlidingRefreshTokenLifetime =
                SlidingRefreshTokenLifetime.GetValueOrDefault(client.SlidingRefreshTokenLifetime);
            client.IncludeJwtId = IncludeJwtId.GetValueOrDefault(client.IncludeJwtId);
            client.AlwaysSendClientClaims = AlwaysSendClientClaims.GetValueOrDefault(client.AlwaysSendClientClaims);
            client.PrefixClientClaims = PrefixClientClaims.GetValueOrDefault(client.PrefixClientClaims);
            client.Claims.AddRange((Claims ?? new Claim[]{}));
            client.CustomGrantTypeRestrictions.AddRange((CustomGrantTypeRestrictions ?? new string[]{}));
            WriteObject(client);
        }
    }
}