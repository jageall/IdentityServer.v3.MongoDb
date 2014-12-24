using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeSerializationTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private AuthorizationCode _expected;
        private AuthorizationCode _actual;

        [Fact]
        public void CheckCreationTime()
        {
            Assert.Equal(_expected.CreationTime, _actual.CreationTime);
        }

        [Fact]
        public void CheckIsOpenId()
        {
            Assert.Equal(_expected.IsOpenId, _actual.IsOpenId);
        }

        [Fact]
        public void CheckRedirectUri()
        {
            Assert.Equal(_expected.RedirectUri, _actual.RedirectUri);
        }

        [Fact]
        public void CheckConsentWasShown()
        {
            Assert.Equal(_expected.WasConsentShown, _actual.WasConsentShown);
        }

        [Fact]
        public void CheckSubject()
        {
            Assert.NotNull(_actual.Subject);
            Assert.Equal(_expected.Subject.Identities.Count(), _actual.Subject.Identities.Count());
            foreach (var identity in _expected.Subject.Identities)
            {
                var actual =
                    _actual.Subject.Identities.SingleOrDefault(
                        x => string.Equals(x.AuthenticationType, identity.AuthenticationType, StringComparison.Ordinal));
                Assert.NotNull(actual);
                CommonVerifications.VerifyClaimset(identity.Claims, actual.Claims);
            }
        }

        [Fact]
        public void CheckScopes()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
        }


        [Fact]
        public void CheckClient()
        {
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void CheckAll()
        {
            var serializer = new JsonSerializer(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
            Console.WriteLine(actual);
        }

        protected override void Initialize()
        {
            var key = "AuthorizationCodeTests";
            _expected = TestData.AuthorizationCode();
            var store = Factory.Resolve<IAuthorizationCodeStore>();
            store.StoreAsync(key, TestData.AuthorizationCode()).Wait();
            _actual = store.GetAsync(key).Result;
        }
    }
}