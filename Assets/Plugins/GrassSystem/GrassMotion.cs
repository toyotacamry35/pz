using UnityEngine;
using System.Collections;
using System;


public enum MotionTextureSize
{
    _32 = 32,
    _64 = 64,
    _128 = 128,
    _256 = 256,
    _512 = 512,
    _1024 = 1024,
    _2048 = 2048,
}

[ExecuteInEditMode]
public class GrassMotion : MonoBehaviour
{
    [SerializeField]
    private GrassMotionRenderer _grassMotionRenderer;
    private Transform _mainCamera;
    private int _lastRenderedFrame = -1;

#if UNITY_EDITOR
    public bool isDebug = false;
    public Material gridMaterial;
    public Transform gridTransform;
#endif
    public GrassMotionZone grassMotionZoneSource;
    public MotionTextureSize motionTextureSize;
    public float textureCenterDistance = 20;
    
	void OnEnable()
	{
		_grassMotionRenderer = new GrassMotionRenderer();
        _grassMotionRenderer.OnEnable();
        _grassMotionRenderer.texSize = (int)motionTextureSize;
        Shader.EnableKeyword("GRASS_MOTION");
    }

	void OnDisable()
	{
		_grassMotionRenderer.OnDisable();
        Shader.DisableKeyword("GRASS_MOTION");
    }

	void OnRenderObject()
	{
        _mainCamera = Camera.current?.transform;
        if (!_mainCamera)
            return;

        
        _grassMotionRenderer.OnRenderObject();
#if UNITY_EDITOR
        gridMaterial.mainTexture = Shader.GetGlobalTexture("_GrassMotionTexture");
#endif
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            Shader.EnableKeyword("GRASS_MOTION");
        }
        else
        {
            Shader.DisableKeyword("GRASS_MOTION");
        }

        if (!_mainCamera)
            return;

        Vector3 cameraDir = new Vector3(_mainCamera.forward.x, 0, _mainCamera.forward.z);
        Shader.SetGlobalVector("_WorldSpaceCameraDir", new Vector4(cameraDir.x, cameraDir.y, cameraDir.z, textureCenterDistance));
        //Shader.EnableKeyword((Application.isPlaying) ? "GRASS_MOTION_ON" : "GRASS_MOTION_OFF");
#if UNITY_EDITOR
        if (isDebug)
        {
            if (!gridTransform.gameObject.activeSelf)
                gridTransform.gameObject.SetActive(true);
            gridTransform.position = _mainCamera.position + cameraDir * textureCenterDistance;
        }
        else
            if (gridTransform.gameObject.activeSelf)
            gridTransform.gameObject.SetActive(false);

        if (Input.GetMouseButton(0))
        {
            if (Time.frameCount == _lastRenderedFrame)
                return;
            
            _lastRenderedFrame = Time.frameCount;

            RaycastHit hit;
            Ray ray = _mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                GrassMotionZone gmz = Instantiate<GrassMotionZone>(grassMotionZoneSource);
                gmz.transform.position = hit.point;
                gmz.transform.parent = transform;

            }
        }
#endif
    }


    private void GetRotatedUVBasis(float angle, out Vector3 right, out Vector3 forward)
	{
		var rotator = Quaternion.AngleAxis(angle, Vector3.up);

		right = rotator * Vector3.right;
		forward = rotator * Vector3.forward;
	}

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (isDebug)
        {
            RenderTexture texture = _grassMotionRenderer._grassMotionRenderTexture;
            Graphics.DrawTexture(new Rect((float)Screen.width - texture.width - 10f, (float)Screen.height - texture.height - 10.0f, texture.width, texture.height), Shader.GetGlobalTexture("_GrassMotionTexture"));
        }
    }
#endif

}