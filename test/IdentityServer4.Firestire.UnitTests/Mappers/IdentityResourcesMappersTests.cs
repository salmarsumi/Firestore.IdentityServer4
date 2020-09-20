using IdentityServer4.Firestore.Mappers;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Firestore.UnitTests.Mappers
{
    public class IdentityResourcesMappersTests
    {
        [Fact]
        public void CanMapIdentityResources()
        {
            var model = new IdentityResource();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }
    }
}