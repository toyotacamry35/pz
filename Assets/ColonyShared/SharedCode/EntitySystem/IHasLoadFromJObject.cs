using System.Threading.Tasks;
using SharedCode.Config;

namespace SharedCode.EntitySystem
{
    public interface IHasLoadFromJObject
    {
        Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository);
    }
}
