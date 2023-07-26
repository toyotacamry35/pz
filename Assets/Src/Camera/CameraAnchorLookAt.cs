using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.Camera;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;

namespace Src.Camera
{
    public class CameraAnchorLookAt : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(Camera));

        public CameraRef Camera;

        public static Transform Find(Transform root, CameraDef tag)
        {
            try
            {
                return root?.GetComponentsInChildren<CameraAnchorFollow>(true).SingleOrDefault(x => x.Camera.Target == tag)?.transform;
            }
            catch (InvalidOperationException e)
            {
                Logger.IfWarn()?.Message($"CameraLookAtAnchor.Find: {e.Message}: Root: {root} Tag: {tag}").Write();
                return null;
            }
        }
    }
}