using System;
using System.Collections.Generic;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using ProtoBuf;
using ResourceSystem.Utils;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Utils;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public struct OuterRef<T> : IEquatable<OuterRef<T>>
    {
        [ProtoMember(1)]
        public Guid Guid;

        [ProtoMember(2)]
        public int TypeId;

        public bool IsValid => TypeId != 0 && Guid != Guid.Empty;

        public Type Type => TypeId != 0 ? ReplicaTypeRegistry.GetTypeById(TypeId) : null;
        public static readonly OuterRef<T> Invalid = default;
        public OuterRef(SavedOuterRef sor)
        {
            Guid = sor.Guid;
            TypeId = EntitiesRepository.GetIdByType(DefToType.GetEntityType(sor.ObjectType.Target.GetType()));
        }
        public OuterRef(Guid id, int typeId)
        {
            Guid = id;
            TypeId = typeId;
#if ENABLE_OUTERREF_CHECK
            {
                var paramType = typeof(T);
                if (paramType != typeof(IEntity) && paramType != typeof(IEntityObject))
                {
                    var expectedType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(paramType);
                    var actualType = EntitiesRepositoryBase.GetTypeById(EntitiesRepositoryBase.GetMasterTypeIdByReplicationLevelType(typeId));
                    if (!expectedType.IsAssignableFrom(actualType))
                        throw new ArgumentException($"Type is {actualType} but expected {expectedType}");
                }
            }
#endif
        }

        public OuterRef(OuterRef @ref)
        {
            Guid = @ref.Guid;
            TypeId = @ref.TypeId;
        }

        public OuterRef(IEntity entity)
        {
            Guid = entity.Id;
            TypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(entity.TypeId);
        }

        public OuterRef(IEntityRef entityRef)
        {
            Guid = entityRef.Id;
            TypeId = entityRef.TypeId;
        }

        public override string ToString() => IsValid ? $"{Guid} {TypeId} {Type.Name}" : "<not valid>";

        public string ShortDebugGuid => Guid.ToString().Substring(28);

        public override bool Equals(object obj)
        {
            return obj is OuterRef<T> && Equals((OuterRef<T>)obj);
        }

        public bool Equals(OuterRef<T> other)
        {
            return TypeId == other.TypeId && Guid.Equals(other.Guid);
        }

        public override int GetHashCode()
        {
            var hashCode = -1531579387;
            hashCode = hashCode * -1521134295 + Guid.GetHashCode();
            hashCode = hashCode * -1521134295 + TypeId.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(OuterRef<T> ref1, OuterRef<T> ref2) => ref1.Equals(ref2);

        public static bool operator !=(OuterRef<T> ref1, OuterRef<T> ref2) => !(ref1 == ref2);

        [System.Diagnostics.Contracts.Pure] public OuterRef<T1> To<T1>() => new OuterRef<T1>(Guid, TypeId);

        [System.Diagnostics.Contracts.Pure] public OuterRef To() => new OuterRef(Guid, TypeId);
        
        public static implicit operator OuterRef(OuterRef<T> @ref) => new OuterRef(@ref.Guid, @ref.TypeId);
    }
}
