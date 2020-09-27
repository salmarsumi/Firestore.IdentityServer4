using Google.Api.Gax;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Options;
using IdentityServer4.Firestore.Storage.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4.Firestore.Storage
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IServiceCollection AddFirestoreDb(
            this IServiceCollection services, 
            Action<FirestoreOptions> storeOptionsAction)
        {
            var options = new FirestoreOptions();
            storeOptionsAction.Invoke(options);

            services.AddSingleton(provider =>
            {
                return new FirestoreDbBuilder
                {
                    ProjectId = options.ProjectId,
                    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                }.Build();
            });

            return services;
        }

        public static IServiceCollection AddConfigurationDbContext(this IServiceCollection services)
        {
            services.AddConfigurationDbContext<ConfigurationContext>();
            return services;
        }

        public static IServiceCollection AddConfigurationDbContext<TContext>(this IServiceCollection services)
            where TContext : class, IConfigurationContext
        {
            services.AddScoped<IConfigurationContext, TContext>();
            return services;
        }

        public static IServiceCollection AddOperationalDbContext(
            this IServiceCollection services, 
            Action<OperationalStoreOptions> options = null)
        {
            services.AddOperationalDbContext<PersistedGrantContext>(options);
            return services;
        }

        public static IServiceCollection AddOperationalDbContext<TContext>(
            this IServiceCollection services, 
            Action<OperationalStoreOptions> options = null)
            where TContext : class, IPersistedGrantContext
        {
            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            options?.Invoke(storeOptions);

            services.AddScoped<IPersistedGrantContext, TContext>();
            services.AddTransient<TokenCleanupService>();

            return services;
        }

        public static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services)
           where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }
    }
}
