/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeSeriailzerAllPropertiesShouldRoundTrip : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private Scope _expected;
        private Scope _actual;
        private Task _setup;
        [Fact]
        public async Task ShouldNotBeNull()
        {
            await _setup;
            Assert.NotNull(_actual);
        }

        [Fact]
        public async Task CheckName()
        {
            await _setup;
            Assert.Equal(_expected.Name, _actual.Name);
        }

        [Fact]
        public async Task CheckClaimsRule()
        {
            await _setup;
            Assert.Equal(_expected.ClaimsRule, _actual.ClaimsRule);
        }

        [Fact]
        public async Task CheckClaims()
        {
            await _setup;
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
        public async Task CheckDescription()
        {
            await _setup;
            Assert.Equal(_expected.Description, _actual.Description);
        }

        [Fact]
        public async Task CheckDisplayName()
        {
            await _setup;
            Assert.Equal(_expected.DisplayName, _actual.DisplayName);
        }

        [Fact]
        public async Task CheckEmphasize()
        {
            await _setup;
            Assert.Equal(_expected.Emphasize, _actual.Emphasize);
        }

        [Fact]
        public async Task CheckEnabled()
        {
            await _setup;
            Assert.Equal(_expected.Enabled, _actual.Enabled);
        }

        [Fact]
        public async Task CheckIncludeAllClaimsForUser()
        {
            await _setup;
            Assert.Equal(_expected.IncludeAllClaimsForUser, _actual.IncludeAllClaimsForUser);
        }

        [Fact]
        public async Task CheckRequired()
        {
            await _setup;
            Assert.Equal(_expected.Required, _actual.Required);
        }

        [Fact]
        public async Task CheckShowInDiscoveryDocument()
        {
            await _setup;
            Assert.Equal(_expected.ShowInDiscoveryDocument, _actual.ShowInDiscoveryDocument);
        }

        [Fact]
        public async Task CheckType()
        {
            await _setup;
            Assert.Equal(_expected.Type, _actual.Type);
        }

        public ScopeSeriailzerAllPropertiesShouldRoundTrip(PersistenceTestFixture data)
            : base(data)
        {
            _expected = TestData.ScopeAllProperties();
            _setup = Setup();
        }

        private async Task Setup()
        {
            await SaveAsync(_expected);
            _actual =
                (await Factory.Resolve<IScopeStore>().GetScopesAsync(publicOnly: false)).SingleOrDefault(
                        x => x.Name == _expected.Name);
        }
    }
}