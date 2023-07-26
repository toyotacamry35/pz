using UnityEngine;

public class RotatorTempo : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        transform.Rotate(Vector3.up, Speed * Time.deltaTime);
    }
}