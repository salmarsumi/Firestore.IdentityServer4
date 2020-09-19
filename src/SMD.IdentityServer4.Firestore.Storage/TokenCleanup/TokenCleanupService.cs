// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Firestore.Options;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Firestore
{
    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly OperationalStoreOptions _options;
        private readonly IPersistedGrantContext _persistedGrantDbContext;
        private readonly IOperationalStoreNotification _operationalStoreNotification;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="persistedGrantDbContext"></param>
        /// <param name="operationalStoreNotification"></param>
        /// <param name="logger"></param>
        public TokenCleanupService(
            OperationalStoreOptions options,
            IPersistedGrantContext persistedGrantDbContext, 
            ILogger<TokenCleanupService> logger,
            IOperationalStoreNotification operationalStoreNotification = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _persistedGrantDbContext = persistedGrantDbContext ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operationalStoreNotification = operationalStoreNotification;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            var found = Int32.MaxValue;
            WriteBatch batch;
            QuerySnapshot expiredGrants;

            while (found >= _options.TokenCleanupBatchSize)
            {
                expiredGrants = await _persistedGrantDbContext.PersistedGrants
                    .WhereLessThan("Expiration", DateTime.UtcNow)
                    .Limit(_options.TokenCleanupBatchSize)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                found = expiredGrants.Count;
                _logger.LogInformation("Removing {grantCount} grants", found);

                if(found > 0)
                {
                    batch = _persistedGrantDbContext.StartBatch();
                    foreach(var doc in expiredGrants)
                    {
                        batch.Delete(doc.Reference);
                    }
                    await batch.CommitAsync();

                    if (_operationalStoreNotification != null)
                    {
                        await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants.Select(x => x.ConvertTo<Entities.PersistedGrant>()));
                    }
                }
            }
        }


        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            var found = Int32.MaxValue;
            WriteBatch batch;
            QuerySnapshot expiredCodes;

            while (found >= _options.TokenCleanupBatchSize)
            {
                expiredCodes = await _persistedGrantDbContext.DeviceFlowCodes
                    .WhereLessThan("Expiration", DateTime.UtcNow)
                    .Limit(_options.TokenCleanupBatchSize)
                    .GetSnapshotAsync()
                    .ConfigureAwait(false);

                found = expiredCodes.Count;
                _logger.LogInformation("Removing {deviceCodeCount} device flow codes", found);

                if (found > 0)
                {
                    batch = _persistedGrantDbContext.StartBatch();
                    foreach (var doc in expiredCodes)
                    {
                        batch.Delete(doc.Reference);
                    }
                    await batch.CommitAsync();

                    if (_operationalStoreNotification != null)
                    {
                        await _operationalStoreNotification.DeviceCodesRemovedAsync(expiredCodes.Select(x => x.ConvertTo<Entities.DeviceFlowCodes>()));
                    }
                }
            }
        }
    }
}