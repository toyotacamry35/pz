using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class SimpleWorldEntity
    {
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
