using System;
using System.Management.Automation;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Remove, "Tokens")]
    public class DeleteExpiredTokens : MongoCmdlet
    {
        [Parameter]
        public TokenTypes Types { get; set; }

        [Parameter]
        [ValidateNotNull]
        public DateTimeOffset? ExpiredBefore { get; set; }

        protected override void ProcessRecord()
        {
            var service = TokenCleanupService;
            var expiredBefore = (ExpiredBefore ?? DateTimeOffset.UtcNow).ToUniversalTime();
            var expiry = new DateTime(
                expiredBefore.Year,
                expiredBefore.Month,
                expiredBefore.Day,
                expiredBefore.Hour,
                expiredBefore.Minute,
                expiredBefore.Second,
                expiredBefore.Millisecond,
                DateTimeKind.Utc);

            if ((Types & TokenTypes.AuthorizationCode) == TokenTypes.AuthorizationCode)
            {
                service.CleanupAuthorizationCodes(expiry);
            }

            if ((Types & TokenTypes.Refresh) == TokenTypes.Refresh)
            {
                service.CleanupRefreshTokens(expiry);
            }

            if ((Types & TokenTypes.Handle) == TokenTypes.Handle)
            {
                service.CleanupTokenHandles(expiry);
            }
        }
    }
}
