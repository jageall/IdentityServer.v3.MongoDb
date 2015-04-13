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
    public class RefreshTokenPersistenceTests: PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private RefreshToken _actual;
        private RefreshToken _expected;

        [Fact]
        public void NotNull()
        {
            Assert.NotNull(_actual);
        }

        [Fact]
        public void CheckToken()
        {
            Assert.NotNull(_actual.AccessToken);
            Assert.Equal(_expected.AccessToken.Scopes, _actual.AccessToken.Scopes);
        }

        [Fact]
        public void CheckClientId()
        {
            Assert.Equal(_expected.ClientId, _actual.ClientId);
        }

        [Fact]
        public void CheckCreationTime()
        {
            Assert.Equal(_expected.CreationTime, _actual.CreationTime);
        }

        [Fact]
        public void CheckLifeTime()
        {
            Assert.Equal(_expected.LifeTime, _actual.LifeTime);
        }
        [Fact]
        public void CheckScopes()
        {
            Assert.Equal(_expected.Scopes, _actual.Scopes);
        }

        [Fact]
        public void CheckSubjectId()
        {
            Assert.Equal(_expected.SubjectId, _actual.SubjectId);
        }

        [Fact]
        public void CheckAll()
        {
            var serializer = new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var expected = JObject.FromObject(_expected, serializer).ToString();
            var actual = JObject.FromObject(_actual, serializer).ToString();
            Assert.Equal(expected, actual);
        }

        public RefreshTokenPersistenceTests(PersistenceTestFixture data)
            : base(data)
        {
            var store = Factory.Resolve<IRefreshTokenStore>();
            Save(TestData.ClientAllProperties());
            var key = GetType().Name;
            _expected = TestData.RefreshToken();
            store.StoreAsync(key, TestData.RefreshToken());
            _actual = store.GetAsync(key).Result;
        }
    }
}

