using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Firestore.Mappers
{
    public static class ApiResourceMappers
    {
        public static ApiResource ToModel(this Entities.ApiResource entity)
        {
            if (entity is null) return default;

            var list = new HashSet<string>();
            var sourceMember = entity.AllowedAccessTokenSigningAlgorithms;
            if (!string.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                {
                    list.Add(item);
                }
            };

            return new ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = list,
                ApiSecrets = entity.Secrets?.Select(s => new Secret
                {
                    Value = s.Value,
                    Type = s.Type,
                    Description = s.Description,
                    Expiration = s.Expiration
                }).ToList(),
                Description = entity.Description,
                DisplayName = entity.DisplayName,
                Enabled = entity.Enabled,
                Name = entity.Name,
                Properties = entity.Properties,
                Scopes = entity.Scopes,
                ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
                UserClaims = entity.UserClaims
            };
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ApiResource ToEntity(this ApiResource model)
        {
            return model is null ? default : new Entities.ApiResource
            {
                AllowedAccessTokenSigningAlgorithms = string.Join(",", model.AllowedAccessTokenSigningAlgorithms ?? new List<string>()),
                Secrets = model.ApiSecrets?.Select(s => new Entities.Secret
                {
                    Value = s.Value,
                    Type = s.Type,
                    Description = s.Description,
                    Expiration = s.Expiration
                }).ToList(),
                Description = model.Description,
                DisplayName = model.DisplayName,
                Enabled = model.Enabled,
                Name = model.Name,
                Properties = model.Properties,
                Scopes = model.Scopes?.ToList(),
                ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
                UserClaims = model.UserClaims?.ToList()
            };
        }
    }
}
