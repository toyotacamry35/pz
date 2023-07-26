using UnityEngine;

namespace Src.Camera
{
    public interface ICameraWithLookAt
    {
        Transform LookAt { get; set; }
    }
}
