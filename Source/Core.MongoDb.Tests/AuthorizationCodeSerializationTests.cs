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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeSerializationTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private AuthorizationCode _expected;
        private AuthorizationCode _actual;
        private AuthorizationCode _actualNoNonce;
        private Task _setup;


        [Fact]
        public async Task CheckNonce()
        {

            await _setup;
           Assert.Null(_actualNoNonce.Nonce);
            Assert.Equal(_expected.Nonce ,_actual.Nonce);
        }

        [Fact]
        public async Task CheckAll()
        {
            await _setup;
            var serializer = new JsonSerializer {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
        }

        public AuthorizationCodeSerializationTests(PersistenceTestFixture data) : base(data)
        {
            var key = "AuthorizationCodeTests";
            _expected = TestData.AuthorizationCode();
            
            _setup = Setup(key);
        }

        private async Task Setup(string key)
        {
            await SaveAsync(_expected.Client);
            foreach (var scope in _expected.RequestedScopes)
            {
                await SaveAsync(scope);
            }
            var store = Factory.Resolve<IAuthorizationCodeStore>();
            await store.StoreAsync(key, TestData.AuthorizationCode());
            await store.StoreAsync(key + "NoNonce", TestData.AuthorizationCodeWithoutNonce());
            _actual = await store.GetAsync(key);
            _actualNoNonce = await store.GetAsync(key + "NoNonce");
        }
    }
}