using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ConsentSerializerTests
    {
        private readonly Consent _expected;
        private readonly Consent _actual;
        private readonly BsonDocument _doc;
        private readonly ConsentSerializer _serializer;

        public ConsentSerializerTests()
        {
            _expected = new Consent
            {
                ClientId = "client",
                Subject = "subject",
                Scopes = new[] {"scope1", "scope2"}
            };

            _serializer = new ConsentSerializer();
            _doc = _serializer.Serialize(_expected);
            _actual = _serializer.Deserialize(_doc);
        }

        [Fact]
        public void SerializedDocShouldHaveId()
        {
            var id = _doc["_id"].AsGuid;
            Assert.Equal(_serializer.GetId(_expected), id);
        }

        [Fact]
        public void TwoConsentsWithTheSameSubjectAndClientShouldHaveTheSameId()
        {
            Assert.Equal(_serializer.GetId("client", "subject"), _serializer.GetId("client", "subject"));
        }

        [Fact]
        public void ConsentsWithDifferentSubjectsShouldHaveDifferentIds()
        {
            Assert.NotEqual(_serializer.GetId("client", "subject"), _serializer.GetId("client", "subject2"));
        }

        [Fact]
        public void ConsentsWithDifferentClientsShouldHaveDifferentIds()
        {
            Assert.NotEqual(_serializer.GetId("client", "subject"), _serializer.GetId("client2", "subject"));
        }


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
    }
}