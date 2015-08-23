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
using IdentityServer3.MongoDb;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class Rfc4122UuidFromName
    {
        [Fact]
        public void CreatesExpectedGuid()
        {
            var expected = new Guid("3d813cbb-47fb-32ba-91df-831e1593ac29");
            var @namespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
            var name = "www.widgets.com";
            var actual = GuidGenerator.CreateGuidFromName(@namespace, name, 3);
            Assert.Equal(expected, actual);
        }
    }
}