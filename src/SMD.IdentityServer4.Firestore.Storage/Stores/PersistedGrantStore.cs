using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System;
using IdentityServer4.Extensions;
using IdentityServer4.Firestore.Interfaces;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Storage.Mappers;

namespace IdentityServer4.Firestore.Stores
{
    /// <summary>
    /// Implementation of IPersistedGrantStore thats uses Firestore.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IPersistedGrantStore" />
    public class PersistedGrantStore : IPersistedGrantStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IPersistedGrantContext Context;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public PersistedGrantStore(IPersistedGrantContext context, ILogger<PersistedGrantStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }

        /// <inheritdoc/>
        public virtual async Task StoreAsync(PersistedGrant token)
        {
            var snapshot = await Context.PersistedGrants.Document(token.Key)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            DocumentReference docRef = snapshot.Reference;
            Entities.PersistedGrant entity;

            if (!snapshot.Exists)
            {
                Logger.LogDebug("{persistedGrantKey} not found in database", token.Key);
                entity = token.ToEntity();
            }
            else
            {
                Logger.LogDebug("{persistedGrantKey} found in database", token.Key);
                entity = snapshot.ConvertTo<Entities.PersistedGrant>();
                token.UpdateEntity(entity);
            }

            await docRef.SetAsync(entity).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<PersistedGrant> GetAsync(string key)
        {
            var snapshot = await Context.PersistedGrants.Document(key)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            Logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, snapshot.Exists);

            if (!snapshot.Exists) return default;

            var persistedGrant = snapshot.ConvertTo<Entities.PersistedGrant>();
            return persistedGrant.ToModel();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var snapshots = await Filter(filter)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if (snapshots.Count == 0) return new List<PersistedGrant>();

            Logger.LogDebug("{persistedGrantCount} persisted grants found for {@filter}", snapshots.Count, filter);
            var model = snapshots.Select(x => x.ConvertTo<Entities.PersistedGrant>().ToModel()).ToList();

            return model;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync(string key)
        {
            var docRef = Context.PersistedGrants.Document(key);
            var snapshot = await docRef.GetSnapshotAsync().ConfigureAwait(false);

            if(snapshot.Exists)
            {
                Logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);
                await docRef.DeleteAsync().ConfigureAwait(false);
            }
            else
            {
                Logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            // firestore batch has a 500 document limit
            // delete the documents in batches of 100
            var snapshots = await Filter(filter)
                .Limit(100)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            WriteBatch batch;

            while (snapshots.Count != 0)
            {
                // using a batch will reduce the required server round trips
                // compared to calling delete for individual documents
                batch = Context.StartBatch();
                foreach(var doc in snapshots)
                {
                    batch.Delete(doc.Reference);
                }
                await batch.CommitAsync().ConfigureAwait(false);

                snapshots = await Filter(filter)
                    .Limit(100)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);
            }
        }


        private Query Filter(PersistedGrantFilter filter)
        {
            Query query = default;

            if (!String.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query?.WhereEqualTo("ClientId", filter.ClientId) ?? Context.PersistedGrants.WhereEqualTo("ClientId", filter.ClientId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query?.WhereEqualTo("SessionId", filter.SessionId) ?? Context.PersistedGrants.WhereEqualTo("SessionId", filter.SessionId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query?.WhereEqualTo("SubjectId", filter.SubjectId) ?? Context.PersistedGrants.WhereEqualTo("SubjectId", filter.SubjectId);
            }
            if (!String.IsNullOrWhiteSpace(filter.Type))
            {
                query = query?.WhereEqualTo("Type", filter.Type) ?? Context.PersistedGrants.WhereEqualTo("Type", filter.Type);
            }

            return query;
        }
    }
}