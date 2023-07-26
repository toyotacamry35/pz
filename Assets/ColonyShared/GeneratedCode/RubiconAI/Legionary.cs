using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using GeneratedCode.MapSystem;
using ResourceSystem.Aspects.Templates;

namespace Assets.Src.RubiconAI
{
    public class Legionary
    {
        public static ConcurrentDictionary<OuterRef<IEntity>, Legionary> LegionariesByRef = new ConcurrentDictionary<OuterRef<IEntity>, Legionary>();

        public IEntitiesRepository Repository;

        public OuterRef<IEntity> _ref;
        public OuterRef<IEntity> Ref
        {
            get
            {
                return _ref;
            }
            set
            {
                LegionariesByRef.TryRemove(_ref, out var _);
                _ref = value;
                LegionariesByRef.TryAdd(_ref, this);
            }
        }

        public bool IsValid { get; set; } = true;
        public IEntityObjectDef EntityDef { get; internal set; }

        public Guid WorldSpaceGuid;
        public Guid SceneId;
        public Legion AssignedLegion;
        public int Index;
        private volatile static int Counter = 0;

        public Legionary(IEntitiesRepository repo)
        {
            Repository = repo;
            Index = Interlocked.Increment(ref Counter);
        }

        public void AssignToLegion(Legion newLegion)
        {
            if (AssignedLegion == newLegion)
                return;

            UnassignFromLegion();
            AssignedLegion = newLegion;
            OnAssignToLegion(newLegion);
            newLegion.Assign(this);
        }

        public virtual void OnAssignToLegion(Legion newLegion) { }

        public void UnassignFromLegion()
        {
            if (AssignedLegion == null)
                return;
            OnUnassignToLegion();
            AssignedLegion.Unassign(this);
            AssignedLegion = null;
        }

        public virtual void OnUnassignToLegion() { }

        public override int GetHashCode() => Index;
    }

    public static class SpatialLegionaryExtension
    {
        public static bool Valid(this ISpatialLegionary legionary)
        {
            return legionary != null && legionary.Legionary != null && legionary.Legionary.IsValid;
        }
        public static bool Valid(this Legionary legionary)
        {
            return legionary != null && legionary.IsValid;
        }
    }

    public class CooldownsStatus
    {
        public Dictionary<string, float> Cooldowns = new Dictionary<string, float>();

        public CooldownsStatus(MobLegionary legionary)
        {
            foreach (var blValue in legionary.TemporaryBlackboard)
            {
                Cooldowns[blValue.Key] = SyncTime.ToSeconds(blValue.Value - SyncTime.NowUnsynced);
            }
        }
    }

    public class RememberedLegionary
    {
        public Legionary WasThisLegionary;
        public string WasNamed;

        public RememberedLegionary(Legionary x)
        {
            WasThisLegionary = x;
            WasNamed = ((BaseResource)x.EntityDef).____GetDebugShortName();
        }
    }

    public class AIEvent
    {
        public ResourceRef<AIEventDef> StaticData { get; set; }
        public Legionary Initiator { get; set; }
    }

    public class SpellCastEvent : AIEvent
    {
        public Legionary Target { get; set; }
    }

    public interface ISpatialLegionary
    {
        Legionary Legionary { get; }
    }

    public class DummyLegionary : Legionary
    {
        public DummyLegionary(IEntitiesRepository repo) : base(repo) { }
    }
}
