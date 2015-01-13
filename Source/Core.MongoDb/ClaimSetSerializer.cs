using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MongoDB.Bson;

namespace IdentityServer.Core.MongoDb
{
    internal class ClaimSetSerializer
    {
        public BsonDocument Serialize(IEnumerable<Claim> claims)
        {
            var result = new BsonDocument();
            Serialize(claims, result);
            return result;
        }

        public void Serialize(IEnumerable<Claim> claims, BsonDocument doc)
        {
            doc["_version"] = 1;
            var array = new BsonArray();
            foreach (Claim claim in claims)
            {
                var c = new BsonDocument();
                c["type"] = claim.Type;
                c["value"] = claim.Value;
                array.Add(c);
            }
            doc["claims"] = array;
        }

        public IEnumerable<Claim> Deserialize(BsonDocument doc)
        {
            return doc.GetValueOrDefault(
                    "claims",
                    c => new Claim(c["type"].AsString, c["value"].AsString),
                    new Claim[] { }).ToList();
        }
    }
}