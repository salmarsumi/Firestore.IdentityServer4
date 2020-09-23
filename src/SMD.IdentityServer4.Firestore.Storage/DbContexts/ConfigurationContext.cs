using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;

namespace IdentityServer4.Firestore.Storage.DbContexts
{
    public class ConfigurationContext : ContextBase, IConfigurationContext
    {
        private const string CLIENTS = "IS4-Clients";
        private const string IDENTITY_RESOURCES = "IS4-IdentityResources";
        private const string API_RESOURCES = "IS4-ApiResources";
        private const string API_SCOPES = "IS4-ApiScopes";

        public ConfigurationContext(FirestoreDb db) : base(db)
        {
        }

        public CollectionReference Clients => _db.Collection(CLIENTS);

        public CollectionReference IdentityResources => _db.Collection(IDENTITY_RESOURCES);

        public CollectionReference ApiResources => _db.Collection(API_RESOURCES);

        public CollectionReference ApiScopes => _db.Collection(API_SCOPES);
    }
}
