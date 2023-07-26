using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.FX.Decals
{
    public interface IHitDecalPlacerDef : IResource
    {    }

    public class SimplifiedHitDecalPlacerDef : BaseResource, IHitDecalPlacerDef { }

    public class BasicHitDecalPlacerDef : BaseResource, IHitDecalPlacerDef { }

}
