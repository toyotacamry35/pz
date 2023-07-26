using UnityEngine;
using TOD;

public class SceneTimePresetSelector : MonoBehaviour
{
    public static SceneTimePresetSelector instance;
    public float time;
    public bool isServerTime;
    public ASkyLightingContextAsset todPreset;
    public DeepSky.Haze.DS_HazeContextAsset hazePreset;

    void Start()
    {
        instance = this;

        if (ASkyLighting._instance == null)
            ASkyLighting.OnTODManagerArrived += SetTime;
       else
            SetTime();
    }

    public void SetTime()
    {
        ASkyLighting.OnTODManagerArrived -= SetTime;

        ASkyLighting._instance.useServerTime = isServerTime;
        ASkyLighting._instance.timeline = time;
        /*
        if (todPreset != null)
            ASkyLighting._instance.LoadFromContextPreset(todPreset);
        else
            ASkyLighting._instance.LoadDefaultPreset();

            if (hazePreset != null)
                ASkyLighting._instance.hazeCore.mainPreset.LoadFromContextPreset(hazePreset);
            else
                ASkyLighting._instance.hazeCore.mainPreset.LoadPresetDefault();
        */
    }
}
