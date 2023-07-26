using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.WorldFiller.Editor
{

    public class LayerFillingData : TerrainBaker.TerrainLayerFilling
    {

        public struct RangeFloat
        {
            public float start;
            public float end;

            public RangeFloat(float min, float max)
            {
                start = min;
                end = max;
            }
        };


        [Serializable]
        public class LOD
        {
            public Mesh mesh;
            public Material material;
            public float distanceInPercents;
            public float countInPercents;
            public bool isCastShadows;
        }

        [Serializable]
        public class Node
        {
            public bool disableGenerate = false;
            public int instanceCount = 10000;
            public float maxViewDistance = 100.0f;
            public bool isEnableTransition = false;
            public float transitionInPersents = 10.0f;
            public float scaleStart = 0.5f;
            public float scaleEnd = 1.0f;
            public float weightThreshold = 0.5f;
            public List<LOD> lods = new List<LOD>() { new LOD() { } };            
        }

        public List<Node> nodes = new List<Node>() { new Node() };
        public int seed = 123;
        public bool disableModifySeedFromPosition;

    }
}
