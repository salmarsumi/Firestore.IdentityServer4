using System;
using System.Collections.Generic;
using IdentityServer4.Models;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Firestore.Storage.DbContexts;
using IdentityServer4.Firestore.Mappers;
using System.Threading.Tasks;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Services;

namespace IdentityServer4.Firestore.IntegrationTests.Services
{
    public class CorsPolicyServiceTests : IClassFixture<FirestoreTestFixture>
    {
        private readonly FirestoreTestFixture _fixture;
        private readonly ConfigurationContext context;

        public CorsPolicyServiceTests(FirestoreTestFixture fixture)
        {
            _fixture = fixture;
            context = new ConfigurationContext(_fixture.DB);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrueAsync()
        {
            const string testCorsOrigin = "https://identityserver.io/";

            var client = new Client
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString(),
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
            };
            await context.Clients.Document(client.ClientId).SetAsync(client.ToEntity());
            client = new Client
            {
                ClientId = "2",
                ClientName = "2",
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com", testCorsOrigin }
            };
            await context.Clients.Document(client.ClientId).SetAsync(client.ToEntity());

            bool result;
            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddSingleton<IConfigurationContext>(context);
            ctx.RequestServices = svcs.BuildServiceProvider();
            var ctxAccessor = new HttpContextAccessor();
            ctxAccessor.HttpContext = ctx;

            var service = new CorsPolicyService(ctxAccessor, FakeLogger<CorsPolicyService>.Create());
            result = await service.IsOriginAllowedAsync(testCorsOrigin);

            Assert.True(result);
        }

        [Fact]
        public async Task IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalseAsync()
        {
            var client = new Client
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = Guid.NewGuid().ToString(),
                AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
            };
            await context.Clients.Document(client.ClientId).SetAsync(client.ToEntity());

            bool result;
            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddSingleton<IConfigurationContext>(context);
            ctx.RequestServices = svcs.BuildServiceProvider();
            var ctxAccessor = new HttpContextAccessor();
            ctxAccessor.HttpContext = ctx;

            var service = new CorsPolicyService(ctxAccessor, FakeLogger<CorsPolicyService>.Create());
            result = await service.IsOriginAllowedAsync("InvalidOrigin");

            Assert.False(result);
        }
    }
}