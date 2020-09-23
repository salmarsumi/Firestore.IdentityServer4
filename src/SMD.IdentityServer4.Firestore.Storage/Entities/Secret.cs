using Google.Cloud.Firestore;
using System;

namespace IdentityServer4.Firestore.Entities
{
    [FirestoreData]
    public class Secret
    {
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public string Value { get; set; }
        [FirestoreProperty]
        public DateTime? Expiration { get; set; }
        [FirestoreProperty]
        public string Type { get; set; } = "SharedSecret";
        [FirestoreProperty]
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}