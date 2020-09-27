using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using System;

namespace IdentityServer4.Firestore.Storage.UnitTests
{
    public class FakeConfigurationContext : IConfigurationContext
    {
        public CollectionReference Clients => throw new NotImplementedException();

        public CollectionReference IdentityResources => throw new NotImplementedException();

        public CollectionReference ApiResources => throw new NotImplementedException();

        public CollectionReference ApiScopes => throw new NotImplementedException();
    }

    public class FakePersistedGrantContext : IPersistedGrantContext
    {
        public CollectionReference PersistedGrants => throw new NotImplementedException();

        public CollectionReference DeviceFlowCodes => throw new NotImplementedException();

        public WriteBatch StartBatch()
        {
            throw new NotImplementedException();
        }
    }
}
