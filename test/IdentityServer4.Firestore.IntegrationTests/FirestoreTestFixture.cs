using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityServer4.Firestore.IntegrationTests
{
    public class FirestoreTestFixture
    {
        private readonly FirestoreDb _db;

        private readonly IConfiguration _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<FirestoreTestFixture>()
                .AddEnvironmentVariables()
                .Build();

        public FirestoreTestFixture()
        {
            _db = CreateDbInstance();

            Clean(_db.Collection("IS4-PersistedGrants"));
            Clean(_db.Collection("IS4-DeviceFlowCodes"));
            Clean(_db.Collection("IS4-Clients"));
            Clean(_db.Collection("IS4-IdentityResources"));
            Clean(_db.Collection("IS4-ApiResources"));
            Clean(_db.Collection("IS4-ApiScopes"));
        }

        public FirestoreDb DB => _db;

        private FirestoreDb CreateDbInstance()
        {
            return new FirestoreDbBuilder
            {
                ProjectId = _config["ProjectId"],
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
