using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    [UsedImplicitly]
    public partial class SpatialDataHandlers : IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy, IHookOnUnload
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _cts;
        private const int _millisecondsDelay = 1000;

        private async Task PeriodicUpdate(CancellationToken cancellationToken, IPositionedObject positionedEntity, IEntitiesRepository repository)
        {
            if (positionedEntity == null)
            {
                Logger.IfError()?.Message("IWorldCharacter should be an IPositionedObject for HasSpatialDataHandlersImplementation to work").Write();
                return;
            }

            List<IRegion> regions = new List<IRegion>();
            Dictionary<IRegion, ActiveRegion> activeRegions = new Dictionary<IRegion, ActiveRegion>();
            List<SpellToCast> spellsToCast = new List<SpellToCast>();
            List<SpellId> spellsToCancel = new List<SpellId>();
            var id = ((IDeltaObject)positionedEntity).ParentEntityId;


            while (true)
            {
                try
                {
                    await Task.Delay(_millisecondsDelay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    var ownerScene = EntitiesRepository.GetSceneForEntity(new OuterRef<IEntity>(parentEntity));
                    var rootRegion = RegionsHolder.GetRegionByGuid(ownerScene);
                    if (rootRegion == null)
                    {
                         Logger.IfTrace()?.Message("No region found by id: '{0}'",  ownerScene).Write();
                        continue;
                    }

                    var position = positionedEntity.Position;

                    spellsToCast.Clear();
                    spellsToCancel.Clear();

                    regions.Clear();
                    rootRegion.GetAllContainingRegionsNonAlloc(regions, position);

                    foreach (var region in regions)
                        AddSpellsToCastFromOnEnter(spellsToCast, activeRegions, region);

                    AddSpellsToCastFromOnExitAndSpellsToCancelFromWhileInside(spellsToCast, spellsToCancel, activeRegions, regions);

                    if (spellsToCast.Count != 0 || spellsToCancel.Count != 0)
                        using (var wrapper = await repository.Get<IWizardEntityServer>(id))
                        {
                            var targetWizardEntity = wrapper?.Get<IWizardEntityServer>(id);
                            if (targetWizardEntity != null)
                            {
                                foreach (var spell in spellsToCast)
                                {
                                    var spellId = await targetWizardEntity.CastSpell(new SpellCast() { Def = spell.SpellDef, StartAt = SyncTime.Now });
                                    if (!spell.IsCastedOnExit)
                                    {
                                        if (!activeRegions.TryGetValue(spell.Region, out ActiveRegion activeRegion))
                                            activeRegions.Add(spell.Region, activeRegion = new ActiveRegion { SpellIds = new List<SpellId>() }); ;
                                        activeRegion.SpellIds.Add(spellId);
                                    }
                                }

                                foreach (var spellId in spellsToCancel)
                                {
                                    await targetWizardEntity.StopCastSpell(spellId);
                                }
                            }
                        }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                    AsyncUtils.RunAsyncTask(async () => await PeriodicUpdate(cancellationToken, positionedEntity, repository), repository);
                    throw;
                }
            }
        }

        private void AddSpellsToCastFromOnEnter(List<SpellToCast> spellsToCast, Dictionary<IRegion, ActiveRegion> lastActiveRegions, IRegion region)
        {
            foreach (var data in region.RegionDef.Data)
            {
                var spellCastRegionDef = data.Target as SpellCastRegionDef;
                if (spellCastRegionDef != null && !lastActiveRegions.ContainsKey(region))
                {
                    var onEnterSpell = spellCastRegionDef.OnEnterSpellDef.Target;
                    if (onEnterSpell != null)
                        spellsToCast.Add(new SpellToCast(region: region, spellDef: onEnterSpell, shouldBeCancelled: false));

                    var whileInsideSpell = spellCastRegionDef.WhileInsideSpellDef.Target;
                    if (whileInsideSpell != null)
                        spellsToCast.Add(new SpellToCast(region: region, spellDef: whileInsideSpell, shouldBeCancelled: true));
                }
            }
        }

        private List<IRegion> _regionsToRemove = new List<IRegion>();
        private void AddSpellsToCastFromOnExitAndSpellsToCancelFromWhileInside(List<SpellToCast> spellsToCast, List<SpellId> spellsToCancel, Dictionary<IRegion, ActiveRegion> activeRegions, List<IRegion> regions)
        {
            _regionsToRemove.Clear();
            foreach (var activeRegion in activeRegions)
            {
                if (!regions.Contains(activeRegion.Key))
                {
                    _regionsToRemove.Add(activeRegion.Key);

                    foreach (var data in activeRegion.Key.RegionDef.Data)
                    {
                        var spellCastRegionDef = data.Target as SpellCastRegionDef;
                        if (spellCastRegionDef != null)
                        {
                            var onExitSpell = spellCastRegionDef.OnExitSpellDef.Target;
                            if (onExitSpell != null)
                                spellsToCast.Add(new SpellToCast(activeRegion.Key, onExitSpell, false, true));
                        }
                    }

                    if (activeRegion.Value.SpellIds != default)
                        spellsToCancel.AddRange(activeRegion.Value.SpellIds);
                }
            }

            foreach (var reg in _regionsToRemove)
                activeRegions.Remove(reg);
        }

        private void startPeriodicUpdate()
        {
            if (_cts != default)
                return;

            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            var repository = EntitiesRepository;
            var positionEntity = PositionedObjectHelper.GetPositioned(parentEntity);
            AsyncUtils.RunAsyncTask(async () => await PeriodicUpdate(token, positionEntity, repository), repository);
        }

        private void stopPeriodicUpdate()
        {
            if (_cts == default)
                return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        public Task OnInit()
        {
            var spEntity = (IHasSpatialDataHandlers)parentEntity;
            if (spEntity.QuerySpatialData)
                startPeriodicUpdate();
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            var spEntity = (IHasSpatialDataHandlers)parentEntity;
            if (spEntity.QuerySpatialData)
                startPeriodicUpdate();
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            stopPeriodicUpdate();
            return Task.CompletedTask;
        }

        public Task OnDestroy()
        {
            stopPeriodicUpdate();
            return Task.CompletedTask;
        }

        private readonly struct SpellToCast
        {
            public readonly IRegion Region;
            public readonly SpellDef SpellDef;
            public readonly bool ShouldBeCancelled;
            public readonly bool IsCastedOnExit;

            public SpellToCast(IRegion region, SpellDef spellDef, bool shouldBeCancelled, bool isCastedOnExit = false)
            {
                Region = region;
                SpellDef = spellDef;
                ShouldBeCancelled = shouldBeCancelled;
                IsCastedOnExit = isCastedOnExit;
            }
        }

        private struct ActiveRegion
        {
            public List<SpellId> SpellIds { get; set; }
        }
    }
}