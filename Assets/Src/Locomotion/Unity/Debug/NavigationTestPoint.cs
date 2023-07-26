using System.Collections.Generic;
using UnityEngine;

namespace Src.Locomotion.Unity
{
    public class NavigationTestPoint : MonoBehaviour
    {
        private static readonly List<NavigationTestPoint> _allPoints = new List<NavigationTestPoint>();

        public static IReadOnlyList<NavigationTestPoint> AllPoints => _allPoints;

        private void Awake()
        {
            _allPoints.Add(this);
            _allPoints.Sort((x,y) => string.CompareOrdinal(x.gameObject.name, y.gameObject.name));
        }

        private void OnDestroy()
        {
            _allPoints.Remove(this);
            _allPoints.Sort((x,y) => string.CompareOrdinal(x.gameObject.name, y.gameObject.name));            
        }
    }
}