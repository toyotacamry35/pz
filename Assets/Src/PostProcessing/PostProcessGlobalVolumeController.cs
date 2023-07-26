using System.Collections.Generic;
using Assets.ASkyLighting.SSShadows;
using Trive.Rendering;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PostProcessGlobalVolumeController : MonoBehaviour
{
    public static List<PostProcessGlobalVolumeController> additionalList = new List<PostProcessGlobalVolumeController>();
    private static PostProcessGlobalVolumeController _instance;
    public static PostProcessGlobalVolumeController Instance => _instance;

    private static bool _ssrEnabled = true;

    [SerializeField]
    private PostProcessProfile mainProfile;

    [SerializeField]
    private PostProcessProfile uiProfile;

    private PostProcessVolume _volume;

    [ContextMenu("Recalculate")]
    public void Recalculate()
    {
        var rects = new Rect[1];
        rects[0] = new Rect(0f, 0f, 1f, 1f);
        Recalculate(rects);
    }

    public void Recalculate(Rect[] rects)
    {
        DepthOfFieldRenderer.RecalculateAlphaTexture(rects);
    }

    private void Awake()
    {
        _instance = this;

        additionalList.Add(this);

        // На случай, если `QualitySimpleSetter.SetQuality` отработал раньше, чем случился этот Awake - тогда не позвали `SetSSROnAll`
        SetSSROnAll(SSShadowsControl.SSShadowsEnabled);
    }

    private void Start()
    {
        _volume = GetComponent<PostProcessVolume>();
        Recalculate();
        SetSSR(_ssrEnabled);
    }

    public void SetSSROnAll(bool state)
    {
        _ssrEnabled = state;
        foreach (var controller in additionalList)
        {
            controller.SetSSR(state);
        }
    }

    public void SetSSR(bool state)
    {
        if (mainProfile.HasSettings(typeof(StochasticReflections)))
        {
            var settings = mainProfile.GetSetting<StochasticReflections>();
            settings.enabled.Override(state);
        }

        if (uiProfile.HasSettings(typeof(StochasticReflections)))
        {
            var settings = uiProfile.GetSetting<StochasticReflections>();
            settings.enabled.Override(state);
        }
    }

    public void ActivateOnAll(bool state)
    {
        _volume.profile = state ? uiProfile : mainProfile;
    }

    public void ActivateDof(bool state)
    {
        foreach (var controller in additionalList)
        {
            try
            {
                controller.ActivateOnAll(state);
            }
            catch
            {
                RemoveFromList();
            }
        }
    }

    private void OnDestroy()
    {
        RemoveFromList();
    }

    private void RemoveFromList()
    {
        if (additionalList.Contains(this))
            additionalList.Remove(this);
    }
}