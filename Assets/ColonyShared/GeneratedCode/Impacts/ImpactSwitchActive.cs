using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Impacts
{
    public class ImpactSwitchActive : IImpactBinding<ImpactSwitchActiveDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSwitchActiveDef def)
        {
            if (cast.IsSlave)
                return;

            var buildingId = cast.Caster.Guid;
            using (var wrapper = await repo.Get<IWorldMachineServer>(buildingId))
            {
                var worldBuilding = wrapper.Get<IWorldMachineServer>(buildingId);
                await worldBuilding.SetActive(!worldBuilding.IsActive);
            }
            }
    }
}