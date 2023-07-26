using System.Threading.Tasks;
using JetBrains.Annotations;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Shared.ManualDefsForSpells;

namespace Assets.Src.Impacts
{
    [UsedImplicitly]
    public class ImpactGrantItem : IImpactBinding<ImpactGrantItemDef>
    {
        public ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactGrantItemDef indef)
        {
            return new ValueTask();
        }
    }
}
