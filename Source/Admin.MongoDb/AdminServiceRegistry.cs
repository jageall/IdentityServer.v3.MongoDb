using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;

namespace IdentityServer.Admin.MongoDb
{
    public class AdminServiceRegistry
    {
        public AdminServiceRegistry()
        {
            AdminService = new Registration<IAdminService>(typeof(AdminService));
            TokenCleanupService =
                new Registration<ICleanupExpiredTokens>(typeof(CleanupExpiredTokens));
   
        }
        public Registration<IAdminService> AdminService { get; set; }

        public Registration<ICleanupExpiredTokens> TokenCleanupService { get; set; }
    }
}
