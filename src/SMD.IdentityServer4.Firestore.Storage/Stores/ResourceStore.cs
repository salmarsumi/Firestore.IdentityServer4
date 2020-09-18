using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Firestore.Stores
{
    /// <summary>
    /// Implementation of IResourceStore thats uses Firestore.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IConfigurationContext Context;
        
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ResourceStore> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ResourceStore(IConfigurationContext context, ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames is null) throw new ArgumentNullException(nameof(apiResourceNames));

            // firestore wherein can only filter by 10 at a time
            var list = new List<string>(apiResourceNames);
            var result = new List<ApiResource>();
            var filter = list.Take(10).ToArray();
            QuerySnapshot snapshots;

            while(filter.Length != 0)
            {
                snapshots = await Context.ApiResources
                    .WhereIn("Name", filter)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                if(snapshots.Count != 0)
                {
                    result.AddRange(snapshots.Select(x => x.ConvertTo<Entities.ApiResource>().ToModel()));
                }

                list.RemoveRange(0, list.Count >= 10 ? 10 : list.Count);
                filter = list.Take(10).ToArray();
            }

            if (result.Count != 0)
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }

            return result;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames is null) throw new ArgumentNullException(nameof(scopeNames));

            // firestore WhereArrayContainsAny can only filter by 10 at a time
            var list = new List<string>(scopeNames);
            var result = new List<ApiResource>();
            var filter = list.Take(10).ToArray();
            QuerySnapshot snapshots;

            while (filter.Length != 0)
            {
                snapshots = await Context.ApiResources
                    .WhereArrayContainsAny("Scopes", filter)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                if (snapshots.Count != 0)
                {
                    result.AddRange(snapshots.Select(x => x.ConvertTo<Entities.ApiResource>().ToModel()));
                }

                list.RemoveRange(0, list.Count >= 10 ? 10 : list.Count);
                filter = list.Take(10).ToArray();
            }

            Logger.LogDebug("Found {apis} API resources in database", result.Select(x => x.Name));

            return result;
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames is null) throw new ArgumentNullException(nameof(scopeNames));

            // firestore wherein can only filter by 10 at a time
            var list = new List<string>(scopeNames);
            var result = new List<IdentityResource>();
            var filter = list.Take(10).ToArray();
            QuerySnapshot snapshots;

            while(filter.Length != 0)
            {
                snapshots = await Context.IdentityResources
                    .WhereIn("Name", filter)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                if(snapshots.Count != 0)
                {
                    result.AddRange(snapshots.Select(x => x.ConvertTo<Entities.IdentityResource>().ToModel()));
                }

                list.RemoveRange(0, list.Count >= 10 ? 10 : list.Count);
                filter = list.Take(10).ToArray();
            }

            Logger.LogDebug("Found {scopes} identity scopes in database", result.Select(x => x.Name));

            return result;
        }

        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames is null) throw new ArgumentNullException(nameof(scopeNames));

            // firestore wherein can only filter by 10 at a time
            var list = new List<string>(scopeNames);
            var result = new List<ApiScope>();
            var filter = list.Take(10).ToArray();
            QuerySnapshot snapshots;

            while (filter.Length != 0)
            {
                snapshots = await Context.ApiScopes
                    .WhereIn("Name", filter)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                if (snapshots.Count != 0)
                {
                    result.AddRange(snapshots.Select(x => x.ConvertTo<Entities.ApiScope>().ToModel()));
                }

                list.RemoveRange(0, list.Count >= 10 ? 10 : list.Count);
                filter = list.Take(10).ToArray();
            }

            Logger.LogDebug("Found {scopes} scopes in database", result.Select(x => x.Name));

            return result;
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = await Context.IdentityResources.GetSnapshotAsync().ConfigureAwait(false);
            var apis = await Context.ApiResources.GetSnapshotAsync().ConfigureAwait(false);
            var scopes = await Context.ApiScopes.GetSnapshotAsync().ConfigureAwait(false);

            var result = new Resources(
                identity.Select(x => x.ConvertTo<Entities.IdentityResource>().ToModel()),
                apis.Select(x => x.ConvertTo<Entities.ApiResource>().ToModel()),
                scopes.Select(x => x.ConvertTo<Entities.ApiScope>().ToModel()));

            Logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources", 
                result.IdentityResources.Select(x=>x.Name).Union(result.ApiScopes.Select(x=>x.Name)),
                result.ApiResources.Select(x=>x.Name));

            return result;
        }
    }
}