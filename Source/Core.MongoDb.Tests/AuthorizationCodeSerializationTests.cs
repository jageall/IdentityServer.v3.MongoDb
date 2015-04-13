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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeSerializationTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private AuthorizationCode _expected;
        private AuthorizationCode _actual;
        private AuthorizationCode _actualNoNonce;

        [Fact]
        public void CheckCreationTime()
        {
            Assert.Equal(_expected.CreationTime, _actual.CreationTime);
        }

        [Fact]
        public void CheckIsOpenId()
        {
            Assert.Equal(_expected.IsOpenId, _actual.IsOpenId);
        }

        [Fact]
        public void CheckRedirectUri()
        {
            Assert.Equal(_expected.RedirectUri, _actual.RedirectUri);
        }

        [Fact]
        public void CheckConsentWasShown()
        {
            Assert.Equal(_expected.WasConsentShown, _actual.WasConsentShown);
        }

        [Fact]
        public void CheckSubject()
        {
            Assert.NotNull(_actual.Subject);
            Assert.Equal(_expected.Subject.Identities.Count(), _actual.Subject.Identities.Count());
            foreach (var identity in _expected.Subject.Identities)
            {
                var actual =
                    _actual.Subject.Identities.SingleOrDefault(
                        x => string.Equals(x.AuthenticationType, identity.AuthenticationType, StringComparison.Ordinal));
                Assert.NotNull(actual);
                CommonVerifications.VerifyClaimset(identity.Claims, actual.Claims);
            }
        }

        [Fact]
        public void CheckScopes()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
        }


        [Fact]
        public void CheckNonce()
        {
           Assert.Null(_actualNoNonce.Nonce);
            Assert.Equal(_expected.Nonce ,_actual.Nonce);
        }

        [Fact]
        public void CheckClient()
        {
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void CheckAll()
        {
            var serializer = new JsonSerializer {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
        }

        public AuthorizationCodeSerializationTests(PersistenceTestFixture data) : base(data)
        {
            var key = "AuthorizationCodeTests";
            _expected = TestData.AuthorizationCode();
            
            Save(_expected.Client);
            foreach (var scope in _expected.RequestedScopes)
            {
                Save(scope);
            }
            var store = Factory.Resolve<IAuthorizationCodeStore>();
            store.StoreAsync(key, TestData.AuthorizationCode()).Wait();
            store.StoreAsync(key + "NoNonce", TestData.AuthorizationCodeWithoutNonce()).Wait();
            _actual = store.GetAsync(key).Result;
            _actualNoNonce = store.GetAsync(key + "NoNonce").Result;
        }
    }
}