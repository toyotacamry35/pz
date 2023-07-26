using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPostProcessingScript_FX : MonoBehaviour {

    public Material effect;

    private Camera cam;
    
    [Range(0.0f, 1.0f)]
    public float weight;
            
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        //write depth texture from main camera
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
        
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // used by other scripts
        effect.SetFloat("_overallIntensity", weight);
        //draw
        Graphics.Blit(source, destination, effect);
    }
}