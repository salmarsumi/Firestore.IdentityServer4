using IdentityServer4.Models;

namespace IdentityServer4.Firestore.Storage.Mappers
{
    public static class PersistedGrantMapper
    {
        public static PersistedGrant ToModel(this Entities.PersistedGrant entity)
        {
            return entity is null ? null : new PersistedGrant
            {
                ClientId = entity.ClientId,
                ConsumedTime = entity.ConsumedTime,
                CreationTime = entity.CreationTime,
                Data = entity.Data,
                Description = entity.Description,
                Expiration = entity.Expiration,
                Key = entity.Key,
                SessionId = entity.SessionId,
                SubjectId = entity.SubjectId,
                Type = entity.Type
            };
        }

        public static Entities.PersistedGrant ToEntity(this PersistedGrant model)
        {
            return model == null ? null : new Entities.PersistedGrant
            {
                ClientId = model.ClientId,
                ConsumedTime = model.ConsumedTime,
                CreationTime = model.CreationTime,
                Data = model.Data,
                Description = model.Description,
                Expiration = model.Expiration,
                Key = model.Key,
                SessionId = model.SessionId,
                SubjectId = model.SubjectId,
                Type = model.Type
            };
        }

        public static void UpdateEntity(this PersistedGrant model, Entities.PersistedGrant entity)
        {
            entity.ClientId = model.ClientId;
            entity.ConsumedTime = model.ConsumedTime;
            entity.CreationTime = model.CreationTime;
            entity.Data = model.Data;
            entity.Description = model.Description;
            entity.Expiration = model.Expiration;
            entity.Key = model.Key;
            entity.SessionId = model.SessionId;
            entity.SubjectId = model.SubjectId;
            entity.Type = model.Type;
        }
    }
}
