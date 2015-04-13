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
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class RefreshTokenStoreTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private const string NotRemovedKey = "notremoved";
        private const string RemovedKey = "removed";
        private const string SubjectB = "subjectB";
        private const string SubjectA = "subjectA";
        private IRefreshTokenStore _store;
        private IReadOnlyList<RefreshToken> _subjectATokens;
        private IReadOnlyList<RefreshToken> _subjectBTokens;
        
        [Fact]
        public async Task RemovedTokenShouldNotBePresent()
        {
            await _store.RemoveAsync(RemovedKey);
            Assert.Null(await _store.GetAsync(RemovedKey));
        }

        [Fact]
        public async Task NotRemovedKeyShouldBePresent()
        {
            await _store.RemoveAsync(RemovedKey);
            Assert.NotNull(await _store.GetAsync(NotRemovedKey));
        }

        [Fact]
        public async Task GetAllBySubjectShouldReturnExpectedTokens()
        {
            var result = await _store.GetAllAsync(SubjectB);
            Assert.Equal(
                _subjectBTokens
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                    ,
                result
                    .OfType<RefreshToken>()
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                );
        }

        [Fact]
        public async Task RevokedTokensShouldNotBeReturned()
        {
            await _store.RevokeAsync(SubjectA, "Client0");
            var result = await _store.GetAllAsync(SubjectA);
            Assert.Equal(
                _subjectATokens
                    .Where(x=>x.ClientId != "Client0")
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                    ,
                result
                    .OfType<RefreshToken>()
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                );
        }

        [Fact]
        public async Task NonRevokedTokensShouldBeReturned()
        {
            await _store.RevokeAsync(SubjectA, "Client0");
            var result = (await _store.GetAllAsync(SubjectA)).ToArray();
            Assert.Equal(
                _subjectATokens
                    .Where(x => x.ClientId == "Client1")
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                    ,
                result
                    .OfType<RefreshToken>()
                    .OrderBy(LifeTimeOrdering)
                    .Select(TestData.ToTestableString)
                );
        }
        static int LifeTimeOrdering(RefreshToken token)
        {
            return token.LifeTime;
        }

        public RefreshTokenStoreTests(PersistenceTestFixture data)
            : base(data)
        {
            _store = Factory.Resolve<IRefreshTokenStore>();
            _store.StoreAsync(NotRemovedKey, TestData.RefreshToken()).Wait();
            _store.StoreAsync(RemovedKey, TestData.RefreshToken()).Wait();
            var subjectATokens = new List<RefreshToken>();
            var subjectBTokens = new List<RefreshToken>();
            for (int i = 0; i < 10; i++)
            {
                var token = TestData.RefreshToken(SubjectA);
                token.LifeTime += (100 + 100 * i);
                token.AccessToken.Client.ClientId = "Client" + i % 2;
                Save(token.AccessToken.Client);
                _store.StoreAsync(SubjectA + i, token).Wait();
                subjectATokens.Add(token);
            }
            
            for (int i = 0; i < 10; i++)
            {
                var token =  TestData.RefreshToken(SubjectB);
                token.LifeTime += (100 + 100*i);
                token.AccessToken.Client.ClientId = "Client" + i % 2;
                _store.StoreAsync(SubjectB + i, token).Wait();
                subjectBTokens.Add(token);
            }
            _subjectATokens = subjectATokens;
            _subjectBTokens = subjectBTokens;
        }
    }
}
