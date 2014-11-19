using System;
using System.Management.Automation;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Remove, "Tokens")]
    public class DeleteExpiredTokens : MongoCmdlet
    {
        [Parameter]
        public TokenTypes Type { get; set; }

        [Parameter]
        [ValidateNotNull]
        public DateTimeOffset ExpiredBefore { get; set; }

        protected override void ProcessRecord()
        {
            var service = TokenCleanupService;
            ExpiredBefore = ExpiredBefore.ToUniversalTime();
            var expiry = new DateTime(
                ExpiredBefore.Year,
                ExpiredBefore.Month,
                ExpiredBefore.Day,
                ExpiredBefore.Hour,
                ExpiredBefore.Minute,
                ExpiredBefore.Second,
                ExpiredBefore.Millisecond,
                DateTimeKind.Utc);

            if ((Type & TokenTypes.AuthorizationCode) == TokenTypes.AuthorizationCode)
            {
                service.CleanupAuthorizationCodes(expiry);
            }

            if ((Type & TokenTypes.Refresh) == TokenTypes.Refresh)
            {
                service.CleanupRefreshTokens(expiry);
            }

            if ((Type & TokenTypes.Handle) == TokenTypes.Handle)
            {
                service.CleanupTokenHandles(expiry);
            }
        }
    }
}
