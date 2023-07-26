using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TerrainBaker
{
    public class TerrainBakerLink : ScriptableObject
    {
        [HideInInspector]
        public int heightmapSize;
        [HideInInspector]
        public byte[] splitsMaps;
        [HideInInspector]
        public float[] heightMapBorder;
    }
}
