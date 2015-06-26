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

using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using MongoDB.Bson;

namespace IdentityServer3.MongoDb
{
    class RefreshTokenSerializer
    {
        private readonly TokenSerializer _tokenSerializer;

        public RefreshTokenSerializer(IClientStore clientStore)
        {
            _tokenSerializer = new TokenSerializer(clientStore);
        }
        public BsonDocument Serialize(string key, RefreshToken value)
        {
            var doc = new BsonDocument();
            doc["_id"] = key;
            doc["_version"] = 1;
            doc["_expires"] = value.CreationTime.AddSeconds(value.LifeTime).ToBsonDateTime();
            doc["_clientId"] = value.ClientId;
            doc["_subjectId"] = value.SubjectId;
            var accessToken = new BsonDocument();
            _tokenSerializer.Serialize(accessToken ,value.AccessToken);
            doc["accessToken"] = accessToken;
            doc["creationTime"] = value.CreationTime.ToBsonDateTime();
            doc["lifetime"] = value.LifeTime;
            doc["version"] = value.Version;
            return doc;
        }

        public async Task<RefreshToken> Deserialize(BsonDocument doc)
        {
            var token = new RefreshToken();
            BsonValue at;
            if (doc.TryGetValue("accessToken", out at))
            {

               token.AccessToken = await _tokenSerializer.Deserialize(at.AsBsonDocument);
            }
            token.CreationTime = doc.GetValueOrDefault("creationTime", token.CreationTime);
            token.LifeTime = doc.GetValueOrDefault("lifetime", token.LifeTime);
            token.Version = doc.GetValueOrDefault("version", token.Version);
            return token;
        }
    }
}