using Google.Api.Gax;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Options;
using IdentityServer4.Firestore.Storage.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4.Firestore.Storage
{
    /// <summary>
    /// Extension methods to add Firestore support to IdentityServer.
    /// </summary>
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddConfigurationDbContext(this IServiceCollection services, Action<FirestoreOptions> storeOptionsAction)
        {
            var options = new FirestoreOptions();
            storeOptionsAction.Invoke(options);

            services.AddScoped<IConfigurationContext, ConfigurationContext>(provider =>
            {
                return new ConfigurationContext(new FirestoreDbBuilder
                {
                    ProjectId = options.ProjectId,
                    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                }.Build());
            });

            return services;
        }

        /// <summary>
        /// Adds operational DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalDbContext(this IServiceCollection services, Action<FirestoreOptions> storeOptionsAction)
        {
            services.AddScoped<IPersistedGrantContext, PersistedGrantContext>(provider =>
            {
                var options = new FirestoreOptions();
                storeOptionsAction.Invoke(options);

                return new PersistedGrantContext(new FirestoreDbBuilder
                {
                    ProjectId = options.ProjectId,
                    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                }.Build());
            });
            services.AddTransient<TokenCleanupService>();

            return services;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to the DI system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services)
           where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }
    }
}
