using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace IdentityServer.Core.MongoDb
{
    class CleanupExpiredTokens : ICleanupExpiredTokens
    {
        private readonly MongoDatabase _db;
        private readonly StoreSettings _settings;

        public CleanupExpiredTokens(MongoDatabase db, StoreSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public void CleanupAuthorizationCodes(DateTime removeTokensBefore)
        {
            var collection = _db.GetCollection(_settings.AuthorizationCodeCollection);
            collection.Remove(Query.LT("_expires", removeTokensBefore));
        }

        public void CleanupTokenHandles(DateTime removeTokensBefore)
        {
            var collection = _db.GetCollection(_settings.TokenHandleCollection);
            collection.Remove(Query.LT("_expires", removeTokensBefore));
        }

        public void CleanupRefreshTokens(DateTime removeTokensBefore)
        {
            var collection = _db.GetCollection(_settings.RefreshTokenCollection);
            collection.Remove(Query.LT("_expires", removeTokensBefore));
        }
    }
}
