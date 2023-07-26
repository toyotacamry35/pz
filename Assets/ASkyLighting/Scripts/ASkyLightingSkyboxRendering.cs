using UnityEngine;
using UnityEngine.Rendering;
using TOD;

[RequireComponent(typeof(Camera))]
public class ASkyLightingSkyboxRendering : MonoBehaviour
{
	public Material material;
	private Camera currentCam;
	private RenderingPath currentUsedRenderingPath;

	public void Apply()
	{
		currentCam = GetComponent<Camera> ();
		currentUsedRenderingPath = currentCam.actualRenderingPath;
		RefreshCameraCommand ();
	}

	void Update ()
	{
		if (currentCam != null) {
			if(currentUsedRenderingPath != currentCam.actualRenderingPath)
				RefreshCameraCommand ();
		}
	}

	public void RefreshCameraCommand ()
	{
		CommandBuffer[] cbs;
		cbs = currentCam.GetCommandBuffers (CameraEvent.BeforeGBuffer);

		for (int i = 0; i < cbs.Length; i++) {

			if (cbs [i].name == "ASkyRendering")
				currentCam.RemoveCommandBuffer (CameraEvent.BeforeGBuffer, cbs [i]);
		}

		cbs = currentCam.GetCommandBuffers (CameraEvent.BeforeForwardOpaque);
		for (int i = 0; i < cbs.Length; i++) {

			if (cbs [i].name == "ASkyRendering")
				currentCam.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cbs [i]);
		}
		currentUsedRenderingPath = currentCam.actualRenderingPath;
		CommandBuffer cb = new CommandBuffer();
		cb.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget,material);
		cb.name = "ASkyRendering";

		if (currentCam.actualRenderingPath == RenderingPath.DeferredShading) 
			currentCam.AddCommandBuffer (CameraEvent.BeforeGBuffer, cb);
		else
			currentCam.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
	}

	void OnPreRender ()
	{
        if (!ASkyLighting._instance)
            return;
        if (!ASkyLighting._instance.GetCamera())
            return;

        if (ASkyLighting._instance.skyboxRenderCamera)
        {
            RenderCamera(ASkyLighting._instance.skyboxRenderCamera);
            material.SetTexture("_Clouds", ASkyLighting._instance.skyboxRenderCamera.targetTexture);
        }
	} 

	void RenderCamera(Camera targetCam)
	{
        targetCam.fieldOfView = ASkyLighting._instance.PlayerCamera.fieldOfView;
        targetCam.aspect = ASkyLighting._instance.PlayerCamera.aspect;
        targetCam.transform.position = ASkyLighting._instance.PlayerCamera.transform.position;
        targetCam.transform.rotation = ASkyLighting._instance.PlayerCamera.transform.rotation;
        targetCam.worldToCameraMatrix = ASkyLighting._instance.PlayerCamera.worldToCameraMatrix;
	    targetCam.Render ();
		
	}
}