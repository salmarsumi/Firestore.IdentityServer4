using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer4.Firestore.Mappers
{
    public static class ClientMappers
    {
        public static Client ToModel(this Entities.Client entity)
        {
            if (entity is null) return default;

            var list = new HashSet<string>();
            var sourceMember = entity.AllowedIdentityTokenSigningAlgorithms;
            if (!string.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                {
                    list.Add(item);
                }
            }

            return new Client
            {
                AbsoluteRefreshTokenLifetime = entity.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = entity.AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)entity.AccessTokenType,
                AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = entity.AllowedCorsOrigins,
                AllowedGrantTypes = entity.AllowedGrantTypes,
                AllowedIdentityTokenSigningAlgorithms = list,
                AllowedScopes = entity.AllowedScopes,
                AllowOfflineAccess = entity.AllowOfflineAccess,
                AllowPlainTextPkce = entity.AllowPlainTextPkce,
                AllowRememberConsent = entity.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = entity.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = entity.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = entity.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = entity.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = entity.BackChannelLogoutUri,
                Claims = entity.Claims?.Select(c => new ClientClaim(c.Type, c.Value, ClaimValueTypes.String)).ToList(),
                ClientClaimsPrefix = entity.ClientClaimsPrefix,
                ClientId = entity.ClientId,
                ClientName = entity.ClientName,
                ClientSecrets = entity.ClientSecrets?.Select(s => new Secret 
                { 
                    Value = s.Value, 
                    Type = s.Type, 
                    Description = s.Description,
                    Expiration = s.Expiration
                }).ToList(),
                ClientUri = entity.ClientUri,
                ConsentLifetime = entity.ConsentLifetime,
                Description = entity.Description,
                DeviceCodeLifetime = entity.DeviceCodeLifetime,
                Enabled = entity.Enabled,
                EnableLocalLogin = entity.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = entity.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
                IdentityProviderRestrictions = entity.IdentityProviderRestrictions,
                IdentityTokenLifetime = entity.IdentityTokenLifetime,
                IncludeJwtId = entity.IncludeJwtId,
                LogoUri = entity.LogoUri,
                PairWiseSubjectSalt = entity.PairWiseSubjectSalt,
                PostLogoutRedirectUris = entity.PostLogoutRedirectUris,
                Properties = entity.Properties,
                ProtocolType = entity.ProtocolType,
                RedirectUris = entity.RedirectUris,
                RefreshTokenExpiration = (TokenExpiration)entity.RefreshTokenExpiration,
                RefreshTokenUsage = (TokenUsage)entity.RefreshTokenUsage,
                RequireClientSecret = entity.RequireClientSecret,
                RequireConsent = entity.RequireConsent,
                RequirePkce = entity.RequirePkce,
                RequireRequestObject = entity.RequireRequestObject,
                SlidingRefreshTokenLifetime = entity.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = entity.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = entity.UserCodeType,
                UserSsoLifetime = entity.UserSsoLifetime
            };
        }

        public static Entities.Client ToEntity(this Client model)
        {
            if (model is null) return default;

            return new Entities.Client
            {
                AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = model.AccessTokenLifetime,
                AccessTokenType = (int)model.AccessTokenType,
                AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = model.AllowedCorsOrigins?.ToList(),
                AllowedGrantTypes = model.AllowedGrantTypes?.ToList(),
                AllowedIdentityTokenSigningAlgorithms = string.Join(",", model.AllowedIdentityTokenSigningAlgorithms ?? new List<string>()),
                AllowedScopes = model.AllowedScopes?.ToList(),
                AllowOfflineAccess = model.AllowOfflineAccess,
                AllowPlainTextPkce = model.AllowPlainTextPkce,
                AllowRememberConsent = model.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = model.BackChannelLogoutUri,
                Claims = model.Claims?.Select(c => new Entities.ClientClaim { Type = c.Type, Value = c.Value }).ToList(),
                ClientClaimsPrefix = model.ClientClaimsPrefix,
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                ClientSecrets = model.ClientSecrets?.Select(s => new Entities.Secret
                {
                    Value = s.Value,
                    Type = s.Type,
                    Description = s.Description,
                    Expiration = s.Expiration
                }).ToList(),
                ClientUri = model.ClientUri,
                ConsentLifetime = model.ConsentLifetime,
                Description = model.Description,
                DeviceCodeLifetime = model.DeviceCodeLifetime,
                Enabled = model.Enabled,
                EnableLocalLogin = model.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = model.FrontChannelLogoutUri,
                IdentityProviderRestrictions = model.IdentityProviderRestrictions?.ToList(),
                IdentityTokenLifetime = model.IdentityTokenLifetime,
                IncludeJwtId = model.IncludeJwtId,
                LogoUri = model.LogoUri,
                PairWiseSubjectSalt = model.PairWiseSubjectSalt,
                PostLogoutRedirectUris = model.PostLogoutRedirectUris?.ToList(),
                Properties = model.Properties as Dictionary<string, string>,
                ProtocolType = model.ProtocolType,
                RedirectUris = model.RedirectUris?.ToList(),
                RefreshTokenExpiration = (int)model.RefreshTokenExpiration,
                RefreshTokenUsage = (int)model.RefreshTokenUsage,
                RequireClientSecret = model.RequireClientSecret,
                RequireConsent = model.RequireConsent,
                RequirePkce = model.RequirePkce,
                RequireRequestObject = model.RequireRequestObject,
                SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = model.UserCodeType,
                UserSsoLifetime = model.UserSsoLifetime
            };
        }
    }
}
