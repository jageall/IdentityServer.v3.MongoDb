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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
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
        private readonly Task _setup;

        [Fact]
        public async Task RemovedKeyIsNotFound()
        {
            await _setup;
            await _authorizationStore.RemoveAsync(RemoveKey);
            Assert.Null(await _authorizationStore.GetAsync(RemoveKey));
        }

        [Fact]
        public async Task NotRemovedKeyIsFound()
        {
            await _setup;
            await _authorizationStore.RemoveAsync(RemoveKey);
            Assert.NotNull(await _authorizationStore.GetAsync(NotRemovedKey));
        }

        [Fact]
        public async Task GetAllShouldReturnAllCodes()
        {
            await _setup;
            var result = await _authorizationStore.GetAllAsync(SubjectA);

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
        public async Task RevokedClientsCodesShouldNotBeReturned()
        {
            await _setup;
            await _authorizationStore.RevokeAsync(SubjectB, "revoked");
            var result = await _authorizationStore.GetAllAsync(SubjectB);
            Assert.False(result.Any(x => x.ClientId == "revoked"));
        }

        [Fact]
        public async Task NonRevokedClientsCodesShouldBeReturned()
        {
            await _setup;
            await _authorizationStore.RevokeAsync(SubjectB, "revoked");
            var result = await _authorizationStore.GetAllAsync(SubjectB);

            Assert.Equal(
                _subjectBCodes
                    .Where(x => x.ClientId != "revoked")
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString()),
                result
                    .OfType<AuthorizationCode>()
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString())
                );
        }

        [Fact]
        public async Task RevokingOneSubjectShouldNotEffectTheOther()
        {
            await _setup;
            await _authorizationStore.RevokeAsync(SubjectB, "revoked");
            var result = await _authorizationStore.GetAllAsync(SubjectC);

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
            _serializer = new JsonSerializer {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            _authorizationStore = Factory.Resolve<IAuthorizationCodeStore>();


            var subjectACodes = new List<AuthorizationCode>();
            var subjectBCodes = new List<AuthorizationCode>();
            var subjectCCodes = new List<AuthorizationCode>();


            _subjectACodes = subjectACodes;
            _subjectBCodes = subjectBCodes;
            _subjectCCodes = subjectCCodes;

            _setup = Setup(subjectACodes, subjectBCodes, subjectCCodes);
        }

        private async Task Setup(List<AuthorizationCode> subjectACodes, List<AuthorizationCode> subjectBCodes, List<AuthorizationCode> subjectCCodes)
        {
            List<Task> tasks = new List<Task>(); 
            tasks.Add(_authorizationStore.StoreAsync(RemoveKey, TestData.AuthorizationCode()));
            tasks.Add(_authorizationStore.StoreAsync(NotRemovedKey, TestData.AuthorizationCode()));
            foreach (var scope in TestData.Scopes())
            {
                tasks.Add(SaveAsync(scope));
            }
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectA);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                tasks.Add(_authorizationStore.StoreAsync("notRevokedA" + i, code));
                subjectACodes.Add(code);
                tasks.Add(SaveAsync(code.Client));
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectB);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                tasks.Add(_authorizationStore.StoreAsync("notRevokedB" + i, code));
                subjectBCodes.Add(code);

                tasks.Add(SaveAsync(code.Client));
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectB);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                tasks.Add(_authorizationStore.StoreAsync("revokedB" + i, code));
                subjectBCodes.Add(code);

                tasks.Add(SaveAsync(code.Client));
            }
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectC);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                tasks.Add(_authorizationStore.StoreAsync("notRevokedC" + i, code));
                subjectCCodes.Add(code);

                tasks.Add(SaveAsync(code.Client));
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(SubjectC);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                tasks.Add(_authorizationStore.StoreAsync("revokedC" + i, code));
                subjectCCodes.Add(code);
                tasks.Add(SaveAsync(code.Client));
            }

            await Task.WhenAll(tasks);
        }
}
}
