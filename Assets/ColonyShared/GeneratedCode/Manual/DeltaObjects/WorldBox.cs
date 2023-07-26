using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.Logging;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldBox : IHookOnInit
    {
        public Task OnInit()
        {
            return Task.CompletedTask;
        }

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public ItemSpecificStats SpecificStats => ((WorldBoxDef)Def).DefaultStats;
    }
}