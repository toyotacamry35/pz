using UnityEngine;

namespace AwesomeTechnologies.Utility
{
    [ExecuteInEditMode]
    public class MakeVisible : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            GameObject[] obj = (GameObject[])FindObjectsOfType(typeof(GameObject));
            foreach (GameObject o in obj)
                o.hideFlags = HideFlags.None;
        }
    }
}


