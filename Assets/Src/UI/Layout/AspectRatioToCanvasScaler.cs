using UnityEngine;
using UnityEngine.UI;

public class AspectRatioToCanvasScaler : MonoBehaviour
{
    public CanvasScaler CanvasScaler;
    public float MinScalerMatchAspectRatio = 1.5f;
    public float MaxScalerMatchAspectRatio = 1.77f;

    private LinearRelation _linearRelation;
    private float _lastAspectRatio;

    private void Awake()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            enabled = false;
            return;
        }
        CanvasScaler.AssertIfNull(nameof(CanvasScaler));
    }

    private void OnEnable()
    {
        _linearRelation = new LinearRelation(MinScalerMatchAspectRatio, 0, MaxScalerMatchAspectRatio, 1);
    }

    void Update() //TODOM Переписать как потомка CanvasScaler
    {
        var aspectRatio = (float) Screen.width / Screen.height;
        if (Mathf.Approximately(aspectRatio, _lastAspectRatio))
            return;

        _lastAspectRatio = aspectRatio;
        CanvasScaler.matchWidthOrHeight = _linearRelation.GetClampedY(_lastAspectRatio);
    }
}