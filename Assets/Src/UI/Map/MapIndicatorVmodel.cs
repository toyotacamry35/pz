using System;
using UnityEngine;

namespace Uins
{
    public struct MapIndicatorVmodel
    {
        public MapIndicatorsPositions MapIndicatorsPositions;
        public Guid MarkerGuid;
        public Transform Target;
        public IMapIndicatorSettings MapIndicatorSettings;
    }
}