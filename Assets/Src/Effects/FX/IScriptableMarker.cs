using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public interface IScriptableMarker
    {
        BaseResource Marker(Vector3 position);
    }
}
