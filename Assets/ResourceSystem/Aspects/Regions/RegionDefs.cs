using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;

namespace SharedCode.Aspects.Regions
{
    public abstract class ARegionDef : BaseResource
    {
        public ResourceRef<ARegionDef>[] ChildRegions { get; set; }
        public ResourceRef<ARegionDataDef>[] Data { get; set; }
    }

    public class RootRegionDef : ARegionDef { }


    /*-------------------------------------------------- SpatialTableRegion --------------------------------------------------*/
    public abstract class SpatialTableRegionDef : ARegionDef
    {
        //public ObjectContainerDef<IRegionDef> SpatialTable { get; set; }
    }

    public class GeoRegionRootDef : SpatialTableRegionDef { }
    /*-------------------------------------------------- --------------------------------------------------*/
    

    ///*-------------------------------------------------- ObjectContainer --------------------------------------------------*/
    //public abstract class ObjectContainerDef<T> : BaseResource { }

    //public class BoundlessContainerDef : ObjectContainerDef<IRegionDef>
    //{
    //    public IRegionDef[] BoundlessRegions { get; set; }
    //}

    //public class AABBHashedContainerDef : ObjectContainerDef<IRegionDef>
    //{
    //}
    ///*-------------------------------------------------- --------------------------------------------------*/


    /*-------------------------------------------------- GeoRegion --------------------------------------------------*/
    public abstract class GeoRegionDef : ARegionDef
    {
        public ResourceRef<BoundingBoxDef> AABB { get; set; }
    }

    public class GeoFolderDef : GeoRegionDef
    { }

    public class GeoSphereDef : GeoRegionDef
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
    }

    public class GeoBoxDef : GeoRegionDef
    {
        public Vector3 Center { get; set; }
        public Quaternion InverseRotation { get; set; }
        public Vector3 Extents { get; set; }
    }

    public class GeoTextureMaskDef : GeoBoxDef
    {
        public int TexHeight { get; set; }
        public int TexWidth { get; set; }
        public string TexData { get; set; }

        [JsonIgnore]
        private byte[] _textureByteArray;
        [JsonIgnore]
        public byte[] TextureByteArray
        {
            get
            {
                if (_textureByteArray == null)
                    _textureByteArray = Convert.FromBase64String(TexData.Decompress());
                return _textureByteArray;
            }
        }
    }
    /*-------------------------------------------------- --------------------------------------------------*/

    public class BoundingBoxDef : BaseResource
    {
        public Vector3 StartCoords { get; set; }
        public Vector3 Dimensions { get; set; }
    }


    /*-------------------------------------------------- RegionData --------------------------------------------------*/
    public abstract class ARegionDataDef : BaseResource { }

    public class FogOfWarRegionDef : ARegionDataDef
    {
    }
    
    public class BuildBlockerDef : ARegionDataDef
    {
        public bool BlockBuildings { get; set; }
        public bool BlockFences { get; set; }
        public bool BlockWorkbenches { get; set; }
        public bool BlockObelisks { get; set; }
        public bool BlockBakens { get; set; }
        public bool BlockChests { get; set; }
    }

    public class SpellCastRegionDef : ARegionDataDef
    {
        public ResourceRef<SpellDef> OnEnterSpellDef { get; set; }
        public ResourceRef<SpellDef> OnExitSpellDef { get; set; }
        public ResourceRef<SpellDef> WhileInsideSpellDef { get; set; }
    }

    public class SoundRegionDataDef : ARegionDataDef
    {
        public RegionSoundStateDef[] Switches { get; set; }
        public RegionSoundStateDef[] States { get; set; }
    }

    [KnownToGameResources]
    public struct RegionSoundStateDef : IEquatable<RegionSoundStateDef>
    {
        public uint _groupID;
        public uint _valueID;
        
        public bool Equals(RegionSoundStateDef other) => _groupID == other._groupID && _valueID == other._valueID;
    }
    /*-------------------------------------------------- --------------------------------------------------*/
}
