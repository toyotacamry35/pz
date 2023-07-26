using UnityEngine;

namespace Assets.Src.Camera
{
    public interface ICameraWithFollow
    {
        Transform Follow { get; set; }
    }
}