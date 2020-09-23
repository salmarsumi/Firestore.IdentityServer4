using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Firestore.Mappers;
using IdentityServer4.Firestore.Storage.DbContexts;
using IdentityServer4.Firestore.Stores;
using IdentityServer4.Models;
using Xunit;
using Xunit.Sdk;

namespace IdentityServer4.Firestore.IntegrationTests.Stores
{
    [Collection("FirestoreTests")]
    public class ClientStoreTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;
        private readonly ConfigurationContext _context;

        public ClientStoreTests(FirestoreTestFixture fixture)
        {
            this._fixture = fixture;
            _context = new ConfigurationContext(this._fixture.DB);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientDoesNotExist_ExpectNull()
        {
            var store = new ClientStore(_context, FakeLogger<ClientStore>.Create());
            var client = await store.FindClientByIdAsync(Guid.NewGuid().ToString());
            client.Should().BeNull();
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
        {
            var testClient = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            await _context.Clients.Document(testClient.ClientId).SetAsync(testClient.ToEntity());

            Client client;
            var store = new ClientStore(_context, FakeLogger<ClientStore>.Create());
            client = await store.FindClientByIdAsync(testClient.ClientId);

            client.Should().NotBeNull();
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientExistsWithCollections_ExpectClientReturnedCollections()
        {
            var testClient = new Client
            {
                ClientId = "properties_test_client",
                ClientName = "Properties Test Client",
                AllowedCorsOrigins = {"https://localhost"},
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowedScopes = {"openid", "profile", "api1"},
                Claims = {new ClientClaim("test", "value")},
                ClientSecrets = {new Secret("secret".Sha256())},
                IdentityProviderRestrictions = {"AD"},
                PostLogoutRedirectUris = {"https://locahost/signout-callback"},
                Properties = {{"foo1", "bar1"}, {"foo2", "bar2"},},
                RedirectUris = {"https://locahost/signin"}
            };

            await _context.Clients.Document(testClient.ClientId).SetAsync(testClient.ToEntity());

            Client client;
            var store = new ClientStore(_context, FakeLogger<ClientStore>.Create());
            client = await store.FindClientByIdAsync(testClient.ClientId);

            client.Should().BeEquivalentTo(testClient);
        }

        [Fact]
        public async Task FindClientByIdAsync_WhenClientsExistWithManyCollections_ExpectClientReturnedInUnderFiveSeconds()
        {
            var testClient = new Client
            {
                ClientId = "test_client_with_uris",
                ClientName = "Test client with URIs",
                AllowedScopes = {"openid", "profile", "api1"},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials
            };

            for (int i = 0; i < 50; i++)
            {
                testClient.RedirectUris.Add($"https://localhost/{i}");
                testClient.PostLogoutRedirectUris.Add($"https://localhost/{i}");
                testClient.AllowedCorsOrigins.Add($"https://localhost:{i}");
            }

            await _context.Clients.Document(testClient.ClientId).SetAsync(testClient.ToEntity());
            for (int i = 0; i < 50; i++)
            {
                await _context.Clients.Document(testClient.ClientId + i).SetAsync(new Client
                {
                    ClientId = testClient.ClientId + i,
                    ClientName = testClient.ClientName,
                    AllowedScopes = testClient.AllowedScopes,
                    AllowedGrantTypes = testClient.AllowedGrantTypes,
                    RedirectUris = testClient.RedirectUris,
                    PostLogoutRedirectUris = testClient.PostLogoutRedirectUris,
                    AllowedCorsOrigins = testClient.AllowedCorsOrigins,
                }.ToEntity());
            }

            var store = new ClientStore(_context, FakeLogger<ClientStore>.Create());

            const int timeout = 5000;
            var task = Task.Run(() => store.FindClientByIdAsync(testClient.ClientId));

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                var client = task.Result;
                client.Should().BeEquivalentTo(testClient);
            }
            else
            {
                throw new TestTimeoutException(timeout);
            }
        }
    }
}