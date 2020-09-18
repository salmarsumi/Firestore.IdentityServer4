using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Firestore.Interfaces;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using IdentityServer4.Firestore.Mappers;

namespace IdentityServer4.Firestore.Stores
{
    /// <summary>
    /// Implementation of IClientStore thats uses Firestore.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IClientStore" />
    public class ClientStore : IClientStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IConfigurationContext Context;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ClientStore> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ClientStore(IConfigurationContext context, ILogger<ClientStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public virtual async Task<Client> FindClientByIdAsync(string clientId)
        {
            var snapshots = await Context.Clients
                .WhereEqualTo("ClientId", clientId)
                .Limit(1)
                .GetSnapshotAsync()
                .ConfigureAwait(false);

            if (snapshots.Count == 0)
                return null;

            return snapshots.First().ConvertTo<Entities.Client>().ToModel();
        }
    }
}