using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeSerializerMandatoryPropertiesShouldRoundTrip
    {
        private readonly Scope _expected;
        private readonly Scope _actual;
        public ScopeSerializerMandatoryPropertiesShouldRoundTrip()
        {
            _expected = new Scope
            {
                Name = "name",
                DisplayName = "displayName"
            };
            var scopeSerializer = new ScopeSerializer();
            _actual = scopeSerializer.Deserialize(scopeSerializer.Serialize(_expected));
        }

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
    }
}