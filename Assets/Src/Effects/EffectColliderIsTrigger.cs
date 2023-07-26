using System;
using System.Threading.Tasks;
using Assets.Src.Wizardry;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.Effects
{
    public class EffectDisableColliderBase
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Attach(GameObject cgo)
        {
            if (cgo != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var mobCollider = cgo.GetComponent<Collider>();
                    if (mobCollider == null)
                    {
                        Logger.IfError()?.Message($"Caster hasn't `{nameof(Collider)}`.").Write();
                        return;
                    }

                    if (!mobCollider.enabled)
                    {
                        Logger.IfInfo()?.Message/*Warn*/($"#Warn: `{nameof(Collider)}` .{nameof(Collider.enabled)} is `false` already. [ It's Ok on Host ]")
                            .Write();
                        return;
                    }

                    mobCollider.enabled = false;
                });
        }
        
        protected void Detach(GameObject cgo)
        {
            if (cgo != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var mobCollider = cgo.GetComponent<Collider>();
                    if (mobCollider == null)
                    {
                        Logger.IfError()?.Message($"Caster hasn't `{nameof(Collider)}`.").Write();
                        return;
                    }

                    if (mobCollider.enabled)
                    {
                        Logger.IfInfo()?.Message/*Warn*/($"#Warn: `{nameof(Collider)}` .{nameof(Collider.enabled)} is `true` already. [ It's Ok on Host ]")
                            .Write();
                        return;
                    }

                    mobCollider.enabled = true;
                });
        }
    }

    [UsedImplicitly]
    public class EffectDisableCollider : EffectDisableColliderBase, IEffectBinding<EffectDisableColliderDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectDisableColliderDef def)
        {
            Attach(cast.GetCaster());
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectDisableColliderDef def)
        {
            Detach(cast.GetCaster());
            return new ValueTask();
        }
    }
    
    [Obsolete("Use da EffectDisableCollider")]
    public class EffectColliderIsTrigger : EffectDisableColliderBase, IEffectBinding<EffectColliderIsTriggerDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectColliderIsTriggerDef def)
        {
            Attach(cast.GetCaster());
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectColliderIsTriggerDef def)
        {
            Detach(cast.GetCaster());
            return new ValueTask();
        }
    }
}
