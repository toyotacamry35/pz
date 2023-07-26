#if UNITY_EDITOR
using System;
using System.Linq;
using Src.Tools;
using UnityEditor;

namespace Src.Animation
{
    public partial class AnimationTrajectoriesStorage
    {
        public void AddTrajectoryAsset(AnimationTrajectory trj)
        {
            if (trj == null) throw new ArgumentException(nameof(trj));
            AssetDatabase.AddObjectToAsset(trj, this);
            AssetDatabase.AddObjectToAsset(trj.Position, this);
            AssetDatabase.AddObjectToAsset(trj.Rotation, this);
        }

         public void RemoveTrajectoryAsset(AnimationTrajectory trj)
         { 
             var pos = trj.Position;
             var rot = trj.Rotation;
             AnimationTrajectory.DestroyImmediate(pos, true);
             AnimationTrajectory.DestroyImmediate(rot, true);
             AnimationTrajectory.DestroyImmediate(trj, true);
         }

        public bool UpdateRegistry((SerializableGuid guid, string name, AnimationTrajectory trjectory)[] list)
        {
            bool rv = false;
            foreach (var tuple in list)
            {
                var idx = Trajectories.FindIndex(x => x.Guid == tuple.guid);
                if (idx == -1)
                {
                    Trajectories.Add(new Tuple{ Guid = tuple.guid, Name = tuple.name, Trajectory = tuple.trjectory});
                    rv = true;
                }
                else if (Trajectories[idx].Trajectory != tuple.trjectory || Trajectories[idx].Name != tuple.name)
                {
                    Trajectories[idx] = new Tuple{ Guid = tuple.guid, Name = tuple.name, Trajectory = tuple.trjectory};
                    rv = true;
                }
            }
            for (int i = Trajectories.Count - 1; i >= 0; --i)
            {
                var tuple = Trajectories[i];
                if (list.All(x => x.guid != tuple.Guid))
                {
                    Trajectories.RemoveAt(i);
                    rv = true;
                }
            }
            return rv;
        }
    }
}  
#endif