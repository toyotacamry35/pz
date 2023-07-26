using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Uins;

namespace Assets.Src.Impacts.Specific
{
    public class ImpactSetHudVisibility : IImpactBinding<ImpactSetHudVisibilityDef>
    {
        public ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSetHudVisibilityDef indef)
        {
            return new ValueTask();
            // var def = (ImpactSetHudVisibilityDef) indef;
            // await UnityQueueHelper.RunInUnityThread(() =>
            // {
            //     if (def.Enable)
            //         HudVisibility.Instance.VisibleOn(def.Hud);
            //     else
            //         HudVisibility.Instance.VisibleOff(def.Hud);
            // });
            //
            // return true;
        }
    }
}
