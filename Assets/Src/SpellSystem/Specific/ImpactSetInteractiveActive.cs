using Assets.Src.Aspects.Impl;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Assets.Src.Impacts.Specific
{
    public class ImpactSetInteractiveActive : IImpactBinding<ImpactSetInteractiveActiveDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSetInteractiveActiveDef indef)
        {
            var def = (ImpactSetInteractiveActiveDef) indef;
            var interactiveGO = def.Target.Target.GetGo(cast);

            await UnityQueueHelper.RunInUnityThread(() =>
            {
                var interactive = interactiveGO.GetComponent<Interactive>();
                if (interactive != null)
                    interactive.enabled = def.Enable;
            });
        }
    }
}
