using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl;
using Assets.Src.ResourceSystem;
using ColonyShared.SharedCode.Aspects.Misc;
using NLog;
using ResourcesSystem.Loader;
using UnityEngine;

namespace Src.Animation
{
    public class AnimationTrajectory : ScriptableObject
    {
        public Curve3 Position;
        public CurveQ Rotation;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public static AnimationTrajectory CreateInstance()
        {
            AnimationTrajectory trajectory = CreateInstance<AnimationTrajectory>();
            trajectory.Position = CreateInstance<Curve3>();
            trajectory.Rotation = CreateInstance<CurveQ>();
            return trajectory;
        }

        public TrajectoryDef GenerateTrajectoryDef(float timeStep, float lengthStep)
        {
            var trajectoryDef = new TrajectoryDef();
            var trajTime = Position._maxTime - Position._minTime;
            trajectoryDef.Duration = trajTime;

            float length = Position.Distance(0, trajTime);
            int samplesCount = (int)Math.Ceiling(length / lengthStep);
            float time = 0;
            for (int i = 0; i < samplesCount; ++i)
            {
                var pr = Evaluate(Math.Min(time, trajTime));
                trajectoryDef.Keys.Add(new CurveDefKey()
                {
                    Time = time,
                    Position = pr.Position.ToShared(),
                    Rotation = (SharedCode.Utils.Quaternion)pr.Rotation
                });
                time = Position.Shift(time, lengthStep);
            }

            var samplesCountTime = (int)Math.Ceiling(trajTime / timeStep);
            for (int i = 0; i < samplesCountTime; i++)
            {
                var keyTime = Position._minTime + timeStep * i;
                var pr = Evaluate(keyTime);
                trajectoryDef.Keys.Add(new CurveDefKey()
                {
                    Time = keyTime,
                    Position = pr.Position.ToShared(),
                    Rotation = (SharedCode.Utils.Quaternion)pr.Rotation
                });
            }

            trajectoryDef.Keys.Sort((x1, x2) => Math.Sign(x1.Time - x2.Time));

            const float mergeDeltaTime = 0.001f;
            var index = 0;
            while (index < trajectoryDef.Keys.Count - 1)
            {
                if (trajectoryDef.Keys[index + 1].Time - trajectoryDef.Keys[index].Time <= mergeDeltaTime)
                    trajectoryDef.Keys.RemoveAt(index + 1);
                else
                    index++;
            }

            if (trajectoryDef.Keys.Count > 0)
                trajectoryDef.BoundingSphereRadius = trajectoryDef.Keys.Max(x => x.Position.Length);

            trajectoryDef.SamplesCount = trajectoryDef.Keys.Count;

            return trajectoryDef;
        }

        public PositionRotation Evaluate(float time) => new PositionRotation(Position.Evaluate(time), Rotation.Evaluate(time));

        public void Clear()
        {
            Position.Clear();
            Rotation.Clear();
        }

        public void AddKey(float time, Vector3 position, Quaternion rotation)
        {
            Position.AddKey(time, position);
            Rotation.AddKey(time, rotation);
        }

        public void Recalculate(float timeStep)
        {
            Position.Recalculate(timeStep);
        }

        public bool EqualsByContent(AnimationTrajectory other, float tolerance)
        {
            return ReferenceEquals(this, other) ||
                Position.EqualsByContent(other.Position, tolerance) && Rotation.EqualsByContent(other.Rotation, tolerance);
        }

        public bool Optimize(float tolerance)
        {
            bool rv = false;
            rv |= Position.Optimize(tolerance);
            rv |= Rotation.Optimize(tolerance);
            return rv;
        }
    }
}