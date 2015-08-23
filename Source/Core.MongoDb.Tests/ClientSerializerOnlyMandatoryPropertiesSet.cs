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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ClientSerializerOnlyMandatoryPropertiesSet : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private readonly Client _expected;
        private Client _actual;
        private Task _setup;


        [Fact]
        public async Task CheckAll()
        {
            await _setup;
            var serializer = new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
        }

        public ClientSerializerOnlyMandatoryPropertiesSet(PersistenceTestFixture data) : base(data)
        {
            
            _expected = TestData.ClientAllProperties();
            _setup = Setup();
        }

        private async Task Setup()
        {
            var store = Factory.Resolve<IClientStore>();
            await SaveAsync(TestData.ClientAllProperties());
            _actual = await store.FindClientByIdAsync(_expected.ClientId);
        }
    }
}