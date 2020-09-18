using Google.Cloud.Firestore;

namespace IdentityServer4.Firestore.Interfaces
{
    public interface IPersistedGrantContext
    {
        CollectionReference PersistedGrants { get; }
        CollectionReference DeviceFlowCodes { get; }
        WriteBatch StartBatch();
    }
}