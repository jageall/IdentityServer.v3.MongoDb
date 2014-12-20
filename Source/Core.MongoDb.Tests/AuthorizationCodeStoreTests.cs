using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;
using Xunit.Sdk;

namespace Core.MongoDb.Tests
{
    public class AuthorizationCodeStoreTests : PersistenceTest, IUseFixture<RequireAdminService>
    {
        private IAuthorizationCodeStore _authorizationStore;
        private string _removeKey = "remove";
        private string _notRemovedKey = "notRemoved";
        private string _subjectA = "subjectA";
        private string _subjectB = "subjectB";
        private string _subjectC = "subjectC";
        private IReadOnlyList<AuthorizationCode> _subjectACodes;
        private IReadOnlyList<AuthorizationCode> _subjectBCodes;
        private IReadOnlyList<AuthorizationCode> _subjectCCodes;
        private readonly JsonSerializer _serializer;

        public AuthorizationCodeStoreTests()
        {
            _serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }

        [Fact]
        public void RemovedKeyIsNotFound()
        {
            _authorizationStore.RemoveAsync(_removeKey).Wait();
            Assert.Null(_authorizationStore.GetAsync(_removeKey).Result);
        }

        [Fact]
        public void NotRemovedKeyIsFound()
        {
            _authorizationStore.RemoveAsync(_removeKey).Wait();
            Assert.NotNull(_authorizationStore.GetAsync(_notRemovedKey).Result);
        }

        [Fact]
        public void GetAllShouldReturnAllCodes()
        {
            var result = _authorizationStore.GetAllAsync(_subjectA).Result.ToArray();

            var actual = result.OfType<AuthorizationCode>()
                .OrderBy(x => x.Nonce)
                .Select(x => JObject.FromObject(x, _serializer).ToString()).ToArray();
            var expected = _subjectACodes.OrderBy(x => x.Nonce)
                .Select(x => JObject.FromObject(x, _serializer).ToString()).ToArray();
            
                Assert.Equal(
                    expected,
                    actual);
        }

        [Fact]
        public void RevokedClientsCodesShouldNotBeReturned()
        {
            _authorizationStore.RevokeAsync(_subjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(_subjectB).Result.ToArray();
            Assert.False(result.Any(x=>x.ClientId == "revoked"));
        }

        [Fact]
        public void NonRevokedClientsCodesShouldNotBeReturned()
        {
            _authorizationStore.RevokeAsync(_subjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(_subjectB).Result.ToArray();

            Assert.Equal(
                _subjectBCodes
                    .Where(x=>x.ClientId != "revoked")
                    .OrderBy(x=>x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString()),
                result
                    .OfType<AuthorizationCode>()
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString())
                    );
        }

        [Fact]
        public void RevokingOneSubjectShouldNotEffectTheOther()
        {
            _authorizationStore.RevokeAsync(_subjectB, "revoked").Wait();
            var result = _authorizationStore.GetAllAsync(_subjectC).Result.ToArray();

            Assert.Equal(
                _subjectCCodes
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString()),
                result
                    .OfType<AuthorizationCode>()
                    .OrderBy(x => x.Nonce)
                    .Select(x => JObject.FromObject(x, _serializer).ToString())
                    );
        }
        protected override void Initialize()
        {
            _authorizationStore = Factory.AuthorizationCodeStore.TypeFactory(DependencyResolver);
            _authorizationStore.StoreAsync(_removeKey, TestData.AuthorizationCode());
            _authorizationStore.StoreAsync(_notRemovedKey, TestData.AuthorizationCode());
            var subjectACodes = new List<AuthorizationCode>();
            var subjectBCodes = new List<AuthorizationCode>();
            var subjectCCodes = new List<AuthorizationCode>();
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(_subjectA);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedA" + i, code);
                subjectACodes.Add(code);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(_subjectB);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedB" + i, code);
                subjectBCodes.Add(code);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(_subjectB);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                _authorizationStore.StoreAsync("revokedB" + i, code);
                subjectBCodes.Add(code);
            }
            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(_subjectC);
                code.Client.ClientId = "notRevoked";
                code.Nonce = "anr" + i;
                _authorizationStore.StoreAsync("notRevokedC" + i, code);
                subjectCCodes.Add(code);
            }

            for (int i = 0; i < 10; i++)
            {

                var code = TestData.AuthorizationCode(_subjectC);
                code.Client.ClientId = "revoked";
                code.Nonce = "ar" + i;
                _authorizationStore.StoreAsync("revokedC" + i, code);
                subjectCCodes.Add(code);
            }
            _subjectACodes = subjectACodes;
            _subjectBCodes = subjectBCodes;
            _subjectCCodes = subjectCCodes;
        }
    }
}
