using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.Audio;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Entities.Reactions;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using Src.Locomotion.Unity;
using UnityEngine;
using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.Character.Events
{
    public class VisualEventSwapWrapper : IVisualEvent
    {
        private readonly IVisualEvent _visualEvent = default(VisualEvent);

        public FXEventType eventType =>    _visualEvent.eventType;
        public OuterRef<IEntity> casterEntityRef =>     _visualEvent.targetEntityRef;
        public IEntitiesRepository casterRepository =>  _visualEvent.targetRepository;
        public GameObject casterGameObject =>           _visualEvent.targetGameObject;
        public OuterRef<IEntity> targetEntityRef =>     _visualEvent.casterEntityRef;
        public IEntitiesRepository targetRepository =>  _visualEvent.casterRepository;
        public GameObject targetGameObject =>           _visualEvent.casterGameObject;
        public Vector3 position =>                      _visualEvent.position;
        public Quaternion rotation =>                   _visualEvent.rotation;
        public ArgTuple[] parameters => _visualEvent.parameters;

        public VisualEventSwapWrapper(IVisualEvent visualEvent)
        {
            _visualEvent = visualEvent;
        }
    }
    
    public class VisualEventSubWrapper : IVisualEvent
    {
        private readonly IVisualEvent _visualEvent = default(VisualEvent);

        public FXEventType eventType =>    _visualEvent.eventType;
        public OuterRef<IEntity> casterEntityRef =>     _visualEvent.targetEntityRef;
        public IEntitiesRepository casterRepository =>  _visualEvent.targetRepository;
        public GameObject casterGameObject { get; }
        public OuterRef<IEntity> targetEntityRef =>     _visualEvent.casterEntityRef;
        public IEntitiesRepository targetRepository =>  _visualEvent.casterRepository;
        public GameObject targetGameObject =>           _visualEvent.casterGameObject;
        public Vector3 position =>                      _visualEvent.position;
        public Quaternion rotation =>                   _visualEvent.rotation;
        public ArgTuple[] parameters => _visualEvent.parameters;

        public VisualEventSubWrapper(IVisualEvent visualEvent, GameObject casterGameObject)
        {
            _visualEvent = visualEvent;
            this.casterGameObject = casterGameObject;
        }
    }
    
    [UsedImplicitly]
    class SoundState : IVisualEffectHandlerBinding<SoundStateDef>
    {
        public void OnEventUpdate(SoundStateDef defClass, IVisualEvent evt)
        {
            var gameObjectComponent = evt.casterGameObject.GetComponent<AmbientSoundController>();
            if (gameObjectComponent != null)
                gameObjectComponent.AddAmbientEvent(defClass.SoundEvent, 1, defClass.Duration);
        }
    }

    [UsedImplicitly]
    class SoundEvent : IVisualEffectHandlerBinding<SoundEventDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public void OnEventUpdate(SoundEventDef defClass, IVisualEvent evt)
        {
            var casterGameObject = evt.casterGameObject;
            if (casterGameObject)
            {
                
                var soundProxy = casterGameObject.GetComponentInChildren<SoundEventProxy>();
                if (soundProxy)
                {
                    PostEvent(soundProxy.GetComponent<AkGameObj>(), defClass, evt);
                }
                else
                {
                    foreach (var akGameObj in casterGameObject.GetComponentsInChildren<AkGameObj>())
                        PostEvent(akGameObj, defClass, evt);
                }
            }
            else
                if(Logger.IsDebugEnabled)  Logger.IfError()?.Message("No casterGameObject for {event} with params {eventParams}", defClass, evt).Write();
        }

        private void PostEvent(AkGameObj soundAkGameObject, SoundEventDef defClass, IVisualEvent evt)
        {
            if (soundAkGameObject)
            {
                Logger.IfDebug()?.Message("SoundEvent {event} at {object}", defClass.SoundEvent, soundAkGameObject.gameObject.transform.FullName()).Write();
                if (defClass.Params != null)
                    foreach (var param in defClass.Params)
                        if (param.Value.Target != null)
                        {
                            var value = param.Value.Target.GetValue(evt.parameters);
                            Logger.IfTrace()?.Message("SoundEvent {event} set parameter '{param}' to {value} at {object}", defClass.SoundEvent, param.Key, value, soundAkGameObject.gameObject.transform.FullName()).Write();
                            SoundHelper.SetParameter(param.Key, value, soundAkGameObject.gameObject);
                        }
                AkSoundEngine.PostEvent(defClass.SoundEvent, soundAkGameObject.gameObject);
            }
            else
                if(Logger.IsDebugEnabled)  Logger.IfError()?.Message("No AkGameObj for {event} with params {eventParams}", defClass, evt).Write();;
        }
    }

    [UsedImplicitly]
    class PlaceParentedFX : IVisualEffectHandlerBinding<PlaceParentedFXDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public void OnEventUpdate(PlaceParentedFXDef defClass, IVisualEvent evt)
        {
            var objToInstantiate = Constants.WorldConstants?.MockFX?.Target ?? defClass.FX.Target;
            if (!objToInstantiate)
            {
                Logger.IfWarn()?.Write($"Bad FX path in PlaceParentedFX: {defClass.____GetDebugAddress()}");
                return;
            }

            var gObj = evt.casterGameObject;
            if (gObj == null)
                return;

            Transform parent;
            if (defClass.Parent.Target != null)
            {
                var parentPath = defClass.Parent.Target.GetValue(evt.parameters);
                Logger.IfDebug()?.Message($"Parent path: '{parentPath}'").Write();
                parent = string.IsNullOrWhiteSpace(parentPath) ? gObj.transform : gObj.transform.Find(parentPath);
                if (!parent)
                    parent = gObj.transform;
            }
            else
            {
                parent = gObj.transform;
            }

            Vector3 position;
            if (defClass.LocalPosition.Target != null)
                position = parent.TransformPoint(defClass.LocalPosition.Target.GetValue(evt.parameters).ToUnity());
            else 
            if (defClass.Position.Target != null)
                position = defClass.Position.Target.GetValue(evt.parameters).ToUnity();
            else
                position = evt.position;

            Quaternion rotation;
            if (defClass.LocalRotation.Target != null)
                rotation = parent.rotation * objToInstantiate.transform.rotation * defClass.LocalRotation.Target.GetValue(evt.parameters).ToUnity();
            else 
            if (defClass.Rotation.Target != null)
                rotation = defClass.Rotation.Target.GetValue(evt.parameters).ToUnity() * objToInstantiate.transform.rotation;
            else
                rotation = evt.rotation * objToInstantiate.transform.rotation;
            
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Instantiate {objToInstantiate} at {position} with rot {rotation} attached to {parent}").Write();
            
            var hitEffectObject = GameObject.Instantiate(objToInstantiate, position, rotation, parent);
            if (hitEffectObject)
            {
                if (defClass.DestroyDelay > 0)
                {
                    var remover = hitEffectObject.AddComponent<ObjectDelayedRemover>();
                    remover.DestroyWithDelay(defClass.DestroyDelay);
                }
                if (defClass.SubFX?.Length > 0)
                {
                    var wrapper = new VisualEventSubWrapper(evt, hitEffectObject);
                    foreach (var subFx in defClass.SubFX)
                        VisualEffectHandlerCollection.Get(subFx).OnEventUpdate(subFx, wrapper);
                }
            }
            else
                Logger.IfError()?.Message($"Can't instantiate FX {defClass} with prefab {objToInstantiate}").Write();
        }
    }

    [UsedImplicitly]
    class PlaceUnparentedFX : IVisualEffectHandlerBinding<PlaceUnparentedFXDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnEventUpdate(PlaceUnparentedFXDef defClass, IVisualEvent evt)
        {
            var prefab = Constants.WorldConstants?.MockFX?.Target ?? defClass.FX.Target;
            if (!prefab)
            {
                Debug.LogWarning($"Bad FX path in PlaceParentedFX: {defClass.____GetDebugAddress()}");
                return;
            }
            
            Vector3 position;
            Quaternion rotation;
            if (defClass.RelativeToGameObject)
            {
                position = evt.casterGameObject.transform.position + evt.casterGameObject.transform.TransformVector((Vector3)defClass.Shift);
                rotation = evt.casterGameObject.transform.rotation * prefab.transform.rotation;
            }
            else
            {
                if (defClass.Position.Target != null)
                    position = defClass.Position.Target.GetValue(evt.parameters).ToUnity();
                else
                    position = evt.position;
            
                if (defClass.Rotation.Target != null)
                    rotation = defClass.Rotation.Target.GetValue(evt.parameters).ToUnity();
                else
                    rotation = evt.rotation;

                position += (Vector3) defClass.Shift;
                rotation = rotation * prefab.transform.rotation;
            }

            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Instantiate {prefab} at {position} with rot {rotation}").Write();

            var hitEffectObject = GameObject.Instantiate(prefab, position, rotation);
            
            if (hitEffectObject != null)
            {
                hitEffectObject.SetActive(true);
                if (defClass.DestroyDelay != 0)
                {
                    var remover = hitEffectObject.AddComponent<ObjectDelayedRemover>();
                    remover.DestroyWithDelay(defClass.DestroyDelay);
                }
                if (defClass.SubFX?.Length > 0)
                {
                    var wrapper = new VisualEventSubWrapper(evt, hitEffectObject);
                    foreach (var subFx in defClass.SubFX)
                        VisualEffectHandlerCollection.Get(subFx).OnEventUpdate(subFx, wrapper);
                }
            }
        }
    }

    [UsedImplicitly]
    class PlaceUnparentedFXWithTarget : IVisualEffectHandlerBinding<PlaceUnparentedFXWithTargetDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnEventUpdate(PlaceUnparentedFXWithTargetDef defClass, IVisualEvent evt)
        {
            var decal = Constants.WorldConstants?.MockFX?.Target ?? defClass.FX.Target;
            if (decal == default(GameObject))
                return;
            if (evt.targetGameObject == default(GameObject) || evt.casterGameObject == default(GameObject))
                return;

            var direction = evt.targetGameObject.transform.position - evt.casterGameObject.transform.position;
            var rotation = Quaternion.LookRotation(direction) * decal.transform.rotation;
            var position = evt.casterGameObject.transform.position + (Vector3) defClass.Shift;
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Instantiate {decal} at {position} with rot {rotation}").Write();
            var hitEffectObject = GameObject.Instantiate(decal, position, rotation);
            if (hitEffectObject != default(GameObject))
            {
                if (defClass.DestroyDelay != 0)
                {
                    var remover = hitEffectObject.AddComponent<ObjectDelayedRemover>();
                    remover.DestroyWithDelay(defClass.DestroyDelay);
                }
                if (defClass.SubFX?.Length > 0)
                {
                    var wrapper = new VisualEventSubWrapper(evt, hitEffectObject);
                    foreach (var subFx in defClass.SubFX)
                        VisualEffectHandlerCollection.Get(subFx).OnEventUpdate(subFx, wrapper);
                }
            }
        }
    }

    [UsedImplicitly]
    class SetFXOnDestroy : IVisualEffectHandlerBinding<SetFXOnDestroyDef>
    {
        public void OnEventUpdate(SetFXOnDestroyDef defClass, IVisualEvent evt)
        {
            var FXRef = defClass.FX;
            if (FXRef == default(UnityRef<GameObject>) || defClass.FX.Target == default(GameObject))
                return;
            if (evt.casterGameObject == default(GameObject))
                return;
            var destroyFXPlayer = evt.casterGameObject.GetComponent<DestroyFXPlayer>();
            if (destroyFXPlayer == default(DestroyFXPlayer))
                return;
            destroyFXPlayer._onDestroyFx = FXRef.Target;
            destroyFXPlayer._shift = (Vector3)defClass.Shift;
            destroyFXPlayer._destroyDelay = defClass.DestroyDelay;
        }
    }

    [UsedImplicitly]
    class DisableVisual : IVisualEffectHandlerBinding<DisableVisualDef>
    {
        public void OnEventUpdate(DisableVisualDef defClass, IVisualEvent evt)
        {
            var gO = evt.casterGameObject;
            if (gO == null)
                return;
            var renderers = gO.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
                renderer.enabled = false;
        }
    }

    internal static class PlaceFXHelper
    {
        public static Transform FindNearestGameObjectTransform(GameObject root, Vector3 position, Predicate<Transform> predicate = null)
        {
            return FindNearestTransform(root.transform, position, predicate).Item1;
        }

        public static Transform FindNearestGameObjectTransform(Transform root, Vector3 position, Predicate<Transform> predicate = null)
        {
            return FindNearestTransform(root, position, predicate).Item1;
        }
        
        private static (Transform,float) FindNearestTransform(Transform root, Vector3 position, Predicate<Transform> predicate)
        {
            var nearestTransform = (predicate == null || predicate(root)) ? root : null; 
            float minMagnitude = nearestTransform ? (position - nearestTransform.position).sqrMagnitude : float.MaxValue;
            for (int i=0, cnt = root.childCount; i < cnt; ++i)
            {
                var (transform, magnitude) = FindNearestTransform(root.GetChild(i), position, predicate); 
                if (magnitude < minMagnitude)
                {
                    minMagnitude = magnitude;
                    nearestTransform = transform;
                }
            }
            return (nearestTransform, minMagnitude);
        }
    }
}