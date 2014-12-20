using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class RefreshTokenPersistenceTests: PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private RefreshToken _actual;
        private RefreshToken _expected;

        [Fact]
        public void NotNull()
        {
            Assert.NotNull(_actual);
        }

        [Fact]
        public void CheckToken()
        {
            Assert.NotNull(_actual.AccessToken);
            Assert.Equal(_expected.AccessToken.Scopes, _actual.AccessToken.Scopes);
        }

        [Fact]
        public void CheckClientId()
        {
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void CheckCreationTime()
        {
            Assert.Equal(_expected.CreationTime, _actual.CreationTime);
        }

        [Fact]
        public void CheckLifeTime()
        {
            Assert.Equal(_expected.LifeTime, _actual.LifeTime);
        }
        [Fact]
        public void CheckScopes()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
        }

        [Fact]
        public void CheckSubjectId()
        {
            Assert.Equal(_expected.SubjectId, _actual.SubjectId);
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
            var store = Factory.RefreshTokenStore.TypeFactory(DependencyResolver);
            var key = GetType().Name;
            _expected = TestData.RefreshToken();
            store.StoreAsync(key, TestData.RefreshToken());
            _actual = store.GetAsync(key).Result;
        }
    }
}

