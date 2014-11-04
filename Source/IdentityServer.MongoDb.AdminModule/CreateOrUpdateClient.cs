using System;
using System.ComponentModel.DataAnnotations;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Set, "Client")]
    public class CreateOrUpdateClient : MongoCmdlet
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

        protected override void ProcessRecord()
        {
            var db = OpenDatabase();

            var client = new Client(){ClientId = ClientId, ClientName = ClientName};
            client.AbsoluteRefreshTokenLifetime =
                AbsoluteRefreshTokenLifetime.GetValueOrDefault(client.AbsoluteRefreshTokenLifetime);
            
            var clients = db.GetCollection(ClientCollection);
            var serializer = new ClientSerializer();
            var doc = serializer.Serialize(client);
            clients.Save(doc);
        }

    }
}