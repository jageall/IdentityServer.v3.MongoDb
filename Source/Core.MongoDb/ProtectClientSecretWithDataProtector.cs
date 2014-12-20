using System;
using Thinktecture.IdentityServer.Core.Configuration;

namespace IdentityServer.Core.MongoDb
{
    public class ProtectClientSecretWithDataProtector : IProtectClientSecrets
    {
        private readonly IDataProtector _protector;

        public ProtectClientSecretWithDataProtector(IDataProtector protector)
        {
            _protector = protector;
        }
        public string Protect(string clientId, string clientSecret)
        {
            var decoded = Convert.FromBase64String(clientSecret);
            var bytes = _protector.Protect(decoded, clientId);
            return Convert.ToBase64String(bytes);
        }

        public string Unprotect(string clientId, string clientSecret)
        {

            var decoded = Convert.FromBase64String(clientSecret);
            var bytes = _protector.Unprotect(decoded, clientId);
            return Convert.ToBase64String(bytes);
        }
    }
}