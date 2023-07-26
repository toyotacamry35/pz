using System;

namespace Src.Locomotion
{
    public interface ILocomotionEngine : IDisposable
    {
        /// <summary>
        /// Выполнение 
        /// </summary>
        void Execute(float deltaTime);
    }
}