using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ConsentSerializerTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private Consent _expected;
        private Consent _actual;

        [Fact]
        public void RoundTripConsentShouldNotBeNull()
        {
            Assert.NotNull(_actual);
        }

        [Fact]
        public void RoundTripClientShouldBeTheSame()
        {
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void RoundTripSubjectShouldBeTheSame()
        {
            Assert.Equal(_expected.Subject, _actual.Subject);
        }

        [Fact]
        public void RoundTripScopesShouldBeTheSame()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
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
            var store = Factory.Resolve<IConsentStore>();
            _expected = new Consent
            {
                ClientId = "client",
                Subject = "subject",
                Scopes = new[] { "scope1", "scope2" }
            };
            
            store.UpdateAsync(_expected).Wait();
            _actual = store.LoadAsync(_expected.Subject, _expected.ClientId).Result;
        }
    }
}