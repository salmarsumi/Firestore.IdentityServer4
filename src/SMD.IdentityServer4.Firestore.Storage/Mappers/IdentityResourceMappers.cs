using IdentityServer4.Firestore.Entities;
using System.Linq;

namespace IdentityServer4.Firestore.Mappers
{
    public static class IdentityResourceMappers
    {
        public static Models.IdentityResource ToModel(this IdentityResource entity)
        {
            return entity == null ? null : new Models.IdentityResource
            {
                Description = entity.Description,
                DisplayName = entity.DisplayName,
                Emphasize = entity.Emphasize,
                Enabled = entity.Enabled,
                Name = entity.Name,
                Properties = entity.Properties,
                Required = entity.Required,
                ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
                UserClaims = entity.UserClaims
            };
        }

        public static IdentityResource ToEntity(this Models.IdentityResource model)
        {
            return model == null ? null : new IdentityResource
            {
                Description = model.Description,
                DisplayName = model.DisplayName,
                Emphasize = model.Emphasize,
                Enabled = model.Enabled,
                Name = model.Name,
                Properties = model.Properties,
                Required = model.Required,
                ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
                UserClaims = model.UserClaims?.ToList()
            };
        }
    }
}
