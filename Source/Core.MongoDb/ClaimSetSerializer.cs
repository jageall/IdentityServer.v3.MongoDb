using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MongoDB.Bson;

namespace IdentityServer.Core.MongoDb
{
    internal class ClaimSetSerializer
    {
        static readonly IReadOnlyDictionary<int, Func<BsonDocument, IEnumerable<Claim>>> Deserializers
            = new Dictionary<int, Func<BsonDocument, IEnumerable<Claim>>>
            {
                {1, Version1}
            };
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
            int version = doc["_version"].AsInt32;
            Func<BsonDocument, IEnumerable<Claim>> deserializer;
            if (Deserializers.TryGetValue(version, out deserializer))
            {
                return deserializer(doc);
            }
            throw new InvalidOperationException("No deserializers available for claimset version " + version);
        }

        private static IEnumerable<Claim> Version1(BsonDocument doc)
        {
            return doc.GetValueOrDefault(
                    "claims",
                    c => new Claim(c["type"].AsString, c["value"].AsString),
                    new Claim[] { }).ToList();
        }
    }
}