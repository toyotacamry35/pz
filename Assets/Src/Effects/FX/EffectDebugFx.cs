using Assets.Src.ManualDefsForSpells;
using ResourcesSystem.Loader;
using Assets.Src.Shared.Impl;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using SharedCode.Entities.Engine;
using Assets.Src.Lib.Cheats;
using SharedCode.Aspects.Item.Templates;
using Core.Cheats;
using JetBrains.Annotations;

namespace Assets.Src.Effects.FX
{
    [UsedImplicitly, PredictableEffect]
    public class EffectDebugFx : IClientOnlyEffectBinding<EffectDebugFxDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugFxDef indef)
        {
            var def = indef;
            GameObject castergo = null;
            GameObject targetgo = null;
            OuterRef<IEntity> targetEntityRef = default(OuterRef<IEntity>);
            Vector3 position = default(Vector3);
            Quaternion rotation = default(Quaternion);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                var attachTo = castergo.transform;
                if(!string.IsNullOrWhiteSpace(indef.AttachmentObj))
                {
                    attachTo = castergo.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name == indef.AttachmentObj);
                    if (attachTo == null)
                        attachTo = castergo.transform;
                }
                var go = GameObject.Instantiate<GameObject>(indef.FxObj.Target, attachTo, false);
                go.name = indef.FxObj.Target.name + $"{cast.SpellId}{GameResourcesHolder.Instance.GetIDWithCrc(indef).NetCrc64Id}";
                
            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugFxDef indef)
        {
            var def = indef;
            GameObject castergo = null;
            GameObject targetgo = null;
            OuterRef<IEntity> targetEntityRef = default(OuterRef<IEntity>);
            Vector3 position = default(Vector3);
            Quaternion rotation = default(Quaternion);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                var idstring = indef.FxObj.Target.name + $"{cast.SpellId}{GameResourcesHolder.Instance.GetIDWithCrc(indef).NetCrc64Id}";
                var obj = castergo.GetComponentsInChildren<Transform>().FirstOrDefault(x=>x.name == idstring);
                if (obj != null)
                    GameObject.Destroy(obj.gameObject);
            });

            return new ValueTask();
        }
    }

    [UsedImplicitly, PredictableEffect]
    public class EffectDebugTint : IClientOnlyEffectBinding<EffectDebugTintDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static bool _showTint = Constants.WorldConstants.EnableTintByDefault;
        [Cheat]
        public static void EnableTint()
        {
            _showTint = true;
        }

        [Cheat]
        public static void DisableTint()
        {
            _showTint = false;
        }
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugTintDef indef)
        {
            if (!_showTint && !indef.IgnoreTintDisablment)
                return new ValueTask();
            if (!cast.OnClient())
                return new ValueTask();

            var def = indef;
            GameObject castergo = null;
            GameObject targetgo = null;
            OuterRef<IEntity> targetEntityRef = default(OuterRef<IEntity>);
            Vector3 position = default(Vector3);
            Quaternion rotation = default(Quaternion);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                var attachTo = castergo.transform;
                castergo.GetComponentInChildren<DebugTint>().StartTint(
                    new ModifierCauser() { SpellId = cast.SpellId.Counter, Causer = indef }, 
                    new Color() { a = 1, r = indef.Color.R, g = indef.Color.G, b = indef.Color.B } );

            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDebugTintDef indef)
        {
            if (!_showTint && !indef.IgnoreTintDisablment)
                return new ValueTask();
            if (!cast.OnClient())
                return new ValueTask();

            var def = indef;
            GameObject castergo = null;
            GameObject targetgo = null;
            OuterRef<IEntity> targetEntityRef = default(OuterRef<IEntity>);
            Vector3 position = default(Vector3);
            Quaternion rotation = default(Quaternion);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                castergo.GetComponentInChildren<DebugTint>().StopTint(new ModifierCauser() { SpellId = cast.SpellId.Counter, Causer = indef });
            });

            return new ValueTask();
        }
    }
}
