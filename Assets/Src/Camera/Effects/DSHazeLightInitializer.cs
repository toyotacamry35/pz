using UnityEngine;
using DeepSky.Haze;

// Add this script to camera, which has DS_HazeView component to initiate it
public class DSHazeLightInitializer : MonoBehaviour
{
    void Start ()
    {
        var dsHaze = GetComponent<DS_HazeView>();
        if (!dsHaze)
        {
            Debug.LogError("Can't find necessary component \"DeepSky.Haze.DS_HazeView\" on camera.");
            return;
        }

        // Do work 1of2 - check DSHaze on scene:
        if (!IsDSHazeContorllerOnScene(dsHaze))
            return;

        GameObject sun = GameObject.FindGameObjectWithTag("Sun");
        if (!sun)
        {
            Debug.LogError("Can't find necessary object with tag \"Sun\".");
            return;
        }

        var sunLight = sun.GetComponent<Light>();
        if (!sunLight)
        {
            Debug.LogError("Can't find necessary component of type \"Light\" on Sun gameObj.");
            return;
        }

        // Do work 2of2 - init. DSHaze light:
        dsHaze.DirectLight = sunLight;
    }


    private bool IsDSHazeContorllerOnScene(DS_HazeView dsHazeCamComponent)
    {
        var o = FindObjectOfType<DS_HazeZone>();
        if (!o || !o.GetComponent<DS_HazeZone>().enabled)
        {
            Debug.LogError("Can't find necessary enabled script \"DS_HazeZone\" on any obj. on scene." +
                           "\n \"DS_HazeView\" camera script 'll be off.");
            dsHazeCamComponent.enabled = false;
            return false;
        }

        return true;
    }
}
