using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies;

public class VegetationCamera : MonoBehaviour {

    public static VegetationCamera instance;
    float lastClick = 0f;
    float interval = 0.4f;
    VegetationSystem[] vegetationSystems;
    bool isDebugGUI = false;
    public static bool isDrawGrass = true;

	void Start ()
    {
        instance = this;
        vegetationSystems = FindObjectsOfType<VegetationSystem>();
        for (int i=0; i<vegetationSystems.Length; i++)
            vegetationSystems[i].isCamera = true;

    }
#if PERSISTENT_VEGETATION
    private void Update()
    {
        if (Input.GetKey(KeyCode.F12) && Input.GetKey(KeyCode.LeftControl) && (lastClick + interval) < Time.time)
        {
            lastClick = Time.time;
            isDebugGUI = !isDebugGUI;
        }

    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (isDebugGUI)
        {
            if (GUI.Button(new Rect(Screen.width - 140, 5, 130, 25), (isDrawGrass) ? "Disable Grass" : "Enable Grass"))
                isDrawGrass = !isDrawGrass;

        }
    }
#endif
#endif

}
