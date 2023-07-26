using Assets.ColonyShared.SharedCode.Player;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using SharedCode.Aspects.Science;
using SharedCode.Wizardry;
using System.Collections.Generic;
using ResourceSystem.Aspects.Item.Templates;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    public class WorldConstantsResource : BaseResource
    {
        public float PlayerHeight { get; set; }

        public float BoxSpawnDistance { get; set; }

        public bool EnableBelowPlayerPositionBoxSpawning { get; set; }

        public float SelectionRadius { get; set; }
        public float FogOfWarRefreshPeriod { get; set; } = 1f;
        public float BoxInteractDistance { get; set; }
        public bool IsCharacterInterestingEnoughtToLogWizardEvents { get; set; } = false;
        public bool EnableAIProfiler { get; set; } = false;
        public float BuildingInteractDistance { get; set; }
        public string ReleaseMapName { get; set; }
        public bool IsRelease { get; set; }
        public bool PersistState { get; set; } = false;
        public int PersistCharacterCooldownSeconds { get; set; } = 60 * 2;
        public int PersistWorldCooldownSeconds { get; set; } = 60 * 2;
        public float TimeSyncPingRelaxation { get; set; } = 0.8f;
        public bool SpawnMobs { get; set; } = true;
        public bool SpawnBots { get; set; } = true;
        public long StrikeTimeout { get; set; } = 5000;
        public bool DevMode { get; set; } = false;
        public bool LockCursor { get; set; } = true;
        public float DefaultTemperature { get; set; } = 15;
        public bool ShowHPBars { get; set; } = true;
        public bool SpawnUnityObjects { get; set; } = true;
        public MockLocomotionUsage UseMockLocomotion { get; set; } = MockLocomotionUsage.None;
        public MockView UseMockCharacterView { get; set; } = MockView.None;
        public bool ReplicateAnythingToUnity { get; set; } = true;
        public int ServerMovementHistoryCapacity { get; set; } = 60 /*FPS*/ * 1 /*sec*/;
        public int ServerMovementMobHistoryCapacity { get; set; } = 5 /*FPS*/ * 1 /*sec*/;
        public bool EnableTintByDefault { get; set; } = false;
        public float MobUpdateTick {get; set;} = 0.025f;
        public float SingleMobsUpdatesNoOftenThan {get; set;} = 0.100f;
        public bool ActorWithActorCollisions { get; set; } = true;

        public bool EnableOutline { get; set; } = true;
        public bool EnableFX { get; set; } = true;
        
        public UnityRef<GameObject> MockFX { get; [UsedImplicitly] set; } // Если задан, то этот объект спавнится вместо всех FX'ов 
        public Dictionary<BaseResource, string> DefaultCheatVariableValues { get; set; } = new Dictionary<BaseResource, string>();

        public ResourceRef<SpellDef> NullSpell { get; set; }
        public ResourceRef<TechnologyAtlasDef> TechnologyAtlas { get; set; }
        
        public ResourceRef<CharacterPawnSchedulerDef> CharacterPawnScheduler { get; set; }
    }

    public enum MockLocomotionUsage
    {
        None,
        Unity,
        Cluster,
        ClusterLocalToMobs
    }

    public enum MockView
    {
        None,
        NoAnimations,
        NoView,
    }
}