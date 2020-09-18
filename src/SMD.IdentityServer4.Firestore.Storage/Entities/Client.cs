using Google.Cloud.Firestore;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace IdentityServer4.Firestore.Entities
{
    [FirestoreData]
    public class Client
    {
        [FirestoreProperty]
        public bool Enabled { get; set; } = true;
        
        [FirestoreProperty]
        public string ClientId { get; set; }
        
        [FirestoreProperty]
        public string ProtocolType { get; set; } = "oidc";
        
        [FirestoreProperty]
        public List<Secret> ClientSecrets { get; set; }
        
        [FirestoreProperty]
        public bool RequireClientSecret { get; set; } = true;
        
        [FirestoreProperty]
        public string ClientName { get; set; }
        
        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public string ClientUri { get; set; }

        [FirestoreProperty]
        public string LogoUri { get; set; }

        [FirestoreProperty]
        public bool RequireConsent { get; set; } = false;

        [FirestoreProperty]
        public bool AllowRememberConsent { get; set; } = true;

        [FirestoreProperty]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        [FirestoreProperty]
        public List<string> AllowedGrantTypes { get; set; }

        [FirestoreProperty]
        public bool RequirePkce { get; set; } = true;

        [FirestoreProperty]
        public bool AllowPlainTextPkce { get; set; }

        [FirestoreProperty]
        public bool RequireRequestObject { get; set; }

        [FirestoreProperty]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [FirestoreProperty]
        public List<string> RedirectUris { get; set; }

        [FirestoreProperty]
        public List<string> PostLogoutRedirectUris { get; set; }

        [FirestoreProperty]
        public string FrontChannelLogoutUri { get; set; }

        [FirestoreProperty]
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        [FirestoreProperty]
        public string BackChannelLogoutUri { get; set; }
        
        [FirestoreProperty]
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        [FirestoreProperty]
        public bool AllowOfflineAccess { get; set; }

        [FirestoreProperty]
        public List<string> AllowedScopes { get; set; }

        [FirestoreProperty]
        public int IdentityTokenLifetime { get; set; } = 300;

        [FirestoreProperty]
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }

        [FirestoreProperty]
        public int AccessTokenLifetime { get; set; } = 3600;

        [FirestoreProperty]
        public int AuthorizationCodeLifetime { get; set; } = 300;
        
        [FirestoreProperty]
        public int? ConsentLifetime { get; set; } = null;

        [FirestoreProperty]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        [FirestoreProperty]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        [FirestoreProperty]
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;

        [FirestoreProperty]
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        [FirestoreProperty]
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;

        [FirestoreProperty]
        public int AccessTokenType { get; set; } = 0; // AccessTokenType.Jwt;

        [FirestoreProperty]
        public bool EnableLocalLogin { get; set; } = true;

        [FirestoreProperty]
        public List<string> IdentityProviderRestrictions { get; set; }

        [FirestoreProperty]
        public bool IncludeJwtId { get; set; }

        [FirestoreProperty]
        public List<ClientClaim> Claims { get; set; }

        [FirestoreProperty]
        public bool AlwaysSendClientClaims { get; set; }

        [FirestoreProperty]
        public string ClientClaimsPrefix { get; set; } = "client_";

        [FirestoreProperty]
        public string PairWiseSubjectSalt { get; set; }

        [FirestoreProperty]
        public List<string> AllowedCorsOrigins { get; set; }

        [FirestoreProperty]
        public IDictionary<string, string> Properties { get; set; }

        [FirestoreProperty]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? Updated { get; set; }

        [FirestoreProperty]
        public DateTime? LastAccessed { get; set; }

        [FirestoreProperty]
        public int? UserSsoLifetime { get; set; }

        [FirestoreProperty]
        public string UserCodeType { get; set; }

        [FirestoreProperty]
        public int DeviceCodeLifetime { get; set; } = 300;

        [FirestoreProperty]
        public bool NonEditable { get; set; }
    }
}