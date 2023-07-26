using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Character;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode.Aspects.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Effects
{
    [UsedImplicitly]
    public class EffectEnableBodyPitch: IEffectBinding<EffectEnableBodyPitchDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectEnableBodyPitchDef def)
        {
            if (cast.OnClient() && cast.Caster.HasClientAuthority(repo))
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var view = GetCharacterView(cast);
                    if (view != null)
                        view.BindBodyPitchWithCamera = true;
                });
            }
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectEnableBodyPitchDef def)
        {
            if (cast.OnClient() && cast.Caster.HasClientAuthority(repo))
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    var view = GetCharacterView(cast);
                    if (view != null)
                        view.BindBodyPitchWithCamera = false;
                });
            }
            return new ValueTask();
        }

        private IPitchableView GetCharacterView(SpellWordCastData cast)
        {
            return cast.GetCaster().GetComponent<ISubjectPawn>()?.View.Value as IPitchableView;
        }
    }
}