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