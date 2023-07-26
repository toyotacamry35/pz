using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOD;

public class ASkyLightingCamera : MonoBehaviour {

    ASkyLighting timeOfDayManager;
    void Start()
    {
        timeOfDayManager = FindObjectOfType<ASkyLighting>();
        if (timeOfDayManager!=null && Application.isPlaying)
        {
            ASkyLighting.isActiveOnClient = true;
            timeOfDayManager.Create();
        }

    }

    private void OnEnable()
    {
        ASkyLighting.isActiveOnClient = true;
    }

    private void OnDisable()
    {
        ASkyLighting.isActiveOnClient = false;
    }

    private void OnDestroy()
    {
        ASkyLighting.isActiveOnClient = false;
    }
}
