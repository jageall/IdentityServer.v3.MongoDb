using System.Linq;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class PersistenceTests : IUseFixture<RequireAdminService>
    {
        private ServiceFactory _factory;
        private IAdminService _adminService;

        [Fact]
        public void Client()
        {
            var store = _factory.ClientStore.TypeFactory();
            var expected = TestData.ClientAllProperties();
            _adminService.Save(expected);
            var actual = store.FindClientByIdAsync(expected.ClientId).Result;
            Assert.NotNull(actual);
            Assert.Equal(expected.ClientId, actual.ClientId);
        }

        [Fact]
        public void Scope()
        {
            var store = _factory.ScopeStore.TypeFactory();
            var expected = TestData.ScopeAllProperties();
            _adminService.Save(expected);
            var actual = store.GetScopesAsync().Result.First();
            Assert.NotNull(actual);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void Consent()
        {
            var store = _factory.ConsentStore.TypeFactory();
            var expected = new Consent
            {
                ClientId = "client",
                Subject = "subject",
                Scopes = new[] {"scope1", "scope2"}
            };
            ;
            store.UpdateAsync(expected).Wait();
            var actual = store.LoadAsync(expected.Subject, expected.ClientId).Result;
            Assert.NotNull(actual);
            Assert.Equal(expected.Scopes, actual.Scopes);
        }

        [Fact]
        public void AuthorizationCode()
        {
            var store = _factory.AuthorizationCodeStore.TypeFactory();
            var expected = TestData.AuthorizationCode();
            store.StoreAsync("key", expected);
            var actual = store.GetAsync("key").Result;
            Assert.NotNull(actual);
            Assert.Equal(expected.Scopes, actual.Scopes);
        }

        public void SetFixture(RequireAdminService data)
        {
            _factory = data.Factory;
            _adminService = data.AdminService;
        }
    }
}