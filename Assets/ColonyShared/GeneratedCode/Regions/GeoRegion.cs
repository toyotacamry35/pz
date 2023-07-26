using System.Collections.Generic;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;
using SharedCode.Utils;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public interface GeoRegion : IRegion
    {
        BoundingBox AABB { get; set; }
    }

    public class GeoFolder : BaseRegion, GeoRegion
    {
        public BoundingBox AABB { get => default; set => AABB = value; }

        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            if (IsInside(pointCoords))
            {
                regions.Add(this);
                if (ChildRegions != null)
                    foreach (var childRegion in ChildRegions)
                        childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
            }
        }
        
        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as GeoFolderDef;
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 coords) => false;
    }

    public class GeoSphere : BaseRegion, GeoRegion
    {
        public BoundingBox AABB { get; set; }
        public Vector3 Center { get; private set; }
        public float Radius { get; private set; }

        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            if (IsInside(pointCoords))
            {
                regions.Add(this);
                if (ChildRegions != null)
                    foreach (var childRegion in ChildRegions)
                        childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
            }
        }

        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as GeoSphereDef;
            Center = defClass.Center + providedTransform.Position;
            Radius = defClass.Radius;
            if (defClass.AABB == null)
                AABB = GeometryHelpers.GetBoundingBoxOfSphereWithTransform(defClass.Center, Radius, providedTransform);
            else
                AABB = new BoundingBox { StartCoords = defClass.AABB.Target.StartCoords, Dimensions = defClass.AABB.Target.Dimensions };
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 pointCoords) => GeometryHelpers.IsPointInsideSphere(pointCoords, Center, Radius);
    }

    public class GeoBox : BaseRegion, GeoRegion
    {
        public BoundingBox AABB { get; set; }
        public Vector3 Center { get; private set; }
        public Quaternion InverseRotation { get; private set; }
        public Vector3 Extents { get; private set; }

        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            if (IsInside(pointCoords))
            {
                regions.Add(this);
                if (ChildRegions != null)
                    foreach (var childRegion in ChildRegions)
                        childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
            }
        }
        
        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as GeoBoxDef;
            Center = defClass.Center + providedTransform.Position;
            InverseRotation = defClass.InverseRotation;
            Extents = defClass.Extents;
            var globalT = new Transform(providedTransform.Position + defClass.Center, providedTransform.Rotation, providedTransform.Scale);
            if (defClass.AABB == null)
                AABB = GeometryHelpers.GetBoundingBoxOfBoxWithTransform(Extents * 2, globalT);
            else
                AABB = new BoundingBox { StartCoords = defClass.AABB.Target.StartCoords, Dimensions = defClass.AABB.Target.Dimensions };
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 pointCoords) => GeometryHelpers.IsPointInsideBox(pointCoords, Center, Extents, InverseRotation);
    }

    public class GeoTextureMask : BaseRegion, GeoRegion
    {
        public BoundingBox AABB { get; set; }

        public override void GetAllContainingRegionsNonAlloc(List<IRegion> regions, Vector3 pointCoords)
        {
            if (IsInside(pointCoords))
            {
                regions.Add(this);
                if (ChildRegions != null)
                    foreach (var childRegion in ChildRegions)
                        childRegion.GetAllContainingRegionsNonAlloc(regions, pointCoords);
            }
        }
        
        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            var defClass = def as GeoTextureMaskDef;
            if (defClass.AABB == null)
                AABB = GeometryHelpers.GetBoundingBoxOfBoxWithTransform(defClass.Extents * 2, providedTransform);
            else
                AABB = new BoundingBox { StartCoords = defClass.AABB.Target.StartCoords, Dimensions = defClass.AABB.Target.Dimensions };
            RegionDef = defClass;
        }

        public override bool IsInside(Vector3 pointCoords)
        {
            if (RegionDef is GeoTextureMaskDef defClass &&
                GeometryHelpers.PointToRectPos(
                    pointCoords,
                    defClass.Center,
                    defClass.Extents,
                    defClass.InverseRotation,
                    defClass.TexWidth,
                    defClass.TexHeight,
                    out var x, out var y))
            {
                var textureByteArray = defClass.TextureByteArray;
                var bitNumberInArray = x + y * defClass.TexWidth;
                var byteNumberInArray = bitNumberInArray / 8;
                if (0 > byteNumberInArray || byteNumberInArray >= textureByteArray.Length)
                    return false;

                return (textureByteArray[byteNumberInArray] & (1 << (bitNumberInArray % 8))) != 0;
            }

            return false;
        }
    }

    public class BoundingBox
    {
        public Vector3 StartCoords { get; set; }
        public Vector3 Dimensions { get; set; }
        public static implicit operator BoundingBoxDef(BoundingBox input)
        {
            return new BoundingBoxDef() { Dimensions = input.Dimensions, StartCoords = input.StartCoords };
        }
    }
}