using System;

namespace IdentityServer.Core.MongoDb
{
    public interface ICleanupExpiredTokens
    {
        void CleanupAuthorizationCodes(DateTime removeTokensBefore);
        void CleanupTokenHandles(DateTime removeTokensBefore);
        void CleanupRefreshTokens(DateTime removeTokensBefore);
    }
}