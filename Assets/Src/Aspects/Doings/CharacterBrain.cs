using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.BuildingSystem;
using Assets.Src.Camera;
using Assets.Tools;
using ColonyShared.SharedCode.InputActions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using Src.InputActions;
using Src.Locomotion;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityQueueHelper;
using SVector2 = SharedCode.Utils.Vector2;
using SVector3 = SharedCode.Utils.Vector3;

namespace Src.Aspects.Doings
{
    [DefaultExecutionOrder(-1)]
    internal abstract class CharacterBrain : MonoBehaviour, 
        ILocomotionInputSource<CharacterInputs>, IInputActionHandlersFactory, ILocomotionInputReceiver, IGuideProvider
    {
        
        private ISpellDoer _spellDoer;
        private bool _initialized;
        private bool _disposed;
        private InputActionsProcessor _inputActionsProcessor;
        private InputActionStatesGenerator _inputActionsGenerator;
        private CameraGuideController _cameraGuideController;
        private readonly List<IInputActionHandlerLocomotion> _locomotionInputActionHandlers = new List<IInputActionHandlerLocomotion>();
        private readonly List<InputActionHandlerCamera> _cameraInputActionHandlers = new List<InputActionHandlerCamera>();
        private readonly InputState<CharacterInputs> _input = new InputState<CharacterInputs>();
        private readonly LocomotionInputMediator<CharacterInputs> _externalInput = new LocomotionInputMediator<CharacterInputs>();
        private InputActionHandlerInteraction.Delegate _fireTryToInteractCb;
        private TargetHolder _targetHolder;
        private IEntitiesRepository _repository;
        private OuterRef _entityRef;

        private event SpellDoerFinsihDelegate _onSpellFinished = delegate {};

        
        public event InputActionHandlerInteraction.Delegate TryToInteract;

        public event SpellDoerFinsihDelegate InteractionFinished { add => _onSpellFinished += value; remove => _onSpellFinished -= value; }

        public ICharacterBuildInterface BuildInterface { get; private set; }
        
        public IInputActionStatesGenerator InputActionStatesGenerator => _inputActionsGenerator;
        
        protected async Task Init(
            OuterRef entityRef,
            IEntitiesRepository repository,
            ISpellDoer spellDoer, 
            TargetHolder targetHolder,
            CameraGuideController cameraController,
            IInputActionStatesSource inputProvider)
        {
            AssertInUnityThread();

            Assert.IsNotNull(spellDoer);
            Assert.IsNotNull(targetHolder);
            Assert.IsNotNull(cameraController);
            _targetHolder = targetHolder;
            _spellDoer = spellDoer;
            _cameraGuideController = cameraController;
            _fireTryToInteractCb = FireTryToInteract;
            _spellDoer.OnOrderFinished += OnSpellFinished;
            _repository = repository;
            _entityRef = entityRef;
            
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cnt = await repository.Get<IWorldCharacter>(entityRef.Guid))
                {
                    var chr = cnt.Get<IWorldCharacterClientFull>(entityRef.Guid);
                    var bindingsSource = chr.InputActionHandlers.BindingsSource; 
                    Assert.IsNotNull(bindingsSource);

                    var inputActionsSource = new InputActionStatesCompositeSource(
                        _inputActionsGenerator = new InputActionStatesGenerator(),
                        inputProvider ?? new InputActionStatesSourceNull() // на безголовом клиенте input manager'а нет. если понадобится возможность управлять игроком на безголовом клиенте (как?), то нужно будет сделать другой input manager
                        );
                    var inputActionsProcessor = new InputActionsProcessor(bindingsSource, this, inputActionsSource, entityRef.Guid);
                    if (!_disposed)
                        _inputActionsProcessor = inputActionsProcessor; // теоретически сюда мы можем попасть уже после Dispose...
                    if (_disposed)
                        _inputActionsProcessor?.Dispose(); // ...и поэтому дополнительно это проверяем 
                }
                _initialized = true;
            }, repository);
        }

        private void OnSpellFinished(ISpellDoerCastPipeline spellcast)
        {
            RunInUnityThreadNoWait(() =>
            {
                var evt = _onSpellFinished;
                evt?.Invoke(spellcast);
            });
        }

        private void Awake()
        {
            BuildInterface = GetComponent<CharacterBuildBehaviour>();
        }

        protected virtual void OnDestroy()
        {
            _disposed = true;
            _initialized = false;
            _inputActionsProcessor?.Dispose();
            _spellDoer.OnOrderFinished -= OnSpellFinished;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            if (gameObject)
                Destroy(gameObject);
        }

        protected void UpdateCamera()
        {
            if (!_initialized)
                return;
            
            Vector2 axis = Vector2.zero;
            foreach (var handler in _cameraInputActionHandlers)
                handler.FetchInputValue(ref axis);
            _cameraGuideController?.Update(axis);
        }

        private void FireTryToInteract(ISpellDoerCastPipeline spellOrder, SpellDef spell)
        {
            var tryToInteract = TryToInteract;
            tryToInteract?.Invoke(spellOrder, spell);
        }
        
        #region IGuideProvider

        SharedCode.Utils.Vector3 IGuideProvider.Guide => _cameraGuideController?.Guide.ToShared() ?? SharedCode.Utils.Vector3.forward;

        #endregion
        
        #region ILocomotionInputSource

        IInputState<CharacterInputs> ILocomotionInputSource<CharacterInputs>.GetInput()
        {
            var guide = LocomotionHelpers.WorldToLocomotionVector(_cameraGuideController?.Guide ?? transform.forward);
            
            _input.Clear();
            _input[CharacterInputs.Guide] = guide.Horizontal.normalized;
            foreach (var handler in _locomotionInputActionHandlers)
                handler.FetchInputValue(_input);

            _externalInput.ApplyTo(_input);
            
            return _input;
        }

        #endregion
        
        #region ILocomotionInputReceiver

        public void SetInput(InputAxis it, float value) => _externalInput.SetInput(it, value);
        public void SetInput(InputAxes it, SVector2 value) => _externalInput.SetInput(it, value);
        public void SetInput(InputTrigger it, bool value) => _externalInput.SetInput(it, value);      
        public void PushInput(object causer, string inputName, float value) => _externalInput.PushInput(causer, inputName, value);
        public void PopInput(object causer, string inputName) => _externalInput.PopInput(causer, inputName);

        #endregion
        
        #region IInputActionHandlersFactory

        public T Create<T>(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId) where T : IInputActionHandler
        {
            if (typeof(T) == typeof(IInputActionTriggerHandler))
                return (T)CreateTriggerHandler(action, desc, bindingId);
            if (typeof(T) == typeof(IInputActionValueHandler))
                return (T)CreateValueHandler(action, desc, bindingId);
            throw new NotSupportedException($"{typeof(T)}");
        }

        private IInputActionTriggerHandler CreateTriggerHandler(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId)
        {
            switch (desc)
            {
                case IInputActionHandlerSpellDescriptor d:
                    return new InputActionHandlerSpell(d, _spellDoer, bindingId);
                case IInputActionHandlerSpellOnceDescriptor d:
                    return new InputActionHandlerSpell(d, _spellDoer, bindingId);
                case IInputActionHandlerSpellContinuousDescriptor d:
                    return new InputActionHandlerSpellContinuous(d, _spellDoer, bindingId);
                case IInputActionHandlerSpellBreakerDescriptor d:
                    return new InputActionHandlerSpellBreaker(d, _spellDoer, _entityRef, _repository, bindingId);
                case IInputActionHandlerInteractionDescriptor _:
                    return new InputActionHandlerInteraction(action, _spellDoer, _targetHolder, _fireTryToInteractCb, bindingId);
                case IInputActionHandlerLocomotionTriggerDescriptor d:
                    return RegisterInputActionLocomotionHandler(new InputActionHandlerLocomotionTrigger(d.Input, UnregisterInputActionLocomotionHandler, bindingId));
                case IInputActionHandlerLocomotionTriggerToAxisDescriptor d:
                    return RegisterInputActionLocomotionHandler(new InputActionHandlerLocomotionTriggerToAxis(d.Input, d.Value, UnregisterInputActionLocomotionHandler, bindingId));
                default:
                    throw new NotSupportedException($"{desc}");
            }
        }

        private IInputActionValueHandler CreateValueHandler(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId)
        {
            switch(desc)
            {
                case IInputActionHandlerLocomotionAxisDescriptor d:
                    return RegisterInputActionLocomotionHandler(new InputActionHandlerLocomotionAxis(d.Input, UnregisterInputActionLocomotionHandler, bindingId));
                case IInputActionHandlerCameraDescriptor d:
                    return RegisterInputActionCameraHandler(new InputActionHandlerCamera(d.Axis, UnregisterInputActionCameraHandler));
                default:
                    throw new NotSupportedException($"{desc}");
            }            
        }

        private T RegisterInputActionLocomotionHandler<T>(T h) where T : IInputActionHandlerLocomotion
        {
            lock(_locomotionInputActionHandlers) _locomotionInputActionHandlers.Add(h);
            return h;
        }

        private void UnregisterInputActionLocomotionHandler(IInputActionHandlerLocomotion h)
        {
            lock(_locomotionInputActionHandlers) _locomotionInputActionHandlers.Remove(h);
        }
        
        private InputActionHandlerCamera RegisterInputActionCameraHandler(InputActionHandlerCamera h)
        {
            lock(_cameraInputActionHandlers) _cameraInputActionHandlers.Add(h);
            return h;
        }

        private void UnregisterInputActionCameraHandler(InputActionHandlerCamera h)
        {
            lock(_cameraInputActionHandlers) _cameraInputActionHandlers.Remove(h);
        }
        
        #endregion
    }
}