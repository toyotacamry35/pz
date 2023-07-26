using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOD;

namespace DeepSky.Haze
{
    [ExecuteInEditMode]
    public class DS_HazeZoneSphere : DS_HazeZone
    {
        public int m_PrioritySphere = 0;

        public float m_BlendRangeSphere = 5f;
        public float m_BlendRangeOutSphere = 10f;

        [ColorUsage(true)]
        public Color innerColor = new Color(1, 0, 0, 0.25f);/// Color.yellow;
        [ColorUsage(true)]
        public Color outerColor = new Color(0, 1, 0, 0.25f);


        public float GetBlendWeight(Vector3 position)
        {
            float dist = Vector3.Distance(position, transform.position);
            if (dist <= m_BlendRangeSphere)
                return 1f;
            else
            {
                float blendSize = m_BlendRangeOutSphere;
                float distNormalize = Mathf.Clamp(1f - (dist - m_BlendRangeSphere) / blendSize,0,1f);
                return distNormalize;
            }
        }


        public static bool operator >(DS_HazeZoneSphere c1, DS_HazeZoneSphere c2)
        {
            if (c1.m_PrioritySphere == c2.m_PrioritySphere)
                return c1.m_BlendRangeSphere > c2.m_BlendRangeSphere ? true : false;

            return c1.m_PrioritySphere > c2.m_PrioritySphere ? true : false;
        }


        public static bool operator <(DS_HazeZoneSphere c1, DS_HazeZoneSphere c2)
        {
            if (c1.m_PrioritySphere == c2.m_PrioritySphere)
                return c1.m_BlendRangeSphere < c2.m_BlendRangeSphere ? true : false;

            return c1.m_PrioritySphere < c2.m_PrioritySphere ? true : false;
        }
        public void Register()
        {
            DS_HazeCore core = DS_HazeCore.Instance;

            if (core == null)
            {
                Debug.LogError("DeepSky::DS_HazeLightVolume: Attempting to add a light volume but no HS_HazeCore found in scene! Please make sure there is a DS_HazeCore object.");
            }
            else
            {
                core.AddZoneSphere(this);
            }
        }

        public void Deregister()
        {
            DS_HazeCore core = DS_HazeCore.Instance;

            if (core != null)
            {
                core.RemoveZoneSphere(this);
            }
        }

        

        void OnDisable()
        {
            Deregister();
        }

        void OnDestroy()
        {
            Deregister();
        }

        void OnEnable()
        {
            Register();
        }
    }

    
}
