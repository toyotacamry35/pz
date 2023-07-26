using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Shared;
using Assets.Src.SpatialSystem;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.AI;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;
using UnityEngine;
namespace System.Collections.Generic
{
#if !NETSTANDARD
    public static class HackExtension
    {
        public static void TrimExcess(this ICollection collection)
        {
        
        }
    }
#endif

}
namespace Assets.Src.RubiconAI.KnowledgeSystem
{
    class SpatialKnowledgeSource : IKnowledgeSource
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public string FeedName { get; set; }
        public float FeedRange { get; set; }
        float _reduceFactor = 1f;//do not reduce
        static int _reduceIfMoreThan = 15;
        public LayersEnum Layer { get; set; }
        public KnowledgeCategoryDef Category { get; set; }
        public Dictionary<OuterRef<IEntity>, VisibilityDataSample> Legionaries { get; set; } = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();
        public KnowledgeSourceDef Def { get; set; }
        public event Func<OuterRef<IEntity>, ValueTask> OnLearnedAboutLegionary;
        public event Func<OuterRef<IEntity>, ValueTask> OnForgotAboutLegionary;
        public MobLegionary Legionary;
        // --- Publics: -------------------------------------------------------
        long _nextTimeToUpdate;
        static Pool<HashSet<OuterRef<IEntity>>> _diffsetPool =
            new Pool<HashSet<OuterRef<IEntity>>>(30, 3, () => new HashSet<OuterRef<IEntity>>(), x => x.Clear());

        static Pool<Dictionary<OuterRef<IEntity>, VisibilityDataSample>> _prevLegionariesPool = 
            new Pool<Dictionary<OuterRef<IEntity>, VisibilityDataSample>>(30, 3, () => new Dictionary<OuterRef<IEntity>, VisibilityDataSample>(), x => x.Clear());

        DateTime _lastTrimTime;
        public async ValueTask UpdateKnowledge()
        {
            if (_nextTimeToUpdate > SyncTime.NowUnsynced)
                return;
            _nextTimeToUpdate = SyncTime.NowUnsynced + SyncTime.FromSeconds(0.25f);
            if (Legionary == null || !Legionary.IsValid)
                return;
            if (Legionary.WorldSpaceGuid == Guid.Empty)
                return;
            var prevLegionaries = _prevLegionariesPool.Take();
            var diffSet = _diffsetPool.Take();
            try
            {

                var grid = VisibilityGrid.Get(Legionary.WorldSpaceGuid, Legionary.Repository);
                prevLegionaries.Clear();
                foreach (var leg in Legionaries)
                    prevLegionaries.Add(leg.Key, leg.Value);
                Legionaries.Clear();

                int countAround = grid.SampleDataForAllAround(new OuterRef<IEntity>(Legionary.Ref.Guid, Legionary.Ref.TypeId), Legionaries, FeedRange * _reduceFactor, true, _reduceIfMoreThan);
                Legionaries.Remove(new OuterRef<IEntity>(Legionary.Ref.Guid, Legionary.Ref.TypeId));
                /*foreach (var leg in Legionaries)
                {

                    var selfPos = Legionary.GetPos(Legionary).Value;
                    var otherPos = leg.Value.Pos;
                }*/
                if (countAround > _reduceIfMoreThan)
                {
                    _reduceFactor = Math.Max(0.2f, _reduceFactor * 0.7f);
                    TryTrim();

                }
                else
                    _reduceFactor = Math.Min(1f, _reduceFactor * 1.01f);

                diffSet.Clear();
                foreach (var oRef in prevLegionaries)
                {
                    diffSet.Add(oRef.Key);
                }
                foreach (var item in Legionaries)
                {
                    if (!diffSet.Remove(item.Key))
                    {
                        diffSet.Add(item.Key);
                    }
                }
                foreach (var oRef in diffSet)
                {
                    if (prevLegionaries.ContainsKey(oRef))
                    {
                        //it was there and is in diff = remove
                        await OnForgotAboutLegionary.InvokeAndWaitAll(oRef);
                    }
                    else
                    {
                        await OnLearnedAboutLegionary.InvokeAndWaitAll(oRef);
                        //it was not there and is in diff = add
                    }
                }
            }
            finally
            {
                _diffsetPool.Return(diffSet);
                _prevLegionariesPool.Return(prevLegionaries);
            }
        }

        private void TryTrim()
        {
            if((DateTime.Now - _lastTrimTime) > TimeSpan.FromSeconds(60))
            {
                _lastTrimTime = DateTime.Now;
                Legionaries.TrimExcess();
            }
        }

        public BaseResource GetId() => Category;

        public async ValueTask LoadDef(Knowledge knowledge, KnowledgeSourceDef def)
        {
            var skDef = (SpatialKnowledgeSourceDef)def;
            FeedRange = skDef.FeedRange;
            Category = skDef.Category;
        }
    }
}
