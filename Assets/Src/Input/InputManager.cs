using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ReactivePropsNs.ThreadSafe;
using SharedCode.Logging;
using UnityEngine;

namespace Src.Input
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : MonoBehaviour, IInputActionStatesSource
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(InputManager));
        
        public static readonly ResourceRef<InputBindingsDef> PlayerInputBindingsQwerty = new ResourceRef<InputBindingsDef>(@"/UtilPrefabs/Input/PlayerInputBindingsQwerty");
        public static readonly ResourceRef<InputBindingsDef> PlayerInputBindingsAzerty = new ResourceRef<InputBindingsDef>(@"/UtilPrefabs/Input/PlayerInputBindingsAzerty");

        private static InputManager _instance;

        private InputDispatchersStack _inputDispatchers;
        private InputDispatchersPool _dispatchersPool;
        
        public static InputManager Instance => _instance;


        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => _inputDispatchers.Stream(action);

        public IStream<InputActionValueState> Stream(InputActionValueDef action) => _inputDispatchers.Stream(action);
        
        public IEnumerable<InputSourceDef> GetBindingForAction(InputActionDef action) => _inputDispatchers.GetBindingForAction(action);

        public InputSlotDef GetSlotForAction(InputActionDef action) => _inputDispatchers.GetSlotForAction(action);

        public bool IsActionBlocked(InputActionDef action) => _inputDispatchers.IsActionBlocked(action);
        
        public void PushBindings([NotNull] object causer, [NotNull] InputBindingsDef bindings) 
        {            
            _inputDispatchers.PushDispatcher(causer, _dispatchersPool.Acquire(bindings));
        }

        public void PopBindings([NotNull] object causer)
        {
            var dispatcher = _inputDispatchers.PopDispatcher(causer);
            if (dispatcher != null)
                _dispatchersPool.Release(dispatcher);
        }

        public void SetKeyboardLayout(KeyboardLayout layout)
        {
            ResourceRef<InputBindingsDef> layoutDef;
            switch (layout)
            {
                case KeyboardLayout.Qwerty:  layoutDef = PlayerInputBindingsQwerty;  break;
                case KeyboardLayout.Azerty:  layoutDef = PlayerInputBindingsAzerty;  break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
            PopBindings(this);
            Logger.IfInfo()?.Message($"Initialize input manager with {layoutDef}").Write();
            PushBindings(this, layoutDef);
        }

        [UsedImplicitly] //U
        private void Awake()
        {
            if (_instance == null)
            {
                Log.StartupStopwatch.Milestone("InputManager Awake start");
                _instance = this;
                try
                {
                    _dispatchersPool = new InputDispatchersPool();
                    _inputDispatchers = new InputDispatchersStack();
                    //old(moved): _inputDispatchers.PushDispatcher(this, new InputDispatcherBuilder().AddBindings(PlayerInputBindings).Build());
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                    Debug.LogException(e);
                }
                Log.StartupStopwatch.Milestone("InputManager Awake DONE!");
            }
        }

        [UsedImplicitly] //U
        private void OnDestroy()
        {
            Dispose();
            if (_instance == this)
                _instance = null;
        }

        [UsedImplicitly] //U
        private void Update()
        {
            _inputDispatchers.UpdateStates();
        }

        public void Dispose()
        {
            _inputDispatchers?.Dispose();
            _inputDispatchers = null;
        }
    }

    public enum KeyboardLayout : byte
    {
        Qwerty,
        Azerty,

        //#Important!!: Don't forget to update `Max` when add new values to the enum
        Max = Azerty
    }
}