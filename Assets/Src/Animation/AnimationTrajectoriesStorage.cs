using System;
using System.Collections.Generic;
using System.Linq;
using Src.Tools;
using UnityEngine;

namespace Src.Animation
{
    public partial class AnimationTrajectoriesStorage : ScriptableObject
    {
        [SerializeField] public AnimationTrajectoriesSettings Settings;
        [SerializeField] public List<Tuple> Trajectories;

        private Dictionary<SerializableGuid, AnimationTrajectory> _trajectories;
        
        public AnimationTrajectory GetTrajectory(SerializableGuid guid)
        {
            if (_trajectories == null)
                _trajectories = Trajectories.ToDictionary(x => x.Guid, x => x.Trajectory);
            return _trajectories[guid];
        }
        
        public bool TryGetTrajectory(SerializableGuid guid, out AnimationTrajectory trajectory)
        {
            if (_trajectories == null)
                _trajectories = Trajectories.ToDictionary(x => x.Guid, x => x.Trajectory);
            return _trajectories.TryGetValue(guid, out trajectory);
        }

        [Serializable]
        public struct Tuple
        {
            public string Name;
            public SerializableGuid Guid;
            public AnimationTrajectory Trajectory;
        }
    }
}