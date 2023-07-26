using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using UnityEngine;
using SVector3 = SharedCode.Utils.Vector3;


namespace Assets.Src.Regions.RegionMarkers
{
    public class GeoRegionMarker : RegionMarker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override ARegionDef BuildDefs()
        {
            var regDef = GetGeoRegionDefinition();
            var childRegions = GetChildMarkers();
            if (regDef is GeoFolderDef && childRegions.Count == 0)
                Logger.Error(
                    $"{nameof(GameObject)} '{gameObject.name}' does not contain a collider and any childrens. This is probably a mistake and this region should be deleted."
                );

            var aabbDef = GetBoundingBox();
            regDef.AABB = aabbDef;

            regDef.ChildRegions = childRegions
                .Select(childReg => (ResourceRef<ARegionDef>) childReg.BuildDefs())
                .ToArray();

            regDef.Data = GetRegionData();
            return regDef;
        }

        // !!!!!!!!!! This section is commented out due to bug in Unity 2019.1.7f1 which causes collider.bounds.size (and collider.bounds.extents respectively) to be (0, 0, 0).
        // !!!!!!!!!! Alternative variant to get this values is implemented below.
        //public virtual BoundingBoxDef GetBoundingBox()
        //{
        //    var collider = GetComponent<Collider>();
        //    if (collider == default(Collider))
        //    {
        //        Debug.LogError($"{gameObject.name} does not have any colliders");
        //        return default(BoundingBoxDef);
        //    }
        //    return new BoundingBoxDef { StartCoords = (SharedCode.Utils.Vector3)collider.bounds.min, Dimensions = (SharedCode.Utils.Vector3)collider.bounds.size };
        //}


        public virtual BoundingBoxDef GetBoundingBox()
        {
            var collider = GetComponent<Collider>();
            if (collider == default(Collider))
            {
                //Debug.LogWarning($"{gameObject.name} does not have any colliders");
                return default(BoundingBoxDef);
            }

            return GeoRegionExporter.GetBoundingBoxBasedOnCollider(collider, transform);
        }

        public virtual GeoRegionDef GetGeoRegionDefinition()
        {
            var collider = GetComponent<Collider>();
            if (collider == default(Collider))
            {
                //Debug.LogWarning($"{gameObject.name} does not have any colliders");
                return new GeoFolderDef();
            }

            return GeoRegionExporter.GetGeoRegionDefFromGameObject(collider, gameObject);
        }
    }

    public static class GeoRegionExporter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static GeoRegionDef GetGeoRegionDefFromGameObject(Collider collider, GameObject gameObject)
        {
            var sphere = collider as SphereCollider;
            if (sphere != default(SphereCollider))
            {
                if (gameObject.transform.lossyScale != gameObject.transform.lossyScale.x * Vector3.one)
                    Logger.Error(
                        "Sphere colliders does not display their bounds properly and does not work as expected when their scale differs from (1, 1, 1)." +
                        "You probably should change associated region {0} or it's parents.",
                        gameObject.name);
                return new GeoSphereDef
                    {Center = (SharedCode.Utils.Vector3) (sphere.center + gameObject.transform.position), Radius = sphere.radius};
            }

            var box = collider as BoxCollider;
            if (box != default(BoxCollider))
            {
                return new GeoBoxDef
                {
                    Center = (SharedCode.Utils.Vector3) (box.center + gameObject.transform.position),
                    Extents = new SharedCode.Utils.Vector3(Vector3.Scale(box.size, gameObject.transform.lossyScale) / 2f),
                    InverseRotation = (SharedCode.Utils.Quaternion) Quaternion.Inverse(gameObject.transform.rotation)
                };
            }

            Debug.LogError($"{gameObject.name} has unsuppotred collider type");
            return default(GeoRegionDef);
        }

        public static BoundingBoxDef GetBoundingBoxBasedOnCollider(Collider collider, Transform transform)
        {
            var sphereCollider = collider as SphereCollider;
            if (sphereCollider != null)
                return GeometryHelpers.GetBoundingBoxOfSphereWithTransform(
                    (SVector3) sphereCollider.center,
                    sphereCollider.radius,
                    (SharedCode.Entities.Transform) transform);

            var boxCollider = collider as BoxCollider;
            if (boxCollider != null)
            {
                if (boxCollider.center != new Vector3(0, 0, 0))
                    Logger.IfError()?.Message($"Please do not use 'Collider.Center' property of {nameof(BoxCollider)} (use Transform.Position instead)").Write();
                return GeometryHelpers.GetBoundingBoxOfBoxWithTransform(
                    (SVector3) boxCollider.size,
                    (SharedCode.Entities.Transform) transform);
            }

            throw new NotImplementedException(
                $"Only '{nameof(BoxCollider)}' and '{nameof(SphereCollider)}' types supported in {nameof(GeoRegionMarker)} component.");
        }
    }
}