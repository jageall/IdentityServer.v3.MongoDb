/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using IdentityServer3.Core.Models;
using MongoDB.Bson;

namespace IdentityServer3.MongoDb
{
    class ConsentSerializer
    {
        private static readonly Guid Namespace = new Guid("344A5569-E318-4A69-9207-C2EBC501D722");
        private static readonly IReadOnlyDictionary<int, Func<BsonDocument, Consent>> Deserializers =
            new Dictionary<int, Func<BsonDocument, Consent>>
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
            var consent = new Consent
            {
                ClientId = doc["clientId"].AsString, 
                Subject = doc["subject"].AsString
            };
            consent.Scopes = doc.GetValueOrDefault("scopes", consent.Scopes);
            return consent;
        }

        private static Guid GetId(Consent consent)
        {
            return GetId(consent.ClientId, consent.Subject);
        }

        public static Guid GetId(string clientId, string subject)
        {
            return GuidGenerator.CreateGuidFromName(Namespace, clientId + subject);
        }
    }
}