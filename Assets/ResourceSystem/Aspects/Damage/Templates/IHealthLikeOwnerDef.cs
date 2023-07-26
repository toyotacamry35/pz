using Assets.Src.GameObjectAssembler.Res;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public interface IHealthLikeOwnerDef : IComponentDef
    {
    }

    public interface INetworkHealthLikeOwnerDef : IHealthLikeOwnerDef
    {
    }

    public abstract class NetworkHealthLikeOwnerDef : ComponentDef, INetworkHealthLikeOwnerDef
    {
    }
}
