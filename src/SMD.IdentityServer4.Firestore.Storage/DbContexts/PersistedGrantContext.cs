using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;

namespace IdentityServer4.Firestore.Storage.DbContexts
{
    public class PersistedGrantContext : ContextBase, IPersistedGrantContext
    {
        private const string PERSISTED_GRANTS = "IS4-PersistedGrants";
        private const string DEVICE_FLOW_CODES = "IS4-DeviceFlowCodes";

        public PersistedGrantContext(FirestoreDb db) : base(db)
        {
        }

        public CollectionReference PersistedGrants => _db.Collection(PERSISTED_GRANTS);

        public CollectionReference DeviceFlowCodes => _db.Collection(DEVICE_FLOW_CODES);

        public WriteBatch StartBatch() => _db.StartBatch();
    }
}
