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
namespace IdentityServer3.MongoDb
{
    public class StoreSettings
    {
        public string Database { get; set; }
        public string ClientCollection { get; set; }
        public string ScopeCollection { get; set; }
        public string ConsentCollection { get; set; }
        public string AuthorizationCodeCollection { get; set; }
        public string ConnectionString { get; set; }
        public string RefreshTokenCollection { get; set; }
        public string TokenHandleCollection { get; set; }

        public static StoreSettings DefaultSettings()
        {
            return new StoreSettings
            {
                ConnectionString = "mongodb://localhost",
                Database = "identityserver",
                ClientCollection = "clients",
                ScopeCollection = "scopes",
                ConsentCollection = "consents",
                AuthorizationCodeCollection = "authorizationCodes",
                RefreshTokenCollection = "refreshtokens",
                TokenHandleCollection = "tokenhandles"
            };
        }
    }
}