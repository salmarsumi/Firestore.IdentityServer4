﻿using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityServer4.Firestore.IntegrationTests
{
    public class FirestoreTestFixture
    {
        public FirestoreTestFixture()
        {
            DB = CreateDbInstance();

            Clean(DB.Collection("IS4-PersistedGrants"));
            Clean(DB.Collection("IS4-DeviceFlowCodes"));
            Clean(DB.Collection("IS4-Clients"));
            Clean(DB.Collection("IS4-IdentityResources"));
            Clean(DB.Collection("IS4-ApiResources"));
            Clean(DB.Collection("IS4-ApiScopes"));
        }

        public FirestoreDb DB { get; }
        public IConfiguration Configuration { get; } = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<FirestoreTestFixture>()
                .AddEnvironmentVariables()
                .Build();

        private FirestoreDb CreateDbInstance()
        {
            return new FirestoreDbBuilder
            {
                ProjectId = Configuration["ProjectId"],
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();
        }

        private void Clean(CollectionReference collection)
        {
            var snapShot = collection.GetSnapshotAsync().GetAwaiter().GetResult();
            foreach (var doc in snapShot.Documents)
            {
                doc.Reference.DeleteAsync().GetAwaiter().GetResult();
            }
        }
    }
}
