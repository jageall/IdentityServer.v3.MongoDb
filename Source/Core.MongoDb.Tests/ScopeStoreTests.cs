using System.Collections.Generic;
using System.Linq;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ScopeStoreTests : PersistenceTest, IUseFixture<PersistenceTestFixture>
    {
        private List<string> _evenScopeNames;
        private List<string> _oddScopeNames;
        private IScopeStore _scopeStore;

        [Fact]
        public void ShouldContainRequestedScopeNames()
        {
            var result = _scopeStore.FindScopesAsync(_evenScopeNames).Result;
            foreach (var evenScopeName in _evenScopeNames)
            {
                Assert.Contains(evenScopeName, result.Select(x => x.Name));
            }
        }

        [Fact]
        public void ShouldNotContainScopeNamesThatWereNotRequested()
        {
            var result = _scopeStore.FindScopesAsync(_evenScopeNames).Result;
            foreach (var oddScopeName in _oddScopeNames)
            {
                Assert.DoesNotContain(oddScopeName, result.Select(x=>x.Name));
            }

        }

        [Fact]
        public void ShouldOnlyGetPublicScopes()
        {
            var result = _scopeStore.GetScopesAsync(publicOnly: true).Result;
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
        public void ShouldGetAllScopes()
        {

            var result = _scopeStore.GetScopesAsync(publicOnly: false).Result;
            foreach (var evenScopeName in _evenScopeNames)
            {
                Assert.Contains(evenScopeName, result.Select(x => x.Name));
            }

            foreach (var oddScopeName in _oddScopeNames)
            {
                Assert.Contains(oddScopeName, result.Select(x => x.Name));
            }
        }

        protected override void Initialize()
        {
            _scopeStore = Factory.ScopeStore.TypeFactory(null);
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
                AdminService.Save(scope);                
            }
        }
    }
}
