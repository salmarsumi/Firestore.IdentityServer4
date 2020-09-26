using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using IdentityServer4.Firestore.Storage.DbContexts;
using IdentityServer4.Firestore.Storage.Mappers;
using IdentityServer4.Firestore.Stores;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;

namespace IdentityServer4.Firestore.IntegrationTests.Stores
{
    [Collection("FirestoreTests")]
    public class PersistedGrantStoreTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;
        private readonly PersistedGrantContext _context;

        public PersistedGrantStoreTests(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
            _context = new PersistedGrantContext(_fixture.DB);
        }

        private static PersistedGrant CreateTestObject(string sub = null, string clientId = null, string sid = null, string type = null)
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = type ?? "authorization_code",
                ClientId = clientId ?? Guid.NewGuid().ToString(),
                SubjectId = sub ?? Guid.NewGuid().ToString(),
                SessionId = sid ?? Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01).AsUtc(),
                Expiration = new DateTime(2016, 08, 31).AsUtc(),
                Data = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public async Task StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            var persistedGrant = CreateTestObject();

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);

            var foundGrant = (await _context.PersistedGrants.WhereEqualTo("Key", persistedGrant.Key).GetSnapshotAsync())[0].ConvertTo<Entities.PersistedGrant>();
            Assert.NotNull(foundGrant);
        }

        [Fact]
        public async Task GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.Document(persistedGrant.Key).SetAsync(persistedGrant.ToEntity());

            PersistedGrant foundPersistedGrant;
            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            foundPersistedGrant = await store.GetAsync(persistedGrant.Key);

            Assert.NotNull(foundPersistedGrant);
        }

        [Fact]
        public async Task GetAllAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.Document(persistedGrant.Key).SetAsync(persistedGrant.ToEntity());

            IList<PersistedGrant> foundPersistedGrants;
            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            foundPersistedGrants = (await store.GetAllAsync(new PersistedGrantFilter { SubjectId = persistedGrant.SubjectId })).ToList();

            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
        }

        [Fact]
        public async Task GetAllAsync_Should_Filter()
        {
            var model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t1");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t2");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t1");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t2");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t1");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t2");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t1");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t2");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject(sub: "sub1", clientId: "c3", sid: "s3", type: "t3");
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            model = CreateTestObject();
            await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());

            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1"
            })).ToList().Count.Should().Be(9);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1"
            })).ToList().Count.Should().Be(4);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c2"
            })).ToList().Count.Should().Be(4);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3"
            })).ToList().Count.Should().Be(1);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c4"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1"
            })).ToList().Count.Should().Be(2);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3",
                SessionId = "s1"
            })).ToList().Count.Should().Be(0);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t1"
            })).ToList().Count.Should().Be(1);
            (await store.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t3"
            })).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.Document(persistedGrant.Key).SetAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAsync(persistedGrant.Key);

            var foundGrant = (await _context.PersistedGrants.Document(persistedGrant.Key).GetSnapshotAsync()).ConvertTo<Entities.PersistedGrant>();
            Assert.Null(foundGrant);
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.Document(persistedGrant.Key).SetAsync(persistedGrant.ToEntity());

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId
            });

            var foundGrant = (await _context.PersistedGrants.Document(persistedGrant.Key).GetSnapshotAsync()).ConvertTo<Entities.PersistedGrant>();
            Assert.Null(foundGrant);
        }

        [Fact]
        public async Task RemoveAllAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.Document(persistedGrant.Key).SetAsync(persistedGrant.ToEntity());


            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = persistedGrant.SubjectId,
                ClientId = persistedGrant.ClientId,
                Type = persistedGrant.Type
            });

            var foundGrant = (await _context.PersistedGrants.Document(persistedGrant.Key).GetSnapshotAsync()).ConvertTo<Entities.PersistedGrant>();
            Assert.Null(foundGrant);
        }


        [Fact]
        public async Task RemoveAllAsync_Should_Filter()
        {
            var snapshots = await _context.PersistedGrants.GetSnapshotAsync();
            foreach(var doc in snapshots)
            {
                await doc.Reference.DeleteAsync();
            }

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());

            async Task PopulateDb()
            {
                var snapshots = await _context.PersistedGrants.GetSnapshotAsync();
                foreach(var doc in snapshots)
                {
                    await doc.Reference.DeleteAsync();
                }

                var model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t1");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s1", type: "t2");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t1");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c1", sid: "s2", type: "t2");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t1");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s1", type: "t2");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t1");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c2", sid: "s2", type: "t2");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject(sub: "sub1", clientId: "c3", sid: "s3", type: "t3");
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
                model = CreateTestObject();
                await _context.PersistedGrants.Document(model.Key).SetAsync(model.ToEntity());
            }

            await PopulateDb();

            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(1);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(10);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(6);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c2"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(6);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(9);


            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c4"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(10);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(8);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c3",
                SessionId = "s1"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(10);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t1"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(9);

            await PopulateDb();
            await store.RemoveAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "c1",
                SessionId = "s1",
                Type = "t3"
            });
            (await _context.PersistedGrants.GetSnapshotAsync()).Count.Should().Be(10);
        }

        [Fact]
        public async Task Store_should_create_new_record_if_key_does_not_exist()
        {
            var persistedGrant = CreateTestObject();

            var foundGrant = (await _context.PersistedGrants.Document(persistedGrant.Key).GetSnapshotAsync()).ConvertTo<Entities.PersistedGrant>();
            Assert.Null(foundGrant);

            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            await store.StoreAsync(persistedGrant);

            foundGrant = (await _context.PersistedGrants.Document(persistedGrant.Key).GetSnapshotAsync()).ConvertTo<Entities.PersistedGrant>();
            Assert.NotNull(foundGrant);
        }

        [Fact]
        public async Task Store_should_update_record_if_key_already_exists()
        {
            var persistedGrant = CreateTestObject();

            await _context.PersistedGrants.AddAsync(persistedGrant.ToEntity());

            var newDate = persistedGrant.Expiration.Value.AddHours(1);
            var store = new PersistedGrantStore(_context, FakeLogger<PersistedGrantStore>.Create());
            persistedGrant.Expiration = newDate;
            await store.StoreAsync(persistedGrant);

            var foundGrant = (await _context.PersistedGrants.WhereEqualTo("Key", persistedGrant.Key).GetSnapshotAsync())[0].ConvertTo<Entities.PersistedGrant>();
            Assert.NotNull(foundGrant);
            Assert.Equal(newDate, persistedGrant.Expiration);
        }
    }
}