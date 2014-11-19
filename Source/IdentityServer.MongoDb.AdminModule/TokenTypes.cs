using System;

namespace IdentityServer.MongoDb.AdminModule
{
    [Flags]
    public enum TokenTypes
    {
        AuthorizationCode = 0x1,
        Refresh = 0x2,
        Handle = 0x4,
        All = AuthorizationCode | Refresh | Handle
    }
}