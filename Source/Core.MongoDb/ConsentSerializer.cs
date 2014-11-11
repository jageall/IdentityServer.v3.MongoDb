using System;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Core.MongoDb
{
    public class ConsentSerializer
    {
        private static readonly Guid _namespace = new Guid("344A5569-E318-4A69-9207-C2EBC501D722");

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
            return GuidGenerator.CreateGuidFromName(_namespace, clientId + subject);
        }
    }
}