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
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class TokenHandleStoreTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private ITokenHandleStore _store;
        private IReadOnlyList<Token> _subjectATokens;
        private IReadOnlyList<Token> _subjectBTokens;
        private IReadOnlyList<Token> _subjectCTokens;
        private Task _setup;
        private const string RemovedKey = "remove";
        private const string NotRemovedKey = "donotremove";
        private const string SubjectA = "SubjectA";
        private const string SubjectB = "SubjectB";
        private const string SubjectC = "SubjectC";
        private const string RevokedClient = "Revoked";
        private const string NotRevokedClient = "NotRevoked";

        [Fact]
        public async Task NotRemovedTokenIsReturned()
        {
            await _setup;
            await _store.RemoveAsync(RemovedKey);
            var result = (await _store.GetAllAsync(SubjectA)).ToArray();
            Assert.Equal(1, result.Length);
            Assert.Equal(
                TestData.ToTestableString(_subjectATokens[1]),
                TestData.ToTestableString(result[0]));
        }

        [Fact]
        public async Task RemovedTokenIsNotReturned()
        {
            await _setup;
            await _store.RemoveAsync(RemovedKey);
            var result = await _store.GetAllAsync(SubjectA);
            Assert.False(result.Any(r => r.ClientId == _subjectATokens[0].ClientId));      
        }

        [Fact]
        public async Task NonRevokedTokensAreReturned()
        {
            await _setup;
            await _store.RevokeAsync(SubjectB, RevokedClient);
            var results = await _store.GetAllAsync(SubjectB);
            Assert.Equal(
                _subjectBTokens
                    .Where(x=>x.ClientId != RevokedClient)
                    .OrderBy(CreationTime)
                    .Select(TestData.ToTestableString),
                results
                    .OfType<Token>()
                    .OrderBy(CreationTime)
                    .Select(TestData.ToTestableString)
                    );
        }

        [Fact]
        public async Task RevokedTokensAreNotReturned()
        {
            await _setup;
            await _store.RevokeAsync(SubjectB, RevokedClient);
            var results = await _store.GetAllAsync(SubjectB);
            Assert.False(results.Any(x=>x.ClientId == RevokedClient));
        }

        [Fact]
        public async Task RevokingShouldNotEffectOtherSubjects()
        {
            await _setup;
            await _store.RevokeAsync(SubjectB, RevokedClient);
            var results = await _store.GetAllAsync(SubjectC);
            Assert.Equal(
                _subjectCTokens
                    .OrderBy(CreationTime)
                    .Select(TestData.ToTestableString).First(),
                results
                    .OfType<Token>()
                    .OrderBy(CreationTime)
                    .Select(TestData.ToTestableString).First()
                    );   
        }

        [Fact]
        public async Task LoadingNonExistingKeyShouldResultInNull()
        {
            await _setup;
            Assert.Null(await _store.GetAsync("DoesNotExist"));
        }

        static DateTimeOffset CreationTime (Token token)
        {
            return token.CreationTime;
        }

        public TokenHandleStoreTests(PersistenceTestFixture data)
            : base(data)
        {
            _store = Factory.Resolve<ITokenHandleStore>();


            var subjectATokens = new List<Token>();
            var subjectBTokens = new List<Token>();
            var subjectCTokens = new List<Token>();

            _setup = Setup(subjectATokens, subjectBTokens, subjectCTokens);
            _subjectATokens = subjectATokens;
            _subjectBTokens = subjectBTokens;
            _subjectCTokens = subjectCTokens;
        }

        private Task Setup(List<Token> subjectATokens, List<Token> subjectBTokens, List<Token> subjectCTokens)
        {
            List<Task> tasks = new List<Task>();
            var removed = TestData.Token(SubjectA);
            removed.Client.ClientId = removed.ClientId + 0;
            tasks.Add(SaveAsync(removed.Client));

            tasks.Add(_store.StoreAsync(RemovedKey, removed));
            var notRemoved = TestData.Token(SubjectA);
            notRemoved.Client.ClientId = notRemoved.ClientId + 1;
            tasks.Add(SaveAsync(notRemoved.Client));
            tasks.Add(_store.StoreAsync(NotRemovedKey, notRemoved));

            subjectATokens.Add(removed);
            subjectATokens.Add(notRemoved);

            foreach (var subjectConfig in new[]
            {
                new {subject = SubjectB, tokens = subjectBTokens},
                new {subject = SubjectC, tokens = subjectCTokens}
            })
                for (int i = 0; i < 10; i++)
                {
                    var token = TestData.Token(subjectConfig.subject);
                    token.CreationTime = token.CreationTime.AddDays(i);
                    if (i%2 == 0)
                    {
                        token.Client.ClientId = RevokedClient;
                    }
                    else
                    {
                        token.Client.ClientId = NotRevokedClient;
                    }
                    tasks.Add(SaveAsync(token.Client));
                    tasks.Add(_store.StoreAsync(subjectConfig.subject + i, token));
                    subjectConfig.tokens.Add(token);
                }

            return Task.WhenAll(tasks);
        }
    }
}
