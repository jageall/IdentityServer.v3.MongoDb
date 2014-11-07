using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
                Scopes = new []{"scope1", "scope2"}
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

    public class Rfc4122UuidFromName
    {
        [Fact]
        public void CreatesExpectedGuid()
        {
            var expected = new Guid("3d813cbb-47fb-32ba-91df-831e1593ac29");
            var @namespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
            var name = "www.widgets.com";
            var actual = GuidGenerator.CreateGuidFromName(@namespace, name, 3);
            Assert.Equal(expected, actual);

        }
    }
}
