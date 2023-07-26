using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    //DEBUG
    public class MapCustomPointsSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private UserMarkers _userMarkers;

        private void Awake()
        {
            _userMarkers.AssertIfNull(nameof(_userMarkers));
        }

        public void SetCustomMapAnchorPoint1()
        {
            _userMarkers.SetCustomMapAnchorPoint(1);
        }

        public void SetCustomMapAnchorPoint2()
        {
            _userMarkers.SetCustomMapAnchorPoint(2);
        }

        public void SetCustomLevelPoint1()
        {
            _userMarkers.SetCustomLevelPoint(1);
        }

        public void SetCustomLevelPoint2()
        {
            _userMarkers.SetCustomLevelPoint(2);
        }

        public void SetCorners()
        {
            _userMarkers.SetCorners(false);
        }

        public void ResetCorners()
        {
            _userMarkers.SetCorners(true);
        }
    }
}