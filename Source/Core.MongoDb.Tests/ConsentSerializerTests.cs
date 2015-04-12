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
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ConsentSerializerTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private Consent _expected;
        private Consent _actual;

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

        [Fact]
        public void CheckAll()
        {
            var serializer = new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
            Console.WriteLine(actual);
        }

        public ConsentSerializerTests(PersistenceTestFixture data)
            : base(data)
        {
            var store = Factory.Resolve<IConsentStore>();
            _expected = new Consent
            {
                ClientId = "client",
                Subject = "subject",
                Scopes = new[] { "scope1", "scope2" }
            };
            
            store.UpdateAsync(_expected).Wait();
            _actual = store.LoadAsync(_expected.Subject, _expected.ClientId).Result;
        }
    }
}