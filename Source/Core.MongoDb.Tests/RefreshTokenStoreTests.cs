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
    public class RefreshTokenStoreTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
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

        protected override void Initialize()
        {
            _store = Factory.Resolve<IRefreshTokenStore>();
            var admin = Factory.Resolve<IAdminService>();
            _store.StoreAsync(NotRemovedKey, TestData.RefreshToken());
            _store.StoreAsync(RemovedKey, TestData.RefreshToken());
            var subjectATokens = new List<RefreshToken>();
            var subjectBTokens = new List<RefreshToken>();
            for (int i = 0; i < 10; i++)
            {
                var token = TestData.RefreshToken(SubjectA);
                token.LifeTime += (100 + 100 * i);
                token.AccessToken.Client.ClientId = "Client" + i % 2;
                admin.Save(token.AccessToken.Client);
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
