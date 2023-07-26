using Assets.Src.AI.DamageIndication;
using Assets.Src.Aspects.Impl.EntityGameObjectComponents;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Src.Wizardry;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using SharedCode.Utils.Extensions;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats.Proxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uins;
using UnityEngine;

namespace Assets.Src.Aspects
{
    class DebugSpellDrawer : EntityGameObjectComponent
    {
        public ConcurrentDictionary<SpellId, SpellCastStatus> CurrentSpells { get; } = new ConcurrentDictionary<SpellId, SpellCastStatus>();

        public string Filter;
        public float DrawDistance = 10;
        public float OffsetY;
        protected override void GotClient()
        {
            if (!Constants.WorldConstants.ShowHPBars)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                await Subscribe(ClientRepo);
            }, ClientRepo);
        }

        protected override void LostClient()
        {
            if (!Constants.WorldConstants.ShowHPBars)
                return;
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                var outerRef = GetOuterRef<IHasWizardEntity>();
                using (var ent = await ClientRepo.Get(outerRef.TypeId, outerRef.Guid))
                {
                    var hw = ent.Get<IHasWizardEntityClientBroadcast>(outerRef.TypeId, outerRef.Guid, ReplicationLevel.ClientBroadcast);
                    if (hw != null)
                        using (var w = await ClientRepo.Get<IWizardEntityClientBroadcast>(hw.Wizard.Id))
                        {
                            var wizard = w.Get<IWizardEntityClientBroadcast>(hw.Wizard.Id);
                            if (wizard != null)
                            {
                                foreach (var spell in wizard.Spells)
                                {
                                    await Wizard_SpellFinishedInternal(spell.Value);
                                }
                                wizard.Spells.OnItemAddedOrUpdated -= Wizard_SpellStarted;
                                wizard.Spells.OnItemRemoved -= Wizard_SpellFinished;
                            }
                        }
                }
            }, ClientRepo);
        }

        async Task Subscribe(IEntitiesRepository repoToGetEntity)
        {
            using (var ent = await repoToGetEntity.Get(GetOuterRef<IHasWizardEntity>().RepTypeId(ReplicationLevel.ClientBroadcast), GetOuterRef<IHasWizardEntity>().Guid))
            {
                var hw = ent.Get<IHasWizardEntityClientBroadcast>(GetOuterRef<IHasWizardEntity>().RepTypeId(ReplicationLevel.ClientBroadcast), GetOuterRef<IHasWizardEntity>().Guid);
                using (var w = await repoToGetEntity.Get<IWizardEntityClientBroadcast>(hw.Wizard.Id))
                {
                    var wizard = w.Get<IWizardEntityClientBroadcast>(hw.Wizard.Id);
                    foreach (var spell in wizard.Spells)
                    {
                        await Wizard_SpellStartedInternal(hw.Wizard.TypeId, hw.Wizard.Id, spell.Value);
                        // TODO: если это клиент с авторити то, по поидее, нужно запускать спелл на local runner'е 
                    }
                    wizard.Spells.OnItemAddedOrUpdated += Wizard_SpellStarted;
                    wizard.Spells.OnItemRemoved += Wizard_SpellFinished;
                }
            }
        }

        private async Task Wizard_SpellStarted(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> eventArgs)
        {
            var entityTypeId = eventArgs.Sender.ParentTypeId;
            var entityId = eventArgs.Sender.ParentEntityId;
            var newSpell = eventArgs.Value;
            await Wizard_SpellStartedInternal(entityTypeId, entityId, newSpell);
        }

        private async Task Wizard_SpellStartedInternal(int entityTypeId, Guid entityId, ISpellClientBroadcast newSpell)
        {
            using (var wrapper = await ClientRepo.Get(entityTypeId, entityId))
            {
                var id = newSpell.Id;
                var start = newSpell.Started;
                var cast = newSpell.CastData;
                CurrentSpells.TryAdd(id, new SpellCastStatus() { StartTimeStamp = start, SpellDesc = cast });
            }
        }

        private async Task Wizard_SpellFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientBroadcast> eventArgs)
        {
            var removedSpell = eventArgs.Value;
            await Wizard_SpellFinishedInternal(removedSpell);
        }

        private Task Wizard_SpellFinishedInternal(ISpellClientBroadcast removedSpell)
        {
            var reason = removedSpell.FinishReason;
            var id = removedSpell.Id;
            var timeStamp = removedSpell.Finished;
            if (CurrentSpells.TryGetValue(id, out var spell))
            {
                spell.FinishReason = reason;
                spell.RemoveAt = SyncTime.NowUnsynced + 2000;
                spell.EndTimeStamp = timeStamp;
            }   
            return Task.CompletedTask;
        }

#if UNITY_EDITOR        
        List<SpellId> _spellsToRemove = new List<SpellId>();
        private void OnGUI()
        {
            if (!Constants.WorldConstants.ShowHPBars)
            {
                this.enabled = false;
                return;
            }
            if (!DebugExtension.Draw)
                return;
            if (UnityEngine.Camera.main == null)
                return;
            if ((UnityEngine.Camera.main.transform.position - transform.position).magnitude > DrawDistance)
                return;
            if (string.IsNullOrWhiteSpace(Filter))
                return;
            CurrentSpells.RemoveAllNonAlloc((x, y) => y.RemoveAt < SyncTime.NowUnsynced && y.FinishReason != SpellFinishReason.None, _spellsToRemove);
            int index = 0;
            var pos = (transform.position + new Vector3(0, 2.5f, 0)).WorldToGuiPoint();
            if (pos.z < 0)
                return;
            var now = SyncTime.Now;
            var texRect = new Rect(pos + new Vector3(-150 / 2, OffsetY), new Vector2(150, 25));
            var statuses = CurrentSpells.ToDictionary(x => x.Key, x => new SpellCastStatus(x.Value));
            foreach (var status in statuses)
            {

                var spell = status.Value;
                if (!spell.SpellDesc.Def.____GetDebugShortName().Contains(Filter))
                    continue;
                var spellId = status.Key;
                var rect = texRect;
                rect.position += index * new Vector2(0, rect.size.y);
                float fill = 1;
                Color fillColor = Color.blue;
                if (spell.SpellDesc.Def.IsInfinite)
                {
                    fillColor = Color.yellow;
                }
                else
                {
                    fill = SyncTime.ToSeconds((Math.Min(spell.EndTimeStamp, now) - spell.StartTimeStamp)) / spell.SpellDesc.Def.Duration;
                }
                switch (spell.FinishReason)
                {
                    case SpellFinishReason.SucessOnTime:
                    case SpellFinishReason.SucessOnDemand:
                        fillColor = Color.green;
                        break;
                    case SpellFinishReason.FailOnStart:
                    case SpellFinishReason.FailOnDemand:
                    case SpellFinishReason.FailOnEnd:
                        fillColor = Color.red;
                        break;
                }
                GUIExtensions.DrawRectangle(new Rect(rect.position - new Vector2(1, 1), new Vector2(rect.width + 2, rect.height + 2)), Color.black);
                GUIExtensions.DrawRectangle(new Rect(rect.position, new Vector2(rect.width * fill, rect.height)), fillColor);
                var textArea = new Rect(new Vector2(rect.xMax, rect.y), new Vector2(150, 25));
                GUI.Box(textArea, $"{spellId} {spell.SpellDesc.Def.____GetDebugShortName()}");
                index++;
            }
        }
#endif
    }
}
