using IdentityServer4.Firestore.Entities;
using System.Linq;

namespace IdentityServer4.Firestore.Mappers
{
    public static class ApiScopeMapper
    {
        public static Models.ApiScope ToModel(this ApiScope entity)
        {
            return entity is null ? null : new Models.ApiScope
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

        public static ApiScope ToEntity(this Models.ApiScope model)
        {
            return model is null ? null : new ApiScope
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
