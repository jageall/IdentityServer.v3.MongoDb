using System;
using IdentityServer.Core.MongoDb;
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