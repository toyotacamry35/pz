using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Src.Aspects.Impl.Stats;
using Src.Aspects.Impl.Stats.Proxy;
using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.AI.DamageIndication
{
    public class HpIndicationSwitchersController : AHpIndicationSwitchersController
    {
        [Range(0, 1)]
        public float _normalizedHpLevelForTest = 1;
        private TimeStatState _state;
        private float _hpMinLevel;
        private float _hpMaxLevel;
        public bool _defaultHpIsMax = true;
        public float CurrentValue;
        private void Awake() => SetBlendSmoothnessInShaderSwitchers();

        public static async Task SubscribeHpIndicatorControllerInternal(StatResource _hpStatResource, int typeId, Guid id, HpIndicationSwitchersController hpIndicator, IEntitiesRepository repo, Aspects.SubscribeUnsubscribe instruction)
        {
            using (var wrapper = await repo.Get(typeId, id))
            {
                wrapper.TryGet<IHasStatsEngineClientBroadcast>(typeId, id, ReplicationLevel.ClientBroadcast, out var statEntity);
                if (statEntity == null)
                {
                    Logger.IfWarn()?.Message("No IHasStatsClientBroadcast in entity {0}", id).Write();
                    return;
                }
                var hpStat = await statEntity.Stats.GetBroadcastStat(_hpStatResource);
                if (hpStat == null)
                {
                    Logger.IfWarn()?.Message("No stat found by resource {0} in entity {1}", _hpStatResource, statEntity).Write();
                    return;
                }
                switch (instruction)
                {
                    case Aspects.SubscribeUnsubscribe.Subscribe:
                        {

                            var state = ((ITimeStat)hpStat).State;
                            float hpMinLevel = ((ITimeStat)hpStat).LimitMinCache;
                            var hpMaxLevel = ((ITimeStat)hpStat).LimitMaxCache;
                            UnityQueueHelper.RunInUnityThreadNoWait(() =>
                            {
                                if (hpIndicator != null)
                                    hpIndicator.SetValues(state, hpMinLevel, hpMaxLevel);
                            });
                            await SubDeadOrAlive(_hpStatResource, typeId, id, hpIndicator, repo, instruction);
                            ((ITimeStat)hpStat).SubscribePropertyChanged(nameof(ITimeStat.State), hpIndicator.SetNewState);
                            ((ITimeStat)hpStat).SubscribePropertyChanged(nameof(ITimeStat.LimitMinCache), hpIndicator.SetNewMinHp);
                            ((ITimeStat)hpStat).SubscribePropertyChanged(nameof(ITimeStat.LimitMaxCache), hpIndicator.SetNewMaxHp);
                            break;
                        }
                    case Aspects.SubscribeUnsubscribe.Unsubscribe:
                        {
                            await SubDeadOrAlive(_hpStatResource, typeId, id, hpIndicator, repo, instruction);
                            ((ITimeStat)hpStat).UnsubscribePropertyChanged(nameof(ITimeStat.State), hpIndicator.SetNewState);
                            ((ITimeStat)hpStat).UnsubscribePropertyChanged(nameof(ITimeStat.LimitMinCache), hpIndicator.SetNewMinHp);
                            ((ITimeStat)hpStat).UnsubscribePropertyChanged(nameof(ITimeStat.LimitMaxCache), hpIndicator.SetNewMaxHp);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                }
            }
        }
        public static void SubscribeHpIndicatorController(StatResource _hpStatResource, int typeId, Guid id, HpIndicationSwitchersController hpIndicator, IEntitiesRepository repo, Aspects.SubscribeUnsubscribe instruction)
        {
            AsyncUtils.RunAsyncTask(() =>
                SubscribeHpIndicatorControllerInternal(_hpStatResource, typeId, id, hpIndicator, repo, instruction)
            );
        }

        private Func<Guid, int, SharedCode.Entities.GameObjectEntities.PositionRotation, Task> _onResurrect;
        private static async Task SubDeadOrAlive(StatResource _hpStatResource, int typeId, Guid id, HpIndicationSwitchersController hpIndicator, IEntitiesRepository repo, Aspects.SubscribeUnsubscribe instruction)
        {
            using (var wrapper = await repo.Get(typeId, id))
            {
                var mortal = wrapper?.Get<IHasMortalClientBroadcast>(typeId, id, ReplicationLevel.ClientBroadcast);
                if (mortal != null)
                {
                    switch (instruction)
                    {
                        case SubscribeUnsubscribe.Subscribe:
                            mortal.Mortal.ResurrectEvent += hpIndicator._onResurrect = (guid, tid, pr) =>
                            {
                                AsyncUtils.RunAsyncTask(async () =>
                                {
                                    await Task.Delay(1000);
                                    await SubscribeHpIndicatorControllerInternal(_hpStatResource, typeId, id, hpIndicator, repo, SubscribeUnsubscribe.Subscribe);
                                }, repo);
                                return Task.CompletedTask;
                            };
                            break;
                        case SubscribeUnsubscribe.Unsubscribe:
                            mortal.Mortal.ResurrectEvent -= hpIndicator._onResurrect;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                    }
                }
            }
        }
        private void Update()
        {
            if (_hpMaxLevel != 0)
            {
                long passedMs = SyncTime.Now - _state.LastBreakPointTime;
                float passedSeconds = passedMs / 1000.0f;
                var unclampedValue = _state.LastBreakPointValue + _state.ChangeRateCache * passedSeconds;
                var clampedValue = Math.Min(Math.Max(unclampedValue, _hpMinLevel), _hpMaxLevel);
                //var normalizedValue = clampedValue / (_hpMaxLevel - _hpMinLevel); // _hpMinLevel default is float.Min
                var normalizedValue = clampedValue / _hpMaxLevel;
                var clampedNormalizedValue = Mathf.Clamp01(normalizedValue);
                CurrentValue = clampedNormalizedValue;
                SetAllShaderSwitchers(clampedNormalizedValue);
            }
            else
                SetAllShaderSwitchers(_defaultHpIsMax ? 1 : 0);
        }

        public Task SetNewState(EntityEventArgs args)
        {
            var newState = (TimeStatState)args.NewValue;
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (this != null)
                    _state = newState;
            });
            return Task.CompletedTask;
        }

        public Task SetNewMinHp(EntityEventArgs args)
        {
            var newMinHp = (float)args.NewValue;
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (this != null)
                    _hpMinLevel = newMinHp;
            });
            return Task.CompletedTask;
        }

        public Task SetNewMaxHp(EntityEventArgs args)
        {
            var newMaxHp = (float)args.NewValue;
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (this != null)
                    _hpMaxLevel = newMaxHp;
            });
            return Task.CompletedTask;
        }

        internal void SetValues(TimeStatState state, float hpMinLevel, float hpMaxLevel)
        {
            _state = state;
            _hpMinLevel = hpMinLevel;
            _hpMaxLevel = hpMaxLevel;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetAllShaderSwitchers(_normalizedHpLevelForTest);
            SetBlendSmoothnessInShaderSwitchers();
        }
#endif

    }
}