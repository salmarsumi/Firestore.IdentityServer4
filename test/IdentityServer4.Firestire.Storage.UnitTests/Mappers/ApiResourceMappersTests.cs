using FluentAssertions;
using IdentityServer4.Firestore.Mappers;
using System.Linq;
using Xunit;
using ApiResource = IdentityServer4.Models.ApiResource;

namespace IdentityServer4.Firestore.UnitTests.Mappers
{
    public class ApiResourceMappersTests
    {
        [Fact]
        public void Can_Map()
        {
            var model = new ApiResource();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }

        [Fact]
        public void Properties_Map()
        {
            var model = new ApiResource()
            {
               Description = "description",
               DisplayName = "displayname",
               Name = "foo",
               Scopes = { "foo1", "foo2" },
               Enabled = false
            };


            var mappedEntity = model.ToEntity();

            mappedEntity.Scopes.Count.Should().Be(2);
            var foo1 = mappedEntity.Scopes.FirstOrDefault(x => x == "foo1");
            foo1.Should().NotBeNull();
            var foo2 = mappedEntity.Scopes.FirstOrDefault(x => x == "foo2");
            foo2.Should().NotBeNull();
            

            var mappedModel = mappedEntity.ToModel();
            
            mappedModel.Description.Should().Be("description");
            mappedModel.DisplayName.Should().Be("displayname");
            mappedModel.Enabled.Should().BeFalse();
            mappedModel.Name.Should().Be("foo");
        }

        [Fact]
        public void Missing_values_should_use_defaults()
        {
            var entity = new IdentityServer4.Firestore.Entities.ApiResource
            {
                Secrets = new System.Collections.Generic.List<Entities.Secret>
                {
                    new Entities.Secret
                    {
                    }
                }
            };

            var def = new ApiResource
            {
                ApiSecrets = { new Models.Secret("foo") }
            };

            var model = entity.ToModel();
            model.ApiSecrets.First().Type.Should().Be(def.ApiSecrets.First().Type);
        }
    }
}