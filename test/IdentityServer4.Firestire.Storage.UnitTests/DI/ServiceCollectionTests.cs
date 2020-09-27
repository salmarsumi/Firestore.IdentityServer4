using FluentAssertions;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Storage;
using IdentityServer4.Firestore.Storage.UnitTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer4.Firestore.UnitTests.DI
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void CanRegisterConfigurationContext()
        {
            var services = new ServiceCollection();

            services.AddConfigurationDbContext<FakeConfigurationContext>();
            var scope = services.BuildServiceProvider();
            var context = scope.GetRequiredService<IConfigurationContext>();

            context.Should().NotBeNull();
        }

        [Fact]
        public void CanRegisterOperationalContext()
        {
            var services = new ServiceCollection();

            services.AddOperationalDbContext<FakePersistedGrantContext>();
            services.AddTransient<ILogger<TokenCleanupService>, FakeLogger<TokenCleanupService>>();
            var scope = services.BuildServiceProvider();
            var context = scope.GetRequiredService<IPersistedGrantContext>();
            var cleanupService = scope.GetRequiredService<TokenCleanupService>();

            context.Should().NotBeNull();
            cleanupService.Should().NotBeNull();
        }
    }
}
