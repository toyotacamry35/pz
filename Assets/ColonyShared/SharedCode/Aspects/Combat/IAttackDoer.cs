using Assets.ColonyShared.GeneratedCode.Impacts;
using Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Utils.DebugCollector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;  
using System.Threading.Tasks;
using Src.ManualDefsForSpells;

namespace ColonyShared.SharedCode.Aspects.Combat
{
    public interface IAttackDoer
    {
        void StartServerSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, List<ResourceRef<GameObjectMarkerDef>> trajectoryMarkers, AnimationStateDef animation, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef);

        void StartClientSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, object animKey, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef);

        void FinishAttack(SpellPartCastId attackId, TimeRange timeRange, long currentTime);
    }
    
    public struct AttackAnimationInfo
    {
        public AnimationStateDef State;
        public long StartTime;
        public float SpeedFactor;
        public float AnimationOffset;
    }
}