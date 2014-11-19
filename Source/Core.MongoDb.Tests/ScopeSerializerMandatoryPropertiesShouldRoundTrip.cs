using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeSerializerMandatoryPropertiesShouldRoundTrip : PersistenceTest, IUseFixture<RequireAdminService>
    {
        private Scope _expected;
        private Scope _actual;


        [Fact]
        public void ShouldNotBeNull()
        {
            Assert.NotNull(_actual);
        }

        [Fact]
        public void CheckName()
        {
            Assert.Equal(_expected.Name, _actual.Name);
        }

        [Fact]
        public void CheckClaimsRule()
        {
            Assert.Equal(_expected.ClaimsRule, _actual.ClaimsRule);
        }

        [Fact]
        public void CheckClaims()
        {
            Assert.Equal(_expected.Claims, _actual.Claims);
        }

        [Fact]
        public void CheckDescription()
        {
            Assert.Equal(_expected.Description, _actual.Description);
        }

        [Fact]
        public void CheckDisplayName()
        {
            Assert.Equal(_expected.DisplayName, _actual.DisplayName);
        }

        [Fact]
        public void CheckEmphasize()
        {
            Assert.Equal(_expected.Emphasize, _actual.Emphasize);
        }

        [Fact]
        public void CheckEnabled()
        {
            Assert.Equal(_expected.Enabled, _actual.Enabled);
        }

        [Fact]
        public void CheckIncludeAllClaimsForUser()
        {
            Assert.Equal(_expected.IncludeAllClaimsForUser, _actual.IncludeAllClaimsForUser);
        }

        [Fact]
        public void CheckRequired()
        {
            Assert.Equal(_expected.Required, _actual.Required);
        }

        [Fact]
        public void CheckShowInDiscoveryDocument()
        {
            Assert.Equal(_expected.ShowInDiscoveryDocument, _actual.ShowInDiscoveryDocument);
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
            _expected = TestData.ScopeMandatoryProperties();
            AdminService.Save(_expected);
            _actual = Factory.ScopeStore.TypeFactory().GetScopesAsync().Result.SingleOrDefault(x => x.Name == _expected.Name);

        }
    }
}