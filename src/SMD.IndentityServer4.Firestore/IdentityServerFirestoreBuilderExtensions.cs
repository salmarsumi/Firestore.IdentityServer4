using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Stores;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Firestore;
using IdentityServer4.Firestore.Options;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Stores;
using IdentityServer4.Firestore.Services;
using IdentityServer4.Firestore.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add Firestore database support to IdentityServer.
    /// </summary>
    public static class IdentityServerFirestoreBuilderExtensions
    {
        public static IIdentityServerBuilder AddFirestoreDb(
            this IIdentityServerBuilder builder, 
            Action<FirestoreOptions> options)
        {
            builder.Services.AddFirestoreDb(options);
            return builder;
        }

        public static IIdentityServerBuilder AddFirestoreConfigurationStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddConfigurationDbContext();

            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddFirestoreConfigurationStoreCache(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddFirestoreOperationalStore(
            this IIdentityServerBuilder builder,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddOperationalDbContext(storeOptionsAction);

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            builder.Services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStoreNotification<T>(
           this IIdentityServerBuilder builder)
           where T : class, IOperationalStoreNotification
        {
            builder.Services.AddOperationalStoreNotification<T>();
            return builder;
        }
    }
}
