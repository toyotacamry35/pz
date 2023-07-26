using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TerrainBaker
{
    public class TerrainMaterialsArray : ScriptableObject
    {
        public const byte noMaterial = 0xff;
        [HideInInspector]
        public int alphaMapSize;
        [HideInInspector]
        public byte[] materialsMap;
    }
}
