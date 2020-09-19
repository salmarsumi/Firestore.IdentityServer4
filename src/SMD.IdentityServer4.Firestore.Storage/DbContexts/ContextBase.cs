using Google.Cloud.Firestore;
using System;

namespace IdentityServer4.Firestore.Storage.DbContexts
{
    public abstract class ContextBase
    {
        protected readonly FirestoreDb _db;

        protected ContextBase(FirestoreDb db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
    }
}
