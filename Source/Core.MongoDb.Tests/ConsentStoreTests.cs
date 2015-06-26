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
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
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
        private Task _setup;

        [Fact]
        public async Task CanLoadAllConsents()
        {
            await _setup;
            var results = await _store.LoadAllAsync(SubjectA);
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
        public async Task InvalidSubjectShouldBeEmptySet()
        {
            await _setup;
            var results = await _store.LoadAllAsync("Invalid");
            Assert.Empty(results);
        }

        [Fact]
        public async Task InvalidSubjectAndClientShouldBeNull()
        {
            await _setup;
            var results = await _store.LoadAsync("Invalid", "Invalid");
            Assert.Null(results);
        }

        [Fact]
        public async Task UpdatingConsentShouldResultInNewConsentBeingReturned()
        {
            await _setup;
            var consentToUpdate = _subjectCConsents.OrderBy(ClientIdOrdering).Skip(2).First();
            consentToUpdate.Scopes = new[] {"scope3", "scope4"};
            await _store.UpdateAsync(consentToUpdate);
            var stored = await _store.LoadAsync(consentToUpdate.Subject, consentToUpdate.ClientId);
            Assert.Equal(
                TestData.ToTestableString(consentToUpdate),
                TestData.ToTestableString(stored));
        }

        [Fact]
        public async Task RevokedConsentsShouldNotBeReturned()
        {
            await _setup;
            var indexedConsents = _subjectBConsents.OrderBy(ClientIdOrdering)
                .Select((x, i) => new {Index = i, Consent = x}).ToArray();
            foreach (var indexedConsent in indexedConsents)
            {
                if (indexedConsent.Index%2 == 0)
                {
                    await _store.RevokeAsync(indexedConsent.Consent.Subject, indexedConsent.Consent.ClientId);
                }
            }
            var results = await _store.LoadAllAsync(SubjectB);
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
            List<Task> tasks = new List<Task>();
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
                tasks.Add(_store.UpdateAsync(consent));
            }

            _setup = Task.WhenAll(tasks);
        }
    }
}
