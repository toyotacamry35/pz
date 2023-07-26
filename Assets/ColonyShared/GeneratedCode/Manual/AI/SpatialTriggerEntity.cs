using SharedCode.Entities.Regions;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class SpatialTriggerEntity : ISpatialTriggerEntityImplementRemoteMethods, IHookOnInit
    {
        public async Task OnInit()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Guid.NewGuid(), async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); });
        }

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
    }
}
