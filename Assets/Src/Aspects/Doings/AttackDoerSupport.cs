using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Lib.Extensions;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Aspects.Combat;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Aspects.Misc;
using SharedCode.Utils.DebugCollector;
using Src.Animation;
using Src.Animation.ACS;
using Src.Tools;
using UnityEngine;
using static UnityQueueHelper;

namespace Src.Aspects.Doings
{
    public class AttackDoerSupport : IAttackDoerSupport, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly Dictionary<(AnimationStateDef,GameObjectMarkerDef), AnimationTrajectory> _trajectories;
        private readonly Dictionary<object, AnimationPlayInfo> _plays = new Dictionary<object,AnimationPlayInfo>();
        private readonly Dictionary<object, List<TaskCompletionSource<AttackAnimationInfo>>> _awaiters = new Dictionary<object, List<TaskCompletionSource<AttackAnimationInfo>>>();
        private readonly IAnimationPlayProvider _doer;

        public AttackDoerSupport(Animator animator, AnimationTrajectoriesStorage trajectoriesStorage, IAnimationPlayProvider doer)
        {
            if (animator == null) throw new ArgumentNullException(nameof(animator));
            if (trajectoriesStorage == null)  throw new ArgumentNullException(nameof(trajectoriesStorage));
            _doer = doer ?? throw new ArgumentNullException(nameof(doer));
            _trajectories = animator.GetBehaviours<AnimationStateTrajectory>()
                .Select( x =>
                {
                    try
                    {
                        return (state: x.StateDef, bone: x.BodyPart.Target, trajectory: trajectoriesStorage.GetTrajectory(x.Guid));
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message($"Exception while get trajectory for state:{x.StateDef.____GetDebugAddress()} and bodyPart:{x.BodyPart.Target.____GetDebugAddress()}:\n{e}").Write();
                        return default;
                    }
                })
                .Where(x => x.state != null).ToDictionary(x => (x.state, x.bone), x => x.trajectory);
            _doer.AnimationPlayStarted += OnPlayStart;
        }

        public void Dispose()
        {
            _doer.AnimationPlayStarted -= OnPlayStart;
        }

        public Task<AttackAnimationInfo> FetchAttackAnimationInfo(object playId)
        {
            AssertInUnityThread();
            
            if (playId == null) throw new ArgumentNullException(nameof(playId));

            if (!_plays.TryGetValue(playId, out var nfo))
            {
                if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"No anim play found. Engadge awaiter. | Id:{playId}").Write();
                var awaiters = _awaiters.GetOrCreate(playId);
                var taskSource = new TaskCompletionSource<AttackAnimationInfo>();
                awaiters.Add(taskSource);
                return taskSource.Task;
                //throw new KeyNotFoundException($"There are no animation play with Id:{playId}");
            }
            _plays.Remove(playId);
            return Task.FromResult(CreateResult(nfo));
        }

        public bool TryGetTrajectory(AnimationStateDef stateDef, GameObjectMarkerDef bodyPart, out AnimationTrajectory trajectory)
        {
            AssertInUnityThread();
            return _trajectories.TryGetValue((stateDef, bodyPart), out trajectory);
        }

        private void OnPlayStart(in AnimationPlayInfo info)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Anim Info | Id:{info.PlayId} Def:{info.StateDef} StartAt:{info.StartTime} Offset:{info.AnimationOffset} Speed:{info.SpeedFactor}").Write();
            Collect.IfActive?.Event("AttackDoerSupport.AnimationStarted");

            AssertInUnityThread();
            
            _plays[info.PlayId] = info;

            if (_awaiters.TryGetValue(info.PlayId, out var awaiters))
            {
                _awaiters.Remove(info.PlayId);
                var result = CreateResult(info);
                foreach (var awaiter in awaiters)
                {
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Fire Awaiter | {awaiter}").Write();
                    awaiter.SetResult(result);
                }
            }
        }

        private AttackAnimationInfo CreateResult(AnimationPlayInfo nfo)
        {
            return new AttackAnimationInfo
            {
                State = nfo.StateDef,
                StartTime = nfo.StartTime,
                SpeedFactor = nfo.SpeedFactor,
                AnimationOffset = nfo.AnimationOffset
            };
        }
    }
}
