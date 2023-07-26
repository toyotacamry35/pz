using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDirectionSetter_FX : MonoBehaviour {

    private ParticleSystem emitter;
    private float rotation;
    private GameObject mainCamera; 

    private void Awake()
    {
        emitter = gameObject.GetComponent<ParticleSystem>();
        rotation = gameObject.transform.rotation.y;
        var newRotation = emitter.main.startRotation;
        newRotation = rotation;

        Debug.Log(emitter.main.startRotation);

        //mainCamera = GameObject.Find("MainCamera");
        //if (Vector3.Dot(mainCamera.transform.right, gameObject.transform.forward) > 0)
        //{
        //    Shader.SetGlobalInt("_PanDirection" , 1);
        //}
        //else
        //{
        //    Shader.SetGlobalInt("_PanDirection", -1);
        //}        
    }
}
