using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Src.Locomotion
{
    public abstract class StateMachineContext : IDisposable, IResettable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        
        public readonly ILocomotionClock Clock;
        public ILocomotionStateInfo CurrentState { get; private set; }
        public LocomotionVariables CurrentVars { get; private set; }
        public float StateElapsedTime { get; private set; }

        public abstract bool IsReady { get; }

        public void SetCurrentState(ILocomotionStateInfo state) => CurrentState = state;

        public void SetCurrentVars(in LocomotionVariables vars) => CurrentVars = vars;

        public void SetStateElapsedTime(float time) => StateElapsedTime = time;

        protected StateMachineContext(ILocomotionClock clock)
        {
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
#if DEBUG
            Debug.Assert(CurrentState == null && StateElapsedTime == 0f);
#endif
            AddDisposables(clock);
        }

        public virtual void Reset()
        {
            ///#PZ-17474: #Dbg:
            if (DbgLog.Enabled) DbgLog.Log($"StateMachineContext.Reset 2of2");

            CurrentState = null;
            CurrentVars = default;
            StateElapsedTime = 0f;
        }

        protected void AddDisposables(params object[] things)
        {
            foreach (var thing in things)
                if (thing is IDisposable disposable)
                    if (!_disposables.Contains(disposable))
                        _disposables.Add(disposable);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}