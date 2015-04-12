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
using System.Collections.Generic;
using System.Linq;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class ConsentStoreTests : PersistenceTest, IClassFixture<PersistenceTestFixture>
    {
        private IConsentStore _store;
        private const string SubjectA = "SubjectA";
        private const string SubjectB = "SubjectB";
        private const string SubjectC = "SubjectC";

        private IReadOnlyList<Consent> _subjectAConsents;
        private IReadOnlyList<Consent> _subjectBConsents;
        private IReadOnlyList<Consent> _subjectCConsents;
        
        [Fact]
        public void CanLoadAllConsents()
        {
            var results = _store.LoadAllAsync(SubjectA).Result;
            Assert.Equal(
                _subjectAConsents
                    .OrderBy(ClientIdOrdering)
                    .Select(TestData.ToTestableString),
                results
                    .OrderBy(ClientIdOrdering)
                    .Select(TestData.ToTestableString)
                );
        }

        [Fact]
        public void InvalidSubjectShouldBeEmptySet()
        {
            var results = _store.LoadAllAsync("Invalid").Result;
            Assert.Empty(results);
        }

        [Fact]
        public void InvalidSubjectAndClientShouldBeNull()
        {
            var results = _store.LoadAsync("Invalid","Invalid").Result;
            Assert.Null(results);
        }

        [Fact]
        public void UpdatingConsentShouldResultInNewConsentBeingReturned()
        {
            var consentToUpdate = _subjectCConsents.OrderBy(ClientIdOrdering).Skip(2).First();
            consentToUpdate.Scopes = new[] {"scope3", "scope4"};
            _store.UpdateAsync(consentToUpdate).Wait();
            var stored = _store.LoadAsync(consentToUpdate.Subject, consentToUpdate.ClientId).Result;
            Assert.Equal(
                TestData.ToTestableString(consentToUpdate),
                TestData.ToTestableString(stored));
        }

        [Fact]
        public void RevokedConsentsShouldNotBeReturned()
        {
            var indexedConsents = _subjectBConsents.OrderBy(ClientIdOrdering)
                .Select((x, i) => new {Index = i, Consent = x}).ToArray();
            foreach (var indexedConsent in indexedConsents)
            {
                if (indexedConsent.Index%2 == 0)
                {
                    _store.RevokeAsync(indexedConsent.Consent.Subject, indexedConsent.Consent.ClientId).Wait();
                }
            }
            var results = _store.LoadAllAsync(SubjectB).Result;
            Assert.Equal(
                indexedConsents
                    .Where(x=> x.Index%2 != 0)
                    .Select(x=> TestData.ToTestableString(x.Consent))
                    .ToArray(),
                results
                    .OrderBy(ClientIdOrdering)
                    .Select(TestData.ToTestableString)
                    .ToArray()
                );
        }
        static string ClientIdOrdering(Consent consent)
        {
            return consent.ClientId;
        }

        public ConsentStoreTests(PersistenceTestFixture data)
            : base(data)
        {
            _store = Factory.Resolve<IConsentStore>();
            _subjectAConsents = new List<Consent>();
            _subjectBConsents = new List<Consent>();
            _subjectCConsents = new List<Consent>();
            foreach(var subject in new []
            {
                new
                {
                    Subject = SubjectA, 
                    Consents = (List<Consent>)_subjectAConsents
                },
                new
                {
                    Subject = SubjectB, 
                    Consents = (List<Consent>)_subjectBConsents
                },
                new
                {
                    Subject = SubjectC, 
                    Consents = (List<Consent>)_subjectCConsents
                }
            })
            for (int i = 0; i < 10; i++)
            {
                var consent = new Consent() {ClientId = "ClientId" + i, Scopes = new[] {"scope1", "scope2"}, Subject = subject.Subject};
                subject.Consents.Add(consent);
                _store.UpdateAsync(consent).Wait();
            }
            
        }
    }
}
