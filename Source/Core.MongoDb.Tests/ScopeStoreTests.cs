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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeStoreTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private List<string> _evenScopeNames;
        private List<string> _oddScopeNames;
        private IScopeStore _scopeStore;

        [Fact]
        public async Task ShouldContainRequestedScopeNames()
        {
            var result = (await _scopeStore.FindScopesAsync(_evenScopeNames)).ToArray();
            foreach (var evenScopeName in _evenScopeNames)
            {
                Assert.Contains(evenScopeName, result.Select(x => x.Name));
            }
        }

        [Fact]
        public async Task ShouldNotContainScopeNamesThatWereNotRequested()
        {
            var result = (await _scopeStore.FindScopesAsync(_evenScopeNames)).ToArray();
            foreach (var oddScopeName in _oddScopeNames)
            {
                Assert.DoesNotContain(oddScopeName, result.Select(x=>x.Name));
            }

        }

        [Fact]
        public async Task ShouldOnlyGetPublicScopes()
        {
            var result = (await _scopeStore.GetScopesAsync(publicOnly: true)).ToArray();
            foreach (var evenScopeName in _evenScopeNames)
            {
                Assert.DoesNotContain(evenScopeName, result.Select(x => x.Name));
            }

            foreach (var oddScopeName in _oddScopeNames)
            {
                Assert.Contains(oddScopeName, result.Select(x => x.Name));
            }
        }
        [Fact]
        public async Task ShouldGetAllScopes()
        {

            var result = (await _scopeStore.GetScopesAsync(publicOnly: false)).ToArray();
            foreach (var evenScopeName in _evenScopeNames)
            {
                Assert.Contains(evenScopeName, result.Select(x => x.Name));
            }

            foreach (var oddScopeName in _oddScopeNames)
            {
                Assert.Contains(oddScopeName, result.Select(x => x.Name));
            }
        }

        public ScopeStoreTests(PersistenceTestFixture data)
            : base(data)
        {
            _scopeStore = Factory.Resolve<IScopeStore>();
            _evenScopeNames = new List<string>();
            _oddScopeNames = new List<string>();
            var scopes = Enumerable.Range(1, 10).Select(x =>
            {
                var scope = TestData.ScopeAllProperties();
                scope.Name = scope.Name + x;
                if (x%2 == 0)
                {
                    _evenScopeNames.Add(scope.Name);
                    scope.ShowInDiscoveryDocument = false; //private
                }
                else
                {
                    _oddScopeNames.Add(scope.Name);
                    scope.ShowInDiscoveryDocument = true; //public

                }
                return scope;
            });

            foreach (var scope in scopes)
            {
                Save(scope);                
            }
        }
    }
}
