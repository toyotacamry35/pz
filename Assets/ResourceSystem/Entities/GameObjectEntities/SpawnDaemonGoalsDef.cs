using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.RubiconAI;
using GeneratedCode.Custom.Config;
using L10n;
using Newtonsoft.Json;
using ProtoBuf;
using ResourceSystem.MapSystem;
using Scripting;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using ResourceSystem.ContentKeys;

namespace SharedCode.Entities.GameObjectEntities
{
    public class SpawnDaemonGoalsDef : BaseResource
    {
        public Dictionary<string, SpawnObjectDescription> SpawnedObjects { get; set; }
        public float SecondsToFulfill { get; set; }
        public bool TurnOff { get; set; } = false;
        public bool OffsetSpawnedStatics { get; set; } = false;
    }



    public class SceneChunkDef : BaseResource, ISaveableResource
    {
        public List<ResourceRef<ScenicEntityDef>> Entities { get; set; } = new List<ResourceRef<ScenicEntityDef>>();
        public List<ResourceRef<SpawnTemplatesMapDef>> SpawnTemplates { get; set; } = new List<ResourceRef<SpawnTemplatesMapDef>>();
        public List<SpawnPointData> PlayerSpawnPoints { get; set; } = new List<SpawnPointData>();
        public Guid Id { get; set; }
    }

    public class StoryDef : BaseResource
    {
        public ContentKeyRequirement ContentKey { get; set; }
        public ResourceRef<PredicateDef> PredicateToRun { get; set; }
        public ResourceRef<ScriptingContextDef> ContextOnStart { get; set; }
        public int MaxAtTheSameTime { get; set; }
        public ResourceRef<EventInstanceDef> Event { get; set; }
    }
    public class StoriesPackDef : BaseResource
    {
        public List<ResourceRef<StoryDef>> Stories { get; set; } = new List<ResourceRef<StoryDef>>();
        public int MaxAtTheSameTime { get; set; }
    }
    public class StorytellerDef : BaseResource, IEntityObjectDef
    {
        public ContentKeyRequirement ContentKey { get; set; }
        public List<ResourceRef<StoriesPackDef>> StoryPacks { get; set; }
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public Guid Id { get; set; }
    }

    public class EventPhaseDef : SaveableBaseResource
    {
        public List<ResourceRef<SceneChunkDef>> SceneChunks { get; set; }
        public List<BuffDef> Buffs { get; set; }
    }
    public class EventInstanceDef : SaveableBaseResource, IEntityObjectDef
    {
        public List<ResourceRef<EventPhaseDef>> Phases { get; set; } = new List<ResourceRef<EventPhaseDef>>();
        public ResourceRef<BuffDef> ControlBuff { get; set; }
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }
    public class EventPointDef : BaseResource, IEntityObjectDef
    {   public bool SelfRegistry { get; }
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public Guid Id { get; set; }
    }

    public class SceneEntityDef : BaseResource, IEntityObjectDef
    {
        public bool SelfRegistry { get; }
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public Guid Id { get; set; }
    }

    public class ScenicEntityDef : BaseResource
    {
        public ContentKeyRequirement ContentKey { get; set; }
        public ResourceRef<SpawnDaemonSceneDef> SpawnDaemonSceneDef { get; set; }
        public ResourceRef<IEntityObjectDef> Object { get; set; }
        public Guid RefId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float TimeToRespawn { get; set; }
        public ResourceRef<BaseResource> JdbLocator { get; set; }
        public Dictionary<ResourceRef<LinkTypeDef>, List<Guid>> LinksToStatics { get; set; } = new Dictionary<ResourceRef<LinkTypeDef>, List<Guid>>();
    }
    [ProtoContract]
    [KnownToGameResources]
    public struct SpawnPointData
    {
        [ProtoMember(1)] public Vector3 Position { get; set; }
        [ProtoMember(2)] public Quaternion Rotation { get; set; }
        [ProtoMember(3)] public string Name { get; set; }
        [ProtoMember(4)] public float SpawnRadius { get; set; }
        [JsonIgnore]
        [ProtoMember(5)] public SpawnPointTypeDef SpawnPointType { get; set; }
        [ProtoMember(6)] public bool Invalid { get; set; }
        public ResourceRef<SpawnPointTypeDef> SpawnPointTypeRef { get; set; }

        public SpawnPointData(
            Vector3 pos,
            Quaternion rot,
            string name,
            float spawnRadius,
            SpawnPointTypeDef pointType,
            bool invalid = false
        )
        {
            Position = pos;
            Rotation = rot;
            Name = name;
            SpawnRadius = spawnRadius;
            SpawnPointType = pointType;
            Invalid = invalid;
            SpawnPointTypeRef = new ResourceRef<SpawnPointTypeDef>(pointType);
        }

        public static SpawnPointData InvalidData = new SpawnPointData(Vector3.Default, Quaternion.identity, null, 0f, null, true);

        public bool IsValid => !Invalid;

        public PositionRotation ToPositionRotation() => new PositionRotation(Position, Rotation, Invalid);
    }

    [ProtoContract]
    public struct PositionRotation
    {
        [ProtoMember(1)] public Vector3 Position;
        [ProtoMember(2)] public Quaternion Rotation;
        [ProtoMember(3)] public bool Invalid;

        [ProtoIgnore]
        public static PositionRotation InvalidInstatnce = new PositionRotation(Vector3.zero, Quaternion.identity, true);
        public bool IsValid => !Invalid;

        public PositionRotation(Vector3 pos, Quaternion rot, bool invalid = false)
        {
            Position = pos;
            Rotation = rot;
            Invalid = invalid;
        }
    }
    [KnownToGameResources]
    public struct SavedOuterRef
    {
        public Guid Guid { get; set; }
        public ResourceRef<IEntityObjectDef> ObjectType { get; set; }
    }

    [KnownToGameResources]
    public class KnowledgeToPOIs : Dictionary<ResourceRef<KnowledgeCategoryDef>, List<SavedOuterRef>> { }
    public class SpawnDaemonDef : BaseResource, IEntityObjectDef
    {
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public Guid Id { get; set; }
    }
    public class SpawnDaemonSceneDef : BaseResource
    {
        public string Name { get; set; }
        public Guid SpawnDaemonId { get; set; }
        public ResourceRef<SpawnDaemonGoalsDef> Goals { get; set; }
        public string Filter { get; set; }
        public string[] Filters { get; set; }
        public Vector3[] MobsSpawnPoints { get; set; } = Array.Empty<Vector3>();
        public KnowledgeToPOIs POIs { get; set; } = new KnowledgeToPOIs();
        public UnityRef<UnityEngine.GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
    }
}
