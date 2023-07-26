using UnityEngine;

public class ZoneID : MonoBehaviour
{
    public enum IDType
    {
        None,
        Water
    }

    public IDType Type;
    public string IDName;
    public float Radius;

    private const float Height = 5f;

    private void OnDrawGizmos()
    {
        var start = transform.position - new Vector3(0, Height / 2f);
        var end = transform.position + new Vector3(0, Height / 2f);
        DebugExtension.DebugCylinder(start, end, Color.green, Radius);
    }
}
