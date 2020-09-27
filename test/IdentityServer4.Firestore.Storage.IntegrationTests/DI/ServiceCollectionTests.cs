using FluentAssertions;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.Firestore.IntegrationTests.DI
{
    [Collection("FirestoreTests")]
    public class ServiceCollectionTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;

        public ServiceCollectionTests(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CanRegisterFirestoreDb()
        {
            var services = new ServiceCollection();
            var projectId = _fixture.Configuration["ProjectId"];

            services.AddFirestoreDb(options => options.ProjectId = projectId);
            var scope = services.BuildServiceProvider();
            var db = scope.GetRequiredService<FirestoreDb>();

            db.Should().NotBeNull();
        }
    }
}
