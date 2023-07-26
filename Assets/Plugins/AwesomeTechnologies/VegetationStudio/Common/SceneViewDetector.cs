using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;


namespace AwesomeTechnologies.Utility
{
    [InitializeOnLoad]
    public class SceneViewDetector : MonoBehaviour
    {
        private static EditorWindow _currentEditorWindow;
        private static SceneView _currentSceneView;
        private static Matrix4x4 camTransforms = Matrix4x4.identity;
        public delegate void MultiVegetationCellRefreshDelegate(Camera sceneviewCamera);
        public static MultiVegetationCellRefreshDelegate OnChangedSceneViewCameraDelegate;

        static SceneViewDetector()
        {
            EditorApplication.update += UpdateEditorCallback;
        }

        public static void UpdateEditorCallback()
        {
            if (_currentEditorWindow != EditorWindow.focusedWindow)
            {
                _currentEditorWindow = EditorWindow.focusedWindow;
                var view = _currentEditorWindow as SceneView;
                if (view != null)
                {
                    if (_currentSceneView != view)
                    {
                        _currentSceneView = view;
                        if (OnChangedSceneViewCameraDelegate != null)
                        {
                            OnChangedSceneViewCameraDelegate(_currentSceneView.camera);
                            return;
                        }
                    }
                }
            }
            else
            {
                //_currentEditorWindow == EditorWindow.focusedWindow;

                if (_currentSceneView == null)
                    _currentSceneView = _currentEditorWindow as SceneView;

                if (_currentSceneView != null)
                {
                    if (camTransforms == Matrix4x4.identity)
                    {
                        camTransforms = _currentSceneView.camera.transform.worldToLocalMatrix;
                        if (OnChangedSceneViewCameraDelegate != null)
                        {
                            OnChangedSceneViewCameraDelegate(_currentSceneView.camera);
                            return;
                        }
                    }
                    else
                    if (camTransforms != _currentSceneView.camera.transform.worldToLocalMatrix)
                    {
                        camTransforms = _currentSceneView.camera.transform.worldToLocalMatrix;
                        if (OnChangedSceneViewCameraDelegate != null)
                        {
                            OnChangedSceneViewCameraDelegate(_currentSceneView.camera);
                            return;
                        }
                    }
                }
            }
               
        }

        public static Camera GetCurrentSceneViewCamera()
        {
            if (_currentSceneView != null)
            {
                return _currentSceneView.camera;
            }

            Camera[] sceneviewCameras = SceneView.GetAllSceneCameras();
            return sceneviewCameras.Length > 0 ? sceneviewCameras[0] : null;
        }


        void DisableEditorApi()
        {
            EditorApplication.update -= UpdateEditorCallback;
        }
    }
}
#endif
