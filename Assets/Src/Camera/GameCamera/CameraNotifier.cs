using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;

namespace Assets.Src.Camera
{
    public abstract class CameraNotifier<T> : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static event Action<UnityEngine.Camera> OnCameraCreated;

        public static event Action OnCameraDestroyed;

        public static event Action<UnityEngine.Camera> OnCameraChanged;

        public static UnityEngine.Camera Camera => Cameras.Count > 0 ? Cameras[Cameras.Count - 1] : null;
        
        private void OnEnable() 
        {
            _camera = GetComponent<UnityEngine.Camera>();
            if(!_camera)
            {
                Logger.IfError()?.Message($"Need camera on same obj: {transform.name}").Write();
                return;
            }
            var topCamera = Camera;
            Cameras.Add(_camera);
            if (Cameras.Count == 1)
                OnCameraCreated?.Invoke(Camera);
            if (topCamera != Camera)
                OnCameraChanged?.Invoke(Camera);
        }

        private void OnDisable()
        {
            var topCamera = Camera;
            if (_camera && Cameras.Remove(_camera))
            {
                if (Cameras.Count == 0)
                    OnCameraDestroyed?.Invoke();
                if (topCamera != Camera)
                    OnCameraChanged?.Invoke(Camera);
            }
        }

        protected static void InvokeCameraCreated()
        {
            OnCameraCreated?.Invoke(Camera);
        }

        private UnityEngine.Camera _camera;
        private static readonly List<UnityEngine.Camera> Cameras = new List<UnityEngine.Camera>();
    }
}