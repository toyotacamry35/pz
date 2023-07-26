using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Inventory;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using L10n;
using ReactivePropsNs;
using System;
using UnityEngine;
using NLog;

namespace Uins
{
    public class QuestCounterData {
        private static int _nextFreeId = 0;
        public readonly int Id;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary> Полезно для настройки лэйаутов, если это потребуется. </summary>
        public QuestCounterDef Def;
        public IStream<int> Count = null;
        /// <summary> отсутствие требуемого количества определяется по отсутствию потока текущего количества </summary>
        public int RequiredCount;
        public LocalizedString? Name;
        public UnityRef<Sprite> Icon;
        public IStream<string> Msg = null;
        public IMutationSource MutationSource;

        public QuestCounterData(QuestCounterDef def, IStream<int> count, int required) {
            Id = System.Threading.Interlocked.Increment(ref _nextFreeId);
            Def = def;
            Count = count;
            RequiredCount = required;
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ NEW QuestCounterData[id:{Id}](def:{def}, count:{count}, required:{required}) // count.state = {count.StreamState()}").Write();
        }
        ~QuestCounterData() {
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ DESTROY QuestCounterData[id:{Id}]").Write();
        }

        public override string ToString() {
            return $"[QuestCounterData[id:{Id}] count:{Count.StreamState()}, required:{RequiredCount}, Msg:{Msg}, MS:{MutationSource}, Def:{Def}]";
        }

        public static QuestCounterData Create(QuestCounterState counterState) {
            if (counterState.counterDef is ItemsCounterDef items) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, items.Count) {
                    Name = items.Item?.ItemNameLs,
                    Icon = items.Item?.Icon
                };
            } else if (counterState.counterDef is MutationCounterDef mutation) {
                return new QuestCounterData(counterState.counterDef, null, 0) {
                    MutationSource = mutation
                }; // mutation.Faction.Target?.NameLs - Фракция не используется, пусть в тексте квеста пишут.
            } else if (counterState.counterDef is CraftCounterDef craft) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, craft.Count) {
                    Name = craft.AllTargets.FirstOrDefault()?.NameLs,
                    Icon = craft.AllTargets.FirstOrDefault()?.BlueprintIcon
                };
            } else if (counterState.counterDef is KnowledgeCounterDef knowledge) {
                var arr = knowledge.AllTargets.ToArray();
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, knowledge.Count) {
                    Name = knowledge.AllTargets.FirstOrDefault()?.NameLs,
                    Icon = knowledge.AllTargets.FirstOrDefault()?.BlueprintIcon
                };
            } else if (counterState.counterDef is SpellCounterDef spell) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, spell.Count) {
                    Name = spell.AllTargets.FirstOrDefault()?.InteractionDescriptionLs,
                    Icon = spell.AllTargets.FirstOrDefault()?.SpellIcon
                };
            } else if (counterState.counterDef is BuildCounterDef build) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, build.Count) {
                    Name = build.AllTargets.FirstOrDefault()?.NameLs,
                    Icon = build.AllTargets.FirstOrDefault()?.Image
                };
            } else if (counterState.counterDef is MobKillsCounterDef mob) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, mob.Count) {
                    Name = mob.AllTargets.FirstOrDefault()?.NameLs
                };
            } else if (counterState.counterDef is PlaceObjectCounterDef placeObject) {
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, placeObject.Count) {
                    Name = placeObject.AllTargets.FirstOrDefault()?.NameLs
                };
            } else if (counterState.counterDef is TimerCounterDef timer) { // Недооформлено, потому что не применяется в реальности
                return new QuestCounterData(counterState.counterDef, counterState.CountForClient, -1) {
                    Msg = new ReactiveProperty<string>() { Value = TimeSpan.FromSeconds(timer.Time).ToString() }
                };
            } else if (counterState.counterDef is DealDamageCounterDef dealDamage) { // Какой-то этот ресурс недоделаный, вероятно им никто не пользуется.
                return new QuestCounterData(counterState.counterDef, counterState.DamageSumCounter, dealDamage.Count) {
                    Msg = new ReactiveProperty<string>() { Value = dealDamage.ObjectType.Target.Title }
                };
            }
            return new QuestCounterData(counterState.counterDef, counterState.CountForClient, counterState.counterDef is CounteredQuestCounterDef counted ? counted.Count : -1);
        }
    }
}