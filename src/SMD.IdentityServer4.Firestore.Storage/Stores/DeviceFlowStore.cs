using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using IdentityModel;
using IdentityServer4.Firestore.Entities;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Stores
{
    /// <summary>
    /// Implementation of IDeviceFlowStore thats uses Firestore.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IDeviceFlowStore" />
    public class DeviceFlowStore : IDeviceFlowStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IPersistedGrantContext Context;

        /// <summary>
        ///  The serializer.
        /// </summary>
        protected readonly IPersistentGrantSerializer Serializer;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceFlowStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serializer">The serializer</param>
        /// <param name="logger">The logger.</param>
        public DeviceFlowStore(
            IPersistedGrantContext context, 
            IPersistentGrantSerializer serializer, 
            ILogger<DeviceFlowStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Serializer = serializer;
            Logger = logger;
        }

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public virtual async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            await Context.DeviceFlowCodes
                .AddAsync(ToEntity(data, deviceCode, userCode))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <returns></returns>
        public virtual async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var snapshots = await Context.DeviceFlowCodes
                .WhereEqualTo("UserCode", userCode)
                .Limit(1)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if(snapshots.Count == 0)
            {
                return default;
            }

            var model = ToModel(snapshots[0].ConvertTo<DeviceFlowCodes>().Data);
            Logger.LogDebug("{userCode} found in database: {userCodeFound}", userCode, model != null);

            return model;
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public virtual async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var snapshots = await Context.DeviceFlowCodes
                .WhereEqualTo("DeviceCode", deviceCode)
                .Limit(1)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if (snapshots.Count == 0)
            {
                return default;
            }

            var model = ToModel(snapshots[0].ConvertTo<DeviceFlowCodes>().Data);
            Logger.LogDebug("{deviceCode} found in database: {deviceCodeFound}", deviceCode, model != null);

            return model;
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public virtual async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var snapshots = await Context.DeviceFlowCodes
                .WhereEqualTo("UserCode", userCode)
                .Limit(1)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if(snapshots.Count == 0)
            {
                Logger.LogError("{userCode} not found in database", userCode);
                throw new InvalidOperationException("Could not update device code");
            }

            var docRef = snapshots[0].Reference;
            var existing = snapshots[0].ConvertTo<DeviceFlowCodes>();

            var entity = ToEntity(data, existing.DeviceCode, userCode);
            Logger.LogDebug("{userCode} found in database", userCode);

            existing.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
            existing.Data = entity.Data;

            await docRef.SetAsync(existing, SetOptions.Overwrite).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public virtual async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var snapshots = await Context.DeviceFlowCodes
                .WhereEqualTo("DeviceCode", deviceCode)
                .Limit(1)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if(snapshots.Count > 0)
            {
                Logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                var docRef = snapshots[0].Reference;
                await docRef.DeleteAsync();
            }
            else
            {
                Logger.LogDebug("no {deviceCode} device code found in database", deviceCode);
            }
        }

        /// <summary>
        /// Converts a model to an entity.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deviceCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        protected DeviceFlowCodes ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new DeviceFlowCodes
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = Serializer.Serialize(model)
            };
        }

        /// <summary>
        /// Converts a serialized DeviceCode to a model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected DeviceCode ToModel(string entity)
        {
            if (entity == null) return null;

            return Serializer.Deserialize<DeviceCode>(entity);
        }
    }
}