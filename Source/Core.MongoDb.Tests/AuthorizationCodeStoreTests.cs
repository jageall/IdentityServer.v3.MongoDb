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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeStoreTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private readonly IAuthorizationCodeStore _authorizationStore;
        private const string RemoveKey = "remove";
        private const string NotRemovedKey = "notRemoved";
        private const string SubjectA = "subjectA";
        private const string SubjectB = "subjectB";
        private const string SubjectC = "subjectC";
        private readonly IReadOnlyList<AuthorizationCode> _subjectACodes;
        private readonly IReadOnlyList<AuthorizationCode> _subjectBCodes;
        private readonly IReadOnlyList<AuthorizationCode> _subjectCCodes;
        private readonly JsonSerializer _serializer;

        [Fact]
        public void RemovedKeyIsNotFound()
        {
            _authorizationStore.RemoveAsync(RemoveKey).Wait();
            Assert.Null(_authorizationStore.GetAsync(RemoveKey).Result);
        }

        [Fact]
        public void NotRemovedKeyIsFound()
        {
            _authorizationStore.RemoveAsync(RemoveKey).Wait();
            Assert.NotNull(_authorizationStore.GetAsync(NotRemovedKey).Result);
        }

        [Fact]
        public void GetAllShouldReturnAllCodes()
        {
            var result = _authorizationStore.GetAllAsync(SubjectA).Result.ToArray();

            var actual = result.OfType<AuthorizationCode>()
                .OrderBy(x => x.Nonce)
                .Select(x => JObject.FromObject(x, _serializer).ToString()).ToArray();
            var expected = _subjectACodes.OrderBy(x => x.Nonce)
                .Select(x => JObject.FromObject(x, _serializer).ToString()).ToArray();
            
                Assert.Equal(
                    expected,
                    actual);
        }

        [Fact]
        public void RevokedClientsCodesShouldNotBeReturned()
        {
            _authorizationStore.RevokeAsync(SubjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(SubjectB).Result.ToArray();
            Assert.False(result.Any(x=>x.ClientId == "revoked"));
        }

        [Fact]
        public void NonRevokedClientsCodesShouldNotBeReturned()
        {
            _authorizationStore.RevokeAsync(SubjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(SubjectB).Result.ToArray();

            Assert.Equal(
                _subjectBCodes
                    .Where(x=>x.ClientId != "revoked")
                    .OrderBy(x=>x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString()),
                result
                    .OfType<AuthorizationCode>()
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString())
                    );
        }

        [Fact]
        public void RevokingOneSubjectShouldNotEffectTheOther()
        {
            _authorizationStore.RevokeAsync(SubjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(SubjectC).Result.ToArray();

            Assert.Equal(
                _subjectCCodes
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString()),
                result
                    .OfType<AuthorizationCode>()
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString())
                    );
        }
        public AuthorizationCodeStoreTests(PersistenceTestFixture data)
            : base(data)
        {
            _serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            _authorizationStore = Factory.Resolve<IAuthorizationCodeStore>();
            _authorizationStore.StoreAsync(RemoveKey, TestData.AuthorizationCode());
            _authorizationStore.StoreAsync(NotRemovedKey, TestData.AuthorizationCode());
            var subjectACodes = new List<AuthorizationCode>();
            var subjectBCodes = new List<AuthorizationCode>();
            var subjectCCodes = new List<AuthorizationCode>();
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectA);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedA" + i, code);
                subjectACodes.Add(code);
                Save(code.Client);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectB);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedB" + i, code);
                subjectBCodes.Add(code);

                Save(code.Client);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectB);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                _authorizationStore.StoreAsync("revokedB" + i, code);
                subjectBCodes.Add(code);

                Save(code.Client);
            }
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectC);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedC" + i, code);
                subjectCCodes.Add(code);

                Save(code.Client);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectC);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                _authorizationStore.StoreAsync("revokedC" + i, code);
                subjectCCodes.Add(code);
            }
            _subjectACodes = subjectACodes;
            _subjectBCodes = subjectBCodes;
            _subjectCCodes = subjectCCodes;
        }
    }
}
