using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using TimeUnits = System.Int64;

namespace Assets.Src.Effects
{
    [UsedImplicitly, PredictableEffect]

    public class EffectShowDebugTexture : IClientOnlyEffectBinding<EffectShowDebugTextureDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowDebugTextureDef indef)
        {
            UnityQueueHelper.RunInUnityThread(() =>
            {
                var def = indef;
                var targetGo = def.Target.Target?.GetGo(cast) ?? cast.GetCaster();
                if (targetGo)
                {
                    var view = targetGo.Kind<CharacterPawn>();
                    if (view)
                    {
                        if (def.Texture.Target && !view.DebugTextures.ContainsKey(this))
                            view.DebugTextures.Add(this, def.Texture.Target);
                    }
                }
            });
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowDebugTextureDef indef)
        {
            UnityQueueHelper.RunInUnityThread(() =>
            {
                var def = indef;
                var targetGo = def.Target.Target?.GetGo(cast) ?? cast.GetCaster();
                if (targetGo)
                {
                    var view = targetGo.Kind<CharacterPawn>();
                    if (view)
                    {
                        if (def.Texture.Target && view.DebugTextures.ContainsKey(this))
                            view.DebugTextures.Remove(this);
                    }
                }
            });
            return new ValueTask();
        }
    }
}