using UnityEngine;
using System;

namespace DeepSky.Haze
{
    /// <summary>
    /// ScriptableObject wrapper to allow DS_HazeContext to be saved as an asset
    /// using Unity's standard asset pipeline.
    /// </summary>
    [Serializable, AddComponentMenu("")]
    public class DS_HazeContextAsset : ScriptableObject
    {
        [SerializeField]
        private DS_HazeContext m_Context = new DS_HazeContext();

        public DS_HazeContext Context
        {
            get { return m_Context; }
        }
    }
}