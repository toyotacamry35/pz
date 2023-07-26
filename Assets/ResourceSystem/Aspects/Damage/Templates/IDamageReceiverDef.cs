using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public interface IDamageReceiverDef : IComponentDef
    {
    }

    public interface INetworkDamageReceiverDef : IDamageReceiverDef
    {
    }
    
    public abstract class BaseNetworkDamageReceiverDef : ComponentDef, INetworkDamageReceiverDef
    {
        public ResourceRef<IHealthLikeOwnerDef> HealthOwner { get; set; }
    }
    
    public class NetworkDamageReceiverDef : BaseNetworkDamageReceiverDef
    {
    
    }
}
