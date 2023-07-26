using Assets.Src.Aspects.Building;
using Assets.Src.Shared;
using SharedCode.Aspects.Item.Templates;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Quaternion = SharedCode.Utils.Quaternion;
using Vector3 = SharedCode.Utils.Vector3;
using Assets.Src.Tools;

namespace Assets.Src.WorldSpace
{
    public class UnityWorldSpace
    {
        Dictionary<GameObject, Tuple<GameObject, BuildingEditor>> _cachedObjects = new Dictionary<GameObject, Tuple<GameObject, BuildingEditor>>();

        public const int BoxSpawnTryCount = 1;
        public const float PlayerHeightTolerance = 2f; //возможность спавнить сундук чуть ниже ног игрока, на эту величину

        private static void Debugsphere(UnityEngine.Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<Collider>().enabled = false;
            go.transform.position = pos;
            go.transform.localScale = UnityEngine.Vector3.one * 0.1f;
        }

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<Vector3> GetDropPosition(Vector3 playerPosition, Quaternion playerRotation)
        {
            if (Constants.WorldConstants == null)
                return Vector3.zero;

            //Logger.IfError()?.Message(playerPosition + "; " + playerRotation + "; " + playerRotation.eulerAngles).Write();

            return await UnityQueueHelper.RunInUnityThread(() =>
            {
                var raycastMask = PhysicsLayers.BoxSpawnMask;
                var unacceptedSurfaceMask = PhysicsLayers.InteractiveAndDestrMask;
                UnityEngine.Vector3 playerPos = ToVector3(playerPosition);
                UnityEngine.Quaternion playerRot = ToQuaternion(playerRotation);
                var playerHeadPos = playerPos + UnityEngine.Vector3.up * Constants.WorldConstants.PlayerHeight;
                var rotationPart = 360f / BoxSpawnTryCount;

                //Debugsphere(playerHeadPos);
                for (int i = 0; i < BoxSpawnTryCount; i++)
                {
                    var currentRot = i == 0
                        ? playerRot
                        : playerRot * UnityEngine.Quaternion.Euler(0, rotationPart * i, 0);

                    var diff = Mathf.Clamp(rotationPart * (UnityEngine.Random.value - 0.5f), -45, 45);
                    currentRot *= UnityEngine.Quaternion.Euler(0, diff, 0);

                    //Logger.IfError()?.Message($"currentRot = {currentRot}, i = {i}").Write();

                    RaycastHit hit;
                    var direction = currentRot * UnityEngine.Vector3.forward;
                    
                    if (Physics.Raycast(new Ray(playerHeadPos, direction),
                        PhysicsChecker.CheckDistance(Constants.WorldConstants.BoxSpawnDistance),
                        raycastMask,
                        QueryTriggerInteraction.Ignore))
                        continue; //место перед игроком д.б. свободно

                    var groundRaycastOriginPos = playerHeadPos + Constants.WorldConstants.BoxSpawnDistance * direction.normalized;
                    //Debugsphere(groundRaycastOriginPos);

                    if (Physics.Raycast(
                        new Ray(groundRaycastOriginPos, UnityEngine.Vector3.down),
                        out hit,
                        PhysicsChecker.CheckDistance(Constants.WorldConstants.EnableBelowPlayerPositionBoxSpawning
                            ? 100f
                            : Constants.WorldConstants.PlayerHeight + PlayerHeightTolerance, "Drop2"),
                        raycastMask, QueryTriggerInteraction.Ignore))
                    {
                        if ((1 << hit.collider.gameObject.layer & unacceptedSurfaceMask) > 0)
                            continue;

                        //Debugsphere(hit.point);
                        return ToVector3(hit.point);
                    }
                }

                return playerPosition;
            });
        }

        public async Task<KeyValuePair<bool, Vector3>> GetDropPositionEx(Vector3 startPosition, float distance)
        {
            return await UnityQueueHelper.RunInUnityThread(() =>
            {
                var unacceptedSurfaceMask = PhysicsLayers.InteractiveAndDestrMask;
                UnityEngine.Vector3 unityStartPosition = ToVector3(startPosition);
                var ray = new Ray(unityStartPosition, UnityEngine.Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance, PhysicsLayers.CheckIsGroundedMask))
                {
                    return new KeyValuePair<bool, Vector3>(true, ToVector3(hit.point));
                }
                else
                {
                    return new KeyValuePair<bool, Vector3>(false, startPosition);
                }
            });
        }

        public async Task<Vector3> GetDropCorpsePosition(Vector3 playerPosition)
        {
            if (Constants.WorldConstants == null)
                return Vector3.zero;

            return await UnityQueueHelper.RunInUnityThread(() =>
            {
                var raycastMask = PhysicsLayers.CheckIsGroundedMask;
                UnityEngine.Vector3 playerPos = ToVector3(playerPosition);
                var playerHeadPos = playerPos + UnityEngine.Vector3.up * Constants.WorldConstants.PlayerHeight;

                RaycastHit hit;
                if (Physics.Raycast(
                    new Ray(playerHeadPos, UnityEngine.Vector3.down),
                    out hit,
                    PhysicsChecker.CheckDistance(100f, "DropCorpse"),
                    raycastMask, QueryTriggerInteraction.Ignore))
                {
                    return ToVector3(hit.point);
                }

                return playerPosition;
            });
        }

        public static UnityEngine.Vector3 ToVector3(Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.x, vector.y, vector.z);
        }

        public static Vector3 ToVector3(UnityEngine.Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static SharedCode.Utils.Vector2 ToVector2(UnityEngine.Vector2 vector)
        {
            return new SharedCode.Utils.Vector2(vector.x, vector.y);
        }

        public static UnityEngine.Vector2 ToVector2(SharedCode.Utils.Vector2 vector)
        {
            return new UnityEngine.Vector2(vector.x, vector.y);
        }

        public static UnityEngine.Quaternion ToQuaternion(Quaternion q)
        {
            return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static Quaternion ToQuaternion(UnityEngine.Quaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }
    }
}