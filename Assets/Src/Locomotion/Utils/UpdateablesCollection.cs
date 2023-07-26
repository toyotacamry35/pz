using System;
using System.Collections;
using System.Collections.Generic;

namespace Src.Locomotion
{
    public class UpdateablesCollection : ILocomotionUpdateable, IDisposable
    {
        private readonly ILocomotionUpdateable[] _updateables;

        public UpdateablesCollection(params ILocomotionUpdateable[] updateables)
        {
            _updateables = updateables;
        }
        
        public void Update(float deltaTime)
        {
            foreach (var updateable in _updateables)
            {
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample($"Loco Commit: updateables.Update [[{updateable.GetType().Name}]]");
                updateable.Update(deltaTime);
                LocomotionProfiler.EndSample();
            }
        }
        
        public void Dispose()
        {
            foreach (var updateable in _updateables)
                (updateable as IDisposable)?.Dispose();
        }
    }
}