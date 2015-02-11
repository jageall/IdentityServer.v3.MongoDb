using System;
using IdentityServer.Core.MongoDb;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class TokenHandlePersistenceTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private Token _actual;
        private Token _expected;

        [Fact]
        public void NotNull()
        {
            Assert.NotNull(_actual);
        }

        [Fact]
        public void CheckAudience()
        {
            Assert.Equal(_expected.Audience, _actual.Audience);
        }
        [Fact]
        public void CheckClaims()
        {
            CommonVerifications.VerifyClaimset(_expected.Claims, _actual.Claims);
        }

        [Fact]
        public void CheckClient()
        {
            Assert.NotNull(_actual.Client);
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void CheckCreationTime()
        {
            Assert.Equal(_expected.CreationTime, _actual.CreationTime);
        }

        [Fact]
        public void CheckIssuer()
        {
            Assert.Equal(_expected.Issuer, _actual.Issuer);
        }

        [Fact]
        public void CheckLifetime()
        {
            Assert.Equal(_expected.Lifetime, _actual.Lifetime);
        }


        [Fact]
        public void CheckSubjectId()
        {
            Assert.Equal(_expected.SubjectId, _actual.SubjectId);
        }

        [Fact]
        public void CheckScopes()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
        }

        [Fact]
        public void CheckType()
        {
            Assert.Equal(_expected.Type, _actual.Type);
        }

        [Fact]
        public void CheckAll()
        {
            var serializer = new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
            Console.WriteLine(actual);
        }

        protected override void Initialize()
        {
            var store = Factory.Resolve<ITokenHandleStore>();
            Save(TestData.ClientAllProperties());
            
            var key = GetType().Name;
            _expected = TestData.Token();
            store.StoreAsync(key, TestData.Token());
            _actual = store.GetAsync(key).Result;
        }
    }
}