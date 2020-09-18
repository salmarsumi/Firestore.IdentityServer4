using Google.Cloud.Firestore;
using System;

namespace IdentityServer4.Firestore.Entities
{
    [FirestoreData]
    public class PersistedGrant
    {
        [FirestoreProperty]
        public string Key { get; set; }

        [FirestoreProperty]
        public string Type { get; set; }

        [FirestoreProperty]
        public string SubjectId { get; set; }

        [FirestoreProperty]
        public string SessionId { get; set; }

        [FirestoreProperty]
        public string ClientId { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public DateTime CreationTime { get; set; }

        [FirestoreProperty]
        public DateTime? Expiration { get; set; }

        [FirestoreProperty]
        public DateTime? ConsumedTime { get; set; }

        [FirestoreProperty]
        public string Data { get; set; }
    }
}