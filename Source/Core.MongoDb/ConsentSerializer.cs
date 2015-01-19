using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    class ConsentSerializer
    {
        private static readonly Guid Namespace = new Guid("344A5569-E318-4A69-9207-C2EBC501D722");
        private static readonly IReadOnlyDictionary<int, Func<BsonDocument, Consent>> Deserializers =
            new Dictionary<int, Func<BsonDocument, Consent>>()
            {
                {1, Version1}
            };
        public BsonDocument Serialize(Consent consent)
        {
            var doc = new BsonDocument();
            doc["_id"] = GetId(consent);
            doc["_version"] = 1;
            doc["clientId"] = consent.ClientId;
            doc["subject"] = consent.Subject;
            doc["scopes"] = new BsonArray(consent.Scopes);
            return doc;
        }

        public Consent Deserialize(BsonDocument doc)
        {
            int version = doc["_version"].AsInt32;
            Func<BsonDocument, Consent> deserializer;
            if (Deserializers.TryGetValue(version, out deserializer))
            {
                return deserializer(doc);
            }
            throw new InvalidOperationException("No deserializers available for consent version " + version);
        }

        private static Consent Version1(BsonDocument doc)
        {
            var consent = new Consent();
            consent.ClientId = doc["clientId"].AsString;
            consent.Subject = doc["subject"].AsString;
            consent.Scopes = doc.GetValueOrDefault("scopes", consent.Scopes);
            return consent;
        }

        public Guid GetId(Consent consent)
        {
            return GetId(consent.ClientId, consent.Subject);
        }

        public Guid GetId(string clientId, string subject)
        {
            return GuidGenerator.CreateGuidFromName(Namespace, clientId + subject);
        }
    }
}