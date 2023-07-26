using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Aspects.Misc
{
    public interface IAttackDescriptor
    {
        [CanBeNull] AttackTypeDef AttackType { get; }
        bool IsMiningDamage { get; }
        [CanBeNull] IReadOnlyList<AttackActionDef> ActionsOnAttacker { get; }
        [CanBeNull] IReadOnlyList<AttackActionDef> ActionsOnVictim { get; }
        int TargetsLimit { get; }
        float DistanceLimit { get; }
    }
    
    public class AttackDef : BaseResource, IAttackDescriptor
    {
        [UsedImplicitly] public ResourceArray<AttackModifierDef> Modifiers;
        [UsedImplicitly] public ResourceArray<AttackActionDef> ActionsOnAttacker;
        [UsedImplicitly] public ResourceArray<AttackActionDef> ActionsOnVictim;
        [UsedImplicitly] public ResourceRef<ItemSpecificStats> OverriddenWeaponStats;
        [UsedImplicitly] public ResourceRef<AttackTypeDef> AttackType;
        [UsedImplicitly] public bool IsMiningDamage;
        [UsedImplicitly] public int TargetsLimit = 4;
        [UsedImplicitly] public float DistanceLimit = 2;
        AttackTypeDef IAttackDescriptor.AttackType => AttackType;
        bool IAttackDescriptor.IsMiningDamage => IsMiningDamage;
        IReadOnlyList<AttackActionDef> IAttackDescriptor.ActionsOnAttacker => ActionsOnAttacker;
        IReadOnlyList<AttackActionDef> IAttackDescriptor.ActionsOnVictim => ActionsOnVictim;
        int IAttackDescriptor.TargetsLimit => Math.Min(TargetsLimit, Constants.AttackConstants.AttackTargetsLimit);
        float IAttackDescriptor.DistanceLimit => Math.Min(DistanceLimit, Constants.AttackConstants.AttackDistanceLimit);
    }
    
    [KnownToGameResources]
    public struct CurveDefKey
    {
        public float Time;
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public class TrajectoryAnimationSetDef : BaseResource
    {
        public Dictionary<ResourceRef<GameObjectMarkerDef>, ResourceRef<TrajectoryDef>> Trajectories { get; set; }= new Dictionary<ResourceRef<GameObjectMarkerDef>, ResourceRef<TrajectoryDef>>();
    }

    public class TrajectoryDef : BaseResource
    {
        public float Duration { get; set; }
        public int SamplesCount { get; set; } // избыточное поле ( = Keys.Count) но пусть будет для наглядности
        public float BoundingSphereRadius { get; set; }
        public List<CurveDefKey> Keys { get; set; } = new List<CurveDefKey>();
    }

}