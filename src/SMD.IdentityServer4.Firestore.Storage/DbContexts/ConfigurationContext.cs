using Google.Cloud.Firestore;
using IdentityServer4.Firestore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMD.IdentityServer4.Firestore.Storage.DbContexts
{
    public class ConfigurationContext : ContextBase, IConfigurationContext
    {
        public ConfigurationContext(FirestoreDb db) : base(db)
        {
        }

        public CollectionReference Clients => _db.Collection("IS-Clients");

        public CollectionReference ClientCorsOrigins => _db.Collection("IS-ClientCorsOrigins");

        public CollectionReference IdentityResources => _db.Collection("IS-IdentityResources");

        public CollectionReference ApiResources => _db.Collection("IS-ApiResources");

        public CollectionReference ApiScopes => _db.Collection("IS-ApiScopes");
    }
}
