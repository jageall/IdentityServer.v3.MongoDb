using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeSeriailzerAllPropertiesShouldRoundTrip
    {
        private readonly Scope _expected;
        private readonly Scope _actual;

        public ScopeSeriailzerAllPropertiesShouldRoundTrip()
        {
            _expected = new Scope
            {
                Name = "name",
                DisplayName = "displayName",
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim
                    {
                        Name = "claim1", 
                        AlwaysIncludeInIdToken = false, 
                        Description = "claim1 description"
                    },
                    new ScopeClaim
                    {
                        Name = "claim2", 
                        AlwaysIncludeInIdToken = true, 
                        Description = "claim2 description"
                    },
                },
                ClaimsRule = "claimsRule",
                Description = "Description",
                Emphasize = true,
                Enabled = false,
                IncludeAllClaimsForUser = true,
                Required = true,
                ShowInDiscoveryDocument = false,
                Type = ScopeType.Identity
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
            Assert.Equal(_expected.Claims, _actual.Claims, new TestScopeComparer());
        }

        public class TestScopeComparer : IEqualityComparer<ScopeClaim>
        {
            public bool Equals(ScopeClaim x, ScopeClaim y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return string.Equals(x.Name, y.Name, StringComparison.Ordinal)
                       && x.AlwaysIncludeInIdToken == y.AlwaysIncludeInIdToken
                       && string.Equals(x.Description, y.Description, StringComparison.Ordinal);
            }

            public int GetHashCode(ScopeClaim obj)
            {
                return 1;
            }
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
