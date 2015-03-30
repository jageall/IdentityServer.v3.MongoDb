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
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using Newtonsoft.Json.Linq;
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
        public void RemovedTokenShouldNotBePresent()
        {
            _store.RemoveAsync(RemovedKey).Wait();
            Assert.Null(_store.GetAsync(RemovedKey).Result);
        }

        [Fact]
        public void NotRemovedKeyShouldBePresent()
        {
            _store.RemoveAsync(RemovedKey).Wait();
            Assert.NotNull(_store.GetAsync(NotRemovedKey).Result);
        }

        [Fact]
        public void GetAllBySubjectShouldReturnExpectedTokens()
        {
            var result = _store.GetAllAsync(SubjectB).Result.ToArray();
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
        public void RevokedTokensShouldNotBeReturned()
        {
            _store.RevokeAsync(SubjectA, "Client0").Wait();
            var result = _store.GetAllAsync(SubjectA).Result.ToArray();
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
        public void NonRevokedTokensShouldBeReturned()
        {
            _store.RevokeAsync(SubjectA, "Client0").Wait();
            var result = _store.GetAllAsync(SubjectA).Result.ToArray();
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
            _store.StoreAsync(NotRemovedKey, TestData.RefreshToken());
            _store.StoreAsync(RemovedKey, TestData.RefreshToken());
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
