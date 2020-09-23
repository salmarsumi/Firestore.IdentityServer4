using Google.Cloud.Firestore;

namespace IdentityServer4.Firestore.Interfaces
{
    public interface IConfigurationContext
    {
        CollectionReference Clients { get; }
        CollectionReference IdentityResources { get; }
        CollectionReference ApiResources { get; }
        CollectionReference ApiScopes { get; }
    }
}