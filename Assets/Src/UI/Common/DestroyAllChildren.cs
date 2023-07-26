using UnityEngine;

public class DestroyAllChildren : MonoBehaviour
{
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}