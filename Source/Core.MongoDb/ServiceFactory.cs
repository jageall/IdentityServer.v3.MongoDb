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

using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer3.MongoDb
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService)
            : this(userService, StoreSettings.DefaultSettings())
        {
        }

        public ServiceFactory(Registration<IUserService> userService,
            StoreSettings storeSettings)
        {
            var client = new MongoClient(MongoClientSettings(storeSettings.ConnectionString));
            IMongoDatabase db = client.GetDatabase(storeSettings.Database);
            Register(new Registration<IMongoDatabase>(db));
            Register(new Registration<StoreSettings>(storeSettings));
            UserService = userService;
            ClientStore = new Registration<IClientStore>(typeof(ClientStore));
            ScopeStore = new Registration<IScopeStore>(typeof(ScopeStore));
            ConsentStore = new Registration<IConsentStore>(typeof (ConsentStore));
            
            AuthorizationCodeStore = new Registration<IAuthorizationCodeStore>(typeof(AuthorizationCodeStore));

            RefreshTokenStore = new Registration<IRefreshTokenStore>(typeof (RefreshTokenStore));
            TokenHandleStore = new Registration<ITokenHandleStore>(typeof (TokenHandleStore));
            Register(new Registration<ClientSerializer>(typeof(ClientSerializer)));
        }


        private static MongoClientSettings MongoClientSettings(string mongoUrl)
        {
            MongoClientSettings settings = MongoDB.Driver.MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
            settings.GuidRepresentation = GuidRepresentation.Standard;
            return settings;
        }
    }
}