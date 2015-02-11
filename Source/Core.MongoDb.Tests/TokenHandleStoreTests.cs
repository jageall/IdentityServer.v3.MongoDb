using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class TokenHandleStoreTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private ITokenHandleStore _store;
        private IReadOnlyList<Token> _subjectATokens;
        private IReadOnlyList<Token> _subjectBTokens;
        private IReadOnlyList<Token> _subjectCTokens;
        private const string RemovedKey = "remove";
        private const string NotRemovedKey = "donotremove";
        private const string SubjectA = "SubjectA";
        private const string SubjectB = "SubjectB";
        private const string SubjectC = "SubjectC";
        private const string RevokedClient = "Revoked";
        private const string NotRevokedClient = "NotRevoked";

        [Fact]
        public void NotRemovedTokenIsReturned()
        {
            _store.RemoveAsync(RemovedKey).Wait();
            var result = _store.GetAllAsync(SubjectA).Result.ToArray();
            Assert.Equal(1, result.Length);
            Assert.Equal(
                TestData.ToTestableString(_subjectATokens[1]),
                TestData.ToTestableString(result[0]));
        }

        [Fact]
        public void RemovedTokenIsNotReturned()
        {
            _store.RemoveAsync(RemovedKey).Wait();
            var result = _store.GetAllAsync(SubjectA).Result.ToArray();
            Assert.False(result.Any(r => r.ClientId == _subjectATokens[0].ClientId));      
        }

        [Fact]
        public void NonRevokedTokensAreReturned()
        {
            _store.RevokeAsync(SubjectB, RevokedClient).Wait();
            var results = _store.GetAllAsync(SubjectB).Result;
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
        public void RevokedTokensAreNotReturned()
        {
            _store.RevokeAsync(SubjectB, RevokedClient).Wait();
            var results = _store.GetAllAsync(SubjectB).Result;
            Assert.False(results.Any(x=>x.ClientId == RevokedClient));
        }

        [Fact]
        public void RevokingShouldNotEffectOtherSubjects()
        {
            _store.RevokeAsync(SubjectB, RevokedClient).Wait();
            var results = _store.GetAllAsync(SubjectC).Result;
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
        public void LoadingNonExistingKeyShouldResultInNull()
        {
            Assert.Null(_store.GetAsync("DoesNotExist").Result);
        }

        static DateTimeOffset CreationTime (Token token)
        {
            return token.CreationTime;
        }
        protected override void Initialize()
        {
            _store = Factory.Resolve<ITokenHandleStore>();

            var removed = TestData.Token(SubjectA);
            removed.Client.ClientId = removed.ClientId + 0;
            Save(removed.Client);
            
            _store.StoreAsync(RemovedKey, removed).Wait();
            var notRemoved = TestData.Token(SubjectA);
            notRemoved.Client.ClientId = notRemoved.ClientId + 1;
            Save(notRemoved.Client);
            _store.StoreAsync(NotRemovedKey, notRemoved).Wait();
            _subjectATokens = new List<Token> {removed, notRemoved};
            
            
            var subjectBTokens = new List<Token>();
            var subjectCTokens = new List<Token>();
            foreach(var subjectConfig in new[]
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
                Save(token.Client);
                _store.StoreAsync(subjectConfig.subject + i, token).Wait();
                subjectConfig.tokens.Add(token);
            }

            _subjectBTokens = subjectBTokens;
            _subjectCTokens = subjectCTokens;
        }
    }
}
