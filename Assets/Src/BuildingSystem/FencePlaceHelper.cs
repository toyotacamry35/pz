using Assets.Src.Shared;
using Assets.Src.Tools;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{

    public static class FencePlaceHelper
    {
        private static float AdditionalElementRotation = 270.0f;
        public static float SqrNearLinkDifference = (0.5f * 0.5f); // for checking same points
        private static float boxColliderScale = 1.1f;  // >1.0 make collide something outside original collision box
        private static float terrainCollisionDistance = 1.0f;
        private static float terrainCollisionDistanceFar = 4.0f;

        private static void CreateLink(FenceLinkData data)
        {
            if (data.Def != null)
            {
                var result = GameObjectInstantiate.Invoke(data.Def.Prefab, data.Def.PrefabDef, data.Position, data.Rotation, false);

                var fenceLinkBehaviour = result.AddComponent<FenceLinkBehaviour>();
                fenceLinkBehaviour.SetData(data);

                data.GameObject = result;

                result.SetActive(true);
            }
        }

        private static void DestroyLink(FenceLinkData data, bool destroyed)
        {
            if (destroyed)
            {
                var link = data.GameObject.GetComponent<FenceLinkBehaviour>();
                if (link != null)
                {
                    destroyed = link.DestroyGameObject();
                }
            }
            if (!destroyed)
            {
                UnityEngine.Object.Destroy(data.GameObject);
            }
        }

        private static List<KeyValuePair<Vector3, bool>> GetNearPoints(List<KeyValuePair<Vector3, bool>> points, List<KeyValuePair<Vector3, bool>> otherPoints)
        {
            var result = new List<KeyValuePair<Vector3, bool>>();
            foreach (var point in points)
            {
                foreach (var otherPoint in otherPoints)
                {
                    var difference = new Vector2(point.Key.x - otherPoint.Key.x, point.Key.z - otherPoint.Key.z);
                    if (difference.sqrMagnitude < FencePlaceHelper.SqrNearLinkDifference)
                    {
                        result.Add(new KeyValuePair<Vector3, bool>((point.Key + otherPoint.Key) / 2.0f, point.Value && otherPoint.Value));
                    }
                }
            }
            return result;
        }

        private static List<KeyValuePair<Vector3, bool>> GetLinkPoints(this FencePlaceholderData data, bool global)
        {
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            var result = new List<KeyValuePair<Vector3, bool>>(def.LinkPoints.Count);
            foreach (var defLinkPoint in def.LinkPoints)
            {
                var linkPoint = new Vector3(defLinkPoint.X, defLinkPoint.Y, defLinkPoint.Z);
                linkPoint = data.Rotation * linkPoint;
                if (global)
                {
                    linkPoint = linkPoint + data.Position;
                }
                result.Add(new KeyValuePair<Vector3, bool>(linkPoint, defLinkPoint.AddLink));
            }
            return result;
        }

        private static List<KeyValuePair<Vector3, bool>> GetLinkPoints(this FenceElementData data, bool global)
        {
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            var result = new List<KeyValuePair<Vector3, bool>>(def.LinkPoints.Count);
            foreach (var defLinkPoint in def.LinkPoints)
            {
                var linkPoint = new Vector3(defLinkPoint.X, defLinkPoint.Y, defLinkPoint.Z);
                linkPoint = data.ElementGameObject.GetRotation() * linkPoint;
                if (global)
                {
                    linkPoint = linkPoint + data.ElementGameObject.GetPosition();
                }
                result.Add(new KeyValuePair<Vector3, bool>(linkPoint, defLinkPoint.AddLink));
            }
            return result;
        }

        private static List<FenceLinkData> GetInitialLinkPoints(this FenceElementData data)
        {
            var links = new List<FenceLinkData>();
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            foreach (var linkPoint in def.LinkPoints)
            {
                if (linkPoint.InitialLink)
                {
                    var position = new Vector3(linkPoint.X, linkPoint.Y, linkPoint.Z);
                    var linkData = new FenceLinkData();
                    linkData.Def = def.LinkDef.Target;
                    linkData.FracturedChunkScale = SharedCode.Utils.BuildUtils.BuildParamsDef.FracturedChunkScale;
                    linkData.Rotation = data.ElementGameObject.GetRotation();
                    linkData.Position = data.ElementGameObject.GetPosition() + linkData.Rotation * position - linkData.Rotation * (Vector3)(linkData.Def.LinkPoint);
                    links.Add(linkData);
                }
            }
            return links;
        }

        private static bool GetCollisions(this FenceLinkData data, bool clear, List<FenceElementData> fenceCollisions, FenceElementData ignore)
        {
            var fencesFound = 0;
            if (clear)
            {
                fenceCollisions?.Clear();
            }
            var colliders = Physics.OverlapBox(data.GameObject.transform.position + (Vector3)(data.Def.Center), PhysicsChecker.CheckExtentsSize((Vector3)(data.Def.Extents) * boxColliderScale, data.Def.ToString()), data.GameObject.transform.rotation, PhysicsLayers.PlaceBuildElementMask);
            if ((colliders != null) && (colliders.Length > 0))
            {
                foreach (var collider in colliders)
                {
                    var fenceData = collider.GetComponentInParent<FenceElementBehaviour>()?.GetData();
                    if (fenceData != null)
                    {
                        if ((fenceData != ignore) && !fenceData.Placeholder && !fenceData.IsDestroyed())
                        {
                            fenceCollisions?.Add(fenceData);
                            ++fencesFound;
                        }
                    }
                }
            }
            //BuildSystem.Logger.IfError()?.Message($"[FencePlaceHelper]\tFenceLinkData.GetCollisions()\tfences found: {fencesFound}").Write();
            return (fencesFound > 0);
        }

        private static bool GetCollisions(this FenceElementData data, bool clear, List<FenceElementData> fenceCollisions, List<FenceLinkData> linkCollisions)
        {
            var fencesFound = 0;
            var linksFound = 0;
            if (clear)
            {
                fenceCollisions?.Clear();
                linkCollisions?.Clear();
            }
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            var colliders = Physics.OverlapBox(data.ElementGameObject.GetPosition() + (Vector3)(def.Center), PhysicsChecker.CheckExtentsSize((Vector3)(def.Extents) * boxColliderScale, def.ToString()), data.ElementGameObject.GetRotation(), PhysicsLayers.PlaceBuildElementMask);
            if ((colliders != null) && (colliders.Length > 0))
            {
                foreach (var collider in colliders)
                {
                    var fenceData = collider.GetComponentInParent<FenceElementBehaviour>()?.GetData();
                    if (fenceData != null)
                    {
                        if ((fenceData != data) && !fenceData.Placeholder && !fenceData.IsDestroyed())
                        {
                            fenceCollisions?.Add(fenceData);
                            ++fencesFound;
                        }
                    }
                    else
                    {
                        var fenceLinkBehaviour = collider.GetComponentInParent<FenceLinkBehaviour>();
                        var destroyed = fenceLinkBehaviour?.IsDestroyed() ?? true;
                        var linkData = fenceLinkBehaviour?.GetData() ?? null;
                        if ((linkData != null) && !destroyed)
                        {
                            linkCollisions?.Add(linkData);
                            ++linksFound;
                        }
                    }
                }
            }
            //BuildSystem.Logger.IfError()?.Message($"[FencePlaceHelper]\tFenceElementData.GetCollisions()\tfences found: {fencesFound}\tlinksFound: {linksFound}").Write();
            return ((fencesFound > 0) || (linksFound > 0));
        }

        // Helpers --------------------------------------------------------------------------------
        public static void CreateLinks(this FenceElementData data)
        {
            var fenceCollisions = new List<FenceElementData>();
            var linkCollisions = new List<FenceLinkData>();
            data.GetCollisions(false, fenceCollisions, linkCollisions);
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            var links = new List<FenceLinkData>();
            if (fenceCollisions.Count > 0)
            {
                var linkPoints = data.GetLinkPoints(true);
                foreach (var collisionData in fenceCollisions)
                {
                    var points = GetNearPoints(linkPoints, collisionData.GetLinkPoints(true));
                    foreach (var point in points)
                    {
                        if (point.Value)
                        {
                            var linkData = new FenceLinkData();
                            linkData.Def = def.LinkDef.Target;
                            linkData.FracturedChunkScale = SharedCode.Utils.BuildUtils.BuildParamsDef.FracturedChunkScale;
                            linkData.Rotation = Quaternion.Euler(0.0f, Mathf.LerpAngle(data.ElementGameObject.GetRotation().eulerAngles.y, collisionData.ElementGameObject.GetRotation().eulerAngles.y, 0.5f), 0.0f);
                            linkData.Position = point.Key - linkData.Rotation * (Vector3)(linkData.Def.LinkPoint);
                            links.Add(linkData);
                        }
                    }
                }
            }
            foreach (var linkPoint in def.LinkPoints)
            {
                if (linkPoint.InitialLink)
                {
                    var position = new Vector3(linkPoint.X, linkPoint.Y, linkPoint.Z);
                    var linkData = new FenceLinkData();
                    linkData.Def = def.LinkDef.Target;
                    linkData.FracturedChunkScale = SharedCode.Utils.BuildUtils.BuildParamsDef.FracturedChunkScale;
                    linkData.Rotation = data.ElementGameObject.GetRotation();
                    linkData.Position = data.ElementGameObject.GetPosition() + linkData.Rotation * position - linkData.Rotation * (Vector3)(linkData.Def.LinkPoint);
                    foreach (var link in links)
                    {
                        var difference = new Vector2(linkData.Position.x - link.Position.x, linkData.Position.z - link.Position.z);
                        if (difference.sqrMagnitude < FencePlaceHelper.SqrNearLinkDifference)
                        {
                            linkData.Existed = true;
                            break;
                        }
                    }
                    if (!linkData.Existed)
                    {
                        links.Add(linkData);
                    }
                }
            }
            var fenceCollisionsCheck = new List<FenceElementData>();
            foreach (var linkCollision in linkCollisions)
            {
                foreach (var link in links)
                {
                    if (!link.Existed)
                    {
                        var difference = new Vector2(linkCollision.GameObject.transform.position.x - link.Position.x, linkCollision.GameObject.transform.position.z - link.Position.z);
                        if (difference.sqrMagnitude < FencePlaceHelper.SqrNearLinkDifference)
                        {
                            link.Existed = true;
                            linkCollision.GetCollisions(true, fenceCollisionsCheck, null);
                            if (fenceCollisionsCheck.Count < 3)
                            {
                                if (linkCollision.GameObject != null)
                                {
                                    linkCollision.GameObject.transform.position = link.Position;
                                    linkCollision.GameObject.transform.rotation = link.Rotation;
                                }
                            }
                        }
                    }
                }
            }
            foreach (var link in links)
            {
                if (!link.Existed)
                {
                    CreateLink(link);
                }
            }
        }

        public static void DestroyLinks(this FenceElementData data, bool destroyed)
        {
            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            var fenceLinkDef = def.LinkDef.Target;
            var linkCollisions = new List<FenceLinkData>();
            data.GetCollisions(false, null, linkCollisions);
            if (linkCollisions.Count > 0)
            {
                var fenceCollisions = new List<FenceElementData>();
                foreach (var collisionData in linkCollisions)
                {
                    collisionData.GetCollisions(true, fenceCollisions, data);
                    if (fenceCollisions.Count < fenceLinkDef.MinLinks)
                    {
                        DestroyLink(collisionData, destroyed);
                    }
                    else if (fenceCollisions.Count == 2)
                    {
                        var points = GetNearPoints(fenceCollisions[0].GetLinkPoints(true), fenceCollisions[1].GetLinkPoints(true));
                        foreach (var point in points)
                        {
                            if (point.Value)
                            {
                                var _rotation = Quaternion.Euler(0.0f, Mathf.LerpAngle(fenceCollisions[0].ElementGameObject.GetRotation().eulerAngles.y, fenceCollisions[1].ElementGameObject.GetRotation().eulerAngles.y, 0.5f), 0.0f);
                                var _position = point.Key - _rotation * (Vector3)(fenceLinkDef.LinkPoint);
                                var difference = new Vector2(collisionData.GameObject.transform.position.x - _position.x, collisionData.GameObject.transform.position.z - _position.z);
                                if (difference.sqrMagnitude < FencePlaceHelper.SqrNearLinkDifference)
                                {
                                    collisionData.GameObject.transform.position = _position;
                                    collisionData.GameObject.transform.rotation = _rotation;
                                    break;
                                }
                            }
                        }
                    }
                    else if (fenceCollisions.Count == 1)
                    {
                        var fenceCollision = fenceCollisions[0];
                        var fenceCollisionDef = fenceCollision.BuildRecipeDef.ElementDef.Target as FenceElementDef;
                        foreach (var linkPoint in fenceCollisionDef.LinkPoints)
                        {
                            if (linkPoint.InitialLink)
                            {
                                var position = new Vector3(linkPoint.X, linkPoint.Y, linkPoint.Z);
                                var _rotation = fenceCollision.ElementGameObject.GetRotation();
                                var _position = fenceCollision.ElementGameObject.GetPosition() + _rotation * position - _rotation * (Vector3)(fenceLinkDef.LinkPoint);
                                var difference = new Vector2(collisionData.GameObject.transform.position.x - _position.x, collisionData.GameObject.transform.position.z - _position.z);
                                if (difference.sqrMagnitude < FencePlaceHelper.SqrNearLinkDifference)
                                {
                                    collisionData.GameObject.transform.position = _position;
                                    collisionData.GameObject.transform.rotation = _rotation;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool Calculate(this FencePlaceholderData data)
        {
            var lookupDirection = new Vector3(0.0f, 0.0f, data.DistanceXZ);
            var shift = new Vector3(0.0f, data.ShiftY, 0.0f);
            var characterCenterPoint = (Vector3)(SharedCode.Utils.BuildUtils.BuildParamsDef.CharacterCenterPoint);

            data.CanPlace.Set(CanPlaceData.REASON_OUT_OF_PLACE);
            data.Position = data.InterfacePosition + characterCenterPoint + data.InterfaceRotation * lookupDirection;
            data.Rotation = Quaternion.Euler(0.0f, Mathf.Repeat(data.InterfaceRotation.eulerAngles.y + data.RotationY + AdditionalElementRotation, 360.0f), 0.0f);

            var def = data.BuildRecipeDef.ElementDef.Target as FenceElementDef;
            List<FenceElementData> fenceCollisions = new List<FenceElementData>();
            var colliders = Physics.OverlapBox(data.Position + (Vector3)(def.Center) + shift, PhysicsChecker.CheckExtentsSize(((Vector3)(def.Extents) * boxColliderScale), def.ToString()), data.Rotation, PhysicsLayers.PlaceBuildElementMask);
            if ((colliders != null) && (colliders.Length > 0))
            {
                foreach (var collider in colliders)
                {
                    var fenceData = collider.GetComponentInParent<FenceElementBehaviour>()?.GetData();
                    if (fenceData != null)
                    {
                        fenceCollisions.Add(fenceData);
                    }
                }
            }

            bool collisionFound = false;
            if (fenceCollisions.Count > 0)
            {
                float minMagnitude = 0.0f;
                var position = data.Position; // cache because we change data.Position in cirle below
                var points = data.GetLinkPoints(false);
                foreach (var fenceCollision in fenceCollisions)
                {
                    var collisionPoints = fenceCollision.GetLinkPoints(true);
                    foreach (var collisionPoint in collisionPoints)
                    {
                        foreach (var point in points)
                        {
                            float magnitude = (collisionPoint.Key - point.Key - position).sqrMagnitude;
                            if (!collisionFound || (magnitude < minMagnitude))
                            {
                                minMagnitude = magnitude;
                                collisionFound = true;
                                data.Position = collisionPoint.Key - point.Key;
                            }
                        }
                    }
                }
            }

            RaycastHit hit;
            if (Physics.Raycast(data.Position, Vector3.down, out hit, PhysicsChecker.CheckDistance(terrainCollisionDistance, "FencePlace"), PhysicsLayers.PlaceBuildElementMask))
            {
                data.Position = hit.point;
                data.CanPlace.Clear(CanPlaceData.REASON_OUT_OF_PLACE);
            }
            else if (Physics.Raycast(data.Position + Vector3.up * terrainCollisionDistance, Vector3.down, out hit, PhysicsChecker.CheckDistance(terrainCollisionDistance, "FencePlace2"), PhysicsLayers.PlaceBuildElementMask))
            {
                data.Position = hit.point;
                data.CanPlace.Clear(CanPlaceData.REASON_OUT_OF_PLACE);
            }
            else if (collisionFound)
            {
                if (Physics.Raycast(data.Position, Vector3.down, out hit, PhysicsChecker.CheckDistance(terrainCollisionDistanceFar, "FencePlace3"), PhysicsLayers.PlaceBuildElementMask))
                {
                    data.Position = hit.point;
                    data.CanPlace.Clear(CanPlaceData.REASON_OUT_OF_PLACE);
                }
                else if (Physics.Raycast(data.Position + Vector3.up * terrainCollisionDistanceFar, Vector3.down, out hit, PhysicsChecker.CheckDistance(terrainCollisionDistanceFar, "FencePlace4"), PhysicsLayers.PlaceBuildElementMask))
                {
                    data.Position = hit.point;
                    data.CanPlace.Clear(CanPlaceData.REASON_OUT_OF_PLACE);
                }
            }
            data.Position += shift;
            return true;
        }

        public static bool Calculate(this FenceElementData data, out Vector3 resultPosition, out Quaternion resultRotation)
        {
            resultPosition = (Vector3)data.Position;
            resultRotation = (Quaternion)data.Rotation;
            return true;
        }
    }
}
