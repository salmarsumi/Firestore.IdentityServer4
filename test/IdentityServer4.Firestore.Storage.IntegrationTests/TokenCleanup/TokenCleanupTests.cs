using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Firestore.Entities;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Options;
using IdentityServer4.Firestore.Storage.DbContexts;
using IdentityServer4.Firestore.Stores;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.Firestore.IntegrationTests.TokenCleanup
{
    public class TokenCleanupTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;
        private readonly PersistedGrantContext _context;

        public TokenCleanupTests(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
            _context = new PersistedGrantContext(_fixture.DB);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredGrantsExist_ExpectExpiredGrantsRemoved()
        {
            var expiredGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "app1",
                Type = "reference",
                SubjectId = "123",
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            await _context.PersistedGrants.Document(expiredGrant.Key).SetAsync(expiredGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            (await _context.PersistedGrants.WhereEqualTo("Key", expiredGrant.Key).GetSnapshotAsync()).Count.Should().Be(0);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenValidGrantsExist_ExpectValidGrantsInDb()
        {
            var validGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "app1",
                Type = "reference",
                SubjectId = "123",
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            await _context.PersistedGrants.AddAsync(validGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            (await _context.PersistedGrants.WhereEqualTo("Key", validGrant.Key).GetSnapshotAsync()).Count.Should().Be(1);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenExpiredDeviceGrantsExist_ExpectExpiredDeviceGrantsRemoved()
        {
            var expiredGrant = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = Guid.NewGuid().ToString(),
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            await _context.DeviceFlowCodes.AddAsync(expiredGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            (await _context.DeviceFlowCodes.WhereEqualTo("DeviceCode", expiredGrant.DeviceCode).GetSnapshotAsync()).Count.Should().Be(0);
        }

        [Fact]
        public async Task RemoveExpiredGrantsAsync_WhenValidDeviceGrantsExist_ExpectValidDeviceGrantsInDb()
        {
            var validGrant = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                UserCode = "2468",
                ClientId = "app1",
                SubjectId = "123",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "{!}"
            };

            await _context.DeviceFlowCodes.AddAsync(validGrant);

            await CreateSut().RemoveExpiredGrantsAsync();

            (await _context.DeviceFlowCodes.WhereEqualTo("DeviceCode", validGrant.DeviceCode).GetSnapshotAsync()).Count.Should().Be(1);
        }

        private TokenCleanupService CreateSut()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddIdentityServer()
                .AddTestUsers(new List<TestUser>())
                .AddInMemoryClients(new List<Models.Client>())
                .AddInMemoryIdentityResources(new List<Models.IdentityResource>())
                .AddInMemoryApiResources(new List<Models.ApiResource>());

            services.AddScoped<IPersistedGrantContext, PersistedGrantContext>(_ =>
                new PersistedGrantContext(_fixture.DB));
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            
            services.AddTransient<TokenCleanupService>();
            services.AddSingleton<OperationalStoreOptions>(_ => new OperationalStoreOptions());

            return services.BuildServiceProvider().GetRequiredService<TokenCleanupService>();
            //return new EntityFramework.TokenCleanupService(
            //    services.BuildServiceProvider(),
            //    new NullLogger<EntityFramework.TokenCleanup>(),
            //    StoreOptions);
        }
    }
}