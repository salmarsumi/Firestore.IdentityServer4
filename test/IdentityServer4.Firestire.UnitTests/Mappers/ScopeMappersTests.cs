using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Firestore.Mappers;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Firestore.UnitTests.Mappers
{
    public class ScopesMappersTests
    {
        [Fact]
        public void CanMapScope()
        {
            var model = new ApiScope();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }

        [Fact]
        public void Properties_Map()
        {
            var model = new ApiScope()
            {
                Description = "description",
                DisplayName = "displayname",
                Name = "foo",
                UserClaims = { "c1", "c2" },
                Properties = {
                    { "x", "xx" },
                    { "y", "yy" },
               },
                Enabled = false
            };


            var mappedEntity = model.ToEntity();
            mappedEntity.Description.Should().Be("description");
            mappedEntity.DisplayName.Should().Be("displayname");
            mappedEntity.Name.Should().Be("foo");

            mappedEntity.UserClaims.Count.Should().Be(2);
            mappedEntity.UserClaims.Should().BeEquivalentTo(new[] { "c1", "c2" });
            mappedEntity.Properties.Count.Should().Be(2);
            var item1 = new KeyValuePair<string, string>("x", "xx");
            var item2 = new KeyValuePair<string, string>("y", "yy");
            mappedEntity.Properties.Should().Contain(item1);
            mappedEntity.Properties.Should().Contain(item2);


            var mappedModel = mappedEntity.ToModel();

            mappedModel.Description.Should().Be("description");
            mappedModel.DisplayName.Should().Be("displayname");
            mappedModel.Enabled.Should().BeFalse();
            mappedModel.Name.Should().Be("foo");
            mappedModel.UserClaims.Count.Should().Be(2);
            mappedModel.UserClaims.Should().BeEquivalentTo(new[] { "c1", "c2" });
            mappedModel.Properties.Count.Should().Be(2);
            mappedModel.Properties["x"].Should().Be("xx");
            mappedModel.Properties["y"].Should().Be("yy");
        }
    }
}