using System;
using System.Linq;
using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeSerializationTests
    {
        private readonly AuthorizationCode _expected;
        private readonly AuthorizationCode _actual;
        private readonly string _key;
        private readonly BsonDocument _doc;

        public AuthorizationCodeSerializationTests()
        {
            _expected = TestData.AuthorizationCode();

            _key = Guid.NewGuid().ToString("N");
            var serializer = new AuthorizationCodeSerializer();
            _doc = serializer.Serialize(_key, _expected);
            _actual = serializer.Deserialize(_doc);
        }

        [Fact]
        public void ShouldSerializeKeyAsId()
        {
            Assert.Equal(_key, _doc["_id"].AsString);
        }

        [Fact]
        public void CheckExpiry()
        {
            var expected = _expected.CreationTime.AddSeconds(_expected.Client.AuthorizationCodeLifetime);
            Assert.Equal(expected, _doc["_expiry"].ToUniversalTime());
        }

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
                var expectedClaims = identity.Claims.GroupBy(x => x.Type + x.Value).OrderBy(x => x.Key).ToArray();
                var actualClaims =
                    actual.Claims.GroupBy(x => x.Type + x.Value)
                        .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                        .ToArray();
                foreach (var expectedClaim in expectedClaims)
                {
                    Assert.True(
                        actualClaims.Any(
                            x =>
                                string.Equals(x.Key, expectedClaim.Key, StringComparison.Ordinal) &&
                                x.Count() == expectedClaim.Count()));
                }
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
    }
}