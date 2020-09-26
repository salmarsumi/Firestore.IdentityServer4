using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Firestore.Mappers;
using IdentityServer4.Firestore.Storage.DbContexts;
using IdentityServer4.Firestore.Stores;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Firestore.IntegrationTests.Stores
{
    [Collection("FirestoreTests")]
    public class ScopeStoreTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;
        private readonly ConfigurationContext _context;

        public ScopeStoreTests(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
            _context = new ConfigurationContext(_fixture.DB);
        }

        private static IdentityResource CreateIdentityTestResource()
        {
            return new IdentityResource()
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims = 
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateApiResourceTestResource()
        {
            return new ApiResource()
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = { Guid.NewGuid().ToString() },
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }
        
        private static ApiScope CreateApiScopeTestResource()
        {
            return new ApiScope()
            {
                Name = Guid.NewGuid().ToString(),
                UserClaims =
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }


        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateApiResourceTestResource();

            await _context.ApiResources.AddAsync(resource.ToEntity());

            ApiResource foundResource;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            foundResource = (await store.FindApiResourcesByNameAsync(new[] { resource.Name })).SingleOrDefault();

            Assert.NotNull(foundResource);
            Assert.True(foundResource.Name == resource.Name);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
        }

        [Fact]
        public async Task FindApiResourcesByNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            var resource = CreateApiResourceTestResource();

            await _context.ApiResources.AddAsync(resource.ToEntity());
            await _context.ApiResources.AddAsync(CreateApiResourceTestResource().ToEntity());

            ApiResource foundResource;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            foundResource = (await store.FindApiResourcesByNameAsync(new[] { resource.Name })).SingleOrDefault();

            Assert.NotNull(foundResource);
            Assert.True(foundResource.Name == resource.Name);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectResourcesReturned()
        {
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            await _context.ApiResources.AddAsync(testApiResource.ToEntity());
            await _context.ApiScopes.AddAsync(testApiScope.ToEntity());

            IEnumerable<ApiResource> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = await store.FindApiResourcesByScopeNameAsync(new List<string>
                {
                    testApiScope.Name
                });

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }

        [Fact]
        public async Task FindApiResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned()
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiResourceTestResource();
            var testApiScope = CreateApiScopeTestResource();
            testApiResource.Scopes.Add(testApiScope.Name);

            await _context.IdentityResources.AddAsync(testIdentityResource.ToEntity());
            await _context.ApiResources.AddAsync(testApiResource.ToEntity());
            await _context.ApiScopes.AddAsync(testApiScope.ToEntity());
            await _context.IdentityResources.AddAsync(CreateIdentityTestResource().ToEntity());
            await _context.ApiResources.AddAsync(CreateApiResourceTestResource().ToEntity());
            await _context.ApiScopes.AddAsync(CreateApiScopeTestResource().ToEntity());

            IEnumerable<ApiResource> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = await store.FindApiResourcesByScopeNameAsync(new[] { testApiScope.Name });

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == testApiResource.Name));
        }




        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateIdentityTestResource();

            await _context.IdentityResources.AddAsync(resource.ToEntity());

            IList<IdentityResource> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindIdentityResourcesByScopeNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            var resource = CreateIdentityTestResource();

            await _context.IdentityResources.AddAsync(resource.ToEntity());
            await _context.IdentityResources.AddAsync(CreateIdentityTestResource().ToEntity());

            IList<IdentityResource> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = (await store.FindIdentityResourcesByScopeNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned()
        {
            var resource = CreateApiScopeTestResource();

            await _context.ApiScopes.AddAsync(resource.ToEntity());

            IList<ApiScope> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = (await store.FindApiScopesByNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Fact]
        public async Task FindApiScopesByNameAsync_WhenResourcesExist_ExpectOnlyRequestedReturned()
        {
            var resource = CreateApiScopeTestResource();

            await _context.ApiScopes.AddAsync(resource.ToEntity());
            await _context.ApiScopes.AddAsync(CreateApiScopeTestResource().ToEntity());

            IList<ApiScope> resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = (await store.FindApiScopesByNameAsync(new List<string>
                {
                    resource.Name
                })).ToList();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.NotNull(resources.Single(x => x.Name == resource.Name));
        }




        [Fact]
        public async Task GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden()
        {
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiResourceTestResource();
            var visibleApiScope = CreateApiScopeTestResource();
            var hiddenIdentityResource = new IdentityResource { Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false };
            var hiddenApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = { Guid.NewGuid().ToString() },
                ShowInDiscoveryDocument = false
            };
            var hiddenApiScope = new ApiScope
            {
                Name = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = false
            };

            await _context.IdentityResources.AddAsync(visibleIdentityResource.ToEntity());
            await _context.ApiResources.AddAsync(visibleApiResource.ToEntity());
            await _context.ApiScopes.AddAsync(visibleApiScope.ToEntity());

            await _context.IdentityResources.AddAsync(hiddenIdentityResource.ToEntity());
            await _context.ApiResources.AddAsync(hiddenApiResource.ToEntity());
            await _context.ApiScopes.AddAsync(hiddenApiScope.ToEntity());

            Resources resources;
            var store = new ResourceStore(_context, FakeLogger<ResourceStore>.Create());
            resources = await store.GetAllResourcesAsync();

            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotEmpty(resources.ApiScopes);

            Assert.Contains(resources.IdentityResources, x => x.Name == visibleIdentityResource.Name);
            Assert.Contains(resources.IdentityResources, x => x.Name == hiddenIdentityResource.Name);

            Assert.Contains(resources.ApiResources, x => x.Name == visibleApiResource.Name);
            Assert.Contains(resources.ApiResources, x => x.Name == hiddenApiResource.Name);

            Assert.Contains(resources.ApiScopes, x => x.Name == visibleApiScope.Name);
            Assert.Contains(resources.ApiScopes, x => x.Name == hiddenApiScope.Name);
        }
    }
}