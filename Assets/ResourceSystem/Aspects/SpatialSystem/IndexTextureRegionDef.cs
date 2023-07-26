using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.SpatialSystem
{
    public class IndexTextureRegionDef : RectIndexRegionDef
    {
        public ResourceRef<BinaryResource<SVO>> TexData { get; set; }
    }
}