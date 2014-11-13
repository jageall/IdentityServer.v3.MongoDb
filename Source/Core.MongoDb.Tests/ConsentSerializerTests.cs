using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ConsentSerializerTests : PersistenceTest, IUseFixture<RequireAdminService>
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

        protected override void Initialize()
        {
            var store = Factory.ConsentStore.TypeFactory();
            _expected = new Consent
            {
                ClientId = "client",
                Subject = "subject",
                Scopes = new[] { "scope1", "scope2" }
            };
            ;
            store.UpdateAsync(_expected).Wait();
            _actual = store.LoadAsync(_expected.Subject, _expected.ClientId).Result;
        }
    }
}