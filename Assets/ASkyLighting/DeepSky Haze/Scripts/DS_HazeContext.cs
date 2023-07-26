using UnityEngine;
using System;
using System.Collections.Generic;
using TOD;

namespace DeepSky.Haze
{
    [Serializable, AddComponentMenu("")]
    public class DS_HazeContext
    {
        [SerializeField]
        public HazeItemEclipse m_NightItem;
        [SerializeField]
        public DS_HazeContextItem m_ComplexItem;
        
        public DS_HazeContext()
        {
            m_NightItem = new HazeItemEclipse();
            m_ComplexItem = new DS_HazeContextItem();
        }
        	

        public void CopyFrom(DS_HazeContext other)
        {
            m_NightItem = new HazeItemEclipse();
            m_NightItem.CopyFrom(other.m_NightItem);

            m_ComplexItem = new DS_HazeContextItem();
            m_ComplexItem.CopyFrom(other.m_ComplexItem);
        }

        public DS_HazeContextAsset GetContextAsset()
        {
            DS_HazeContextAsset cxt = ScriptableObject.CreateInstance<DS_HazeContextAsset>();

            cxt.Context.CopyFrom(this);
            return cxt;
        }

    }
}
