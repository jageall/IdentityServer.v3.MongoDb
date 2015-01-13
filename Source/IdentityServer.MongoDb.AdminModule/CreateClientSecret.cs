using System;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.New, "ClientSecret")]
    public class CreateClientSecret : PSCmdlet
    {
        [Parameter, ValidateNotNullOrEmpty]
        public string Value { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public DateTimeOffset? Expiration { get; set; }
        [Parameter]
        public HashType? Hash { get; set; }
        protected override void ProcessRecord()
        {
            if (Hash != null)
            {
                var hash = Hash == HashType.SHA256 ? (HashAlgorithm) SHA256.Create() : SHA512.Create();
                var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(Value));
                Value = Convert.ToBase64String(bytes);
            }
            WriteObject(new ClientSecret(Value, Description, Expiration));
        }
    }
}