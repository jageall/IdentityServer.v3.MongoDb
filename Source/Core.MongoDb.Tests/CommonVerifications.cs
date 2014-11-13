using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class CommonVerifications
    {
        private static string ClaimToString(Claim claim)
        {
            return string.Format("{0}:{1}", claim.Type, claim.Value);
        }

        public static void VerifyClaimset(IEnumerable<Claim> expected, IEnumerable<Claim> actual)
        {
            
            var expectedClaims = expected.GroupBy(ClaimToString).ToArray();

            var actualClaims =
                actual.GroupBy(ClaimToString)
                    .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            foreach (var expectedClaim in expectedClaims)
            {
                Assert.True(
                    actualClaims.Any(
                        x =>
                            string.Equals(x.Key, expectedClaim.Key, StringComparison.Ordinal) &&
                            x.Count() == expectedClaim.Count()), string.Format("Failed to find the claim {0}", expectedClaim.Key));
            }
        }
    }
}