using Assets.Instancenator;
using Core.Cheats;
using UnityEngine;

namespace Assets.Src.Instancenator
{
    public static class InstanceCompositionDistanceManager
    {
        private const float _transitionTime = 3f;
        private static float _remainingTime;
        public static float CurrentDistanceBias { get; private set; } = 1f;
        private static float TargetDistanceBias = 1f;
        private static float TransitionStartDistanceBias;


        public delegate void GrassDistanceChangeHandler(float newDistance);

        public static event GrassDistanceChangeHandler GrassDistanceChangedEvent;

        internal static void UpdateDistances(float timePassed)
        {
            if (CurrentDistanceBias != TargetDistanceBias)
            {
                _remainingTime = Mathf.Clamp(_remainingTime - timePassed, 0, _transitionTime);
                var transition01 = Mathf.Pow(Mathf.Cos(Mathf.PI / 2f * _remainingTime / _transitionTime), 1.5f);
                var slope = TargetDistanceBias - TransitionStartDistanceBias;
                var shift = TransitionStartDistanceBias;
                var transition = transition01 * slope + shift;
                SetDistance(transition);
            }
        }

        /// <summary>
        /// Set lod bias of instanced vegetation.
        /// Default vegetation distance with value = 1 is 100m.
        /// </summary>
        /// <param name="value">default value is 1</param>
        /// <param name="immediately">do not apply smooth transition</param>
        [Cheat]
        public static void SetVegetationLodBias(float value, bool immediately)
        {
            TargetDistanceBias = value;
            TransitionStartDistanceBias = CurrentDistanceBias;
            if (immediately)
                SetDistance(value);
            else
                _remainingTime = _transitionTime;
        }

        private static void SetDistance(float value)
        {
            InstanceComposition.InitShaderIDs();
            CurrentDistanceBias = value;
            if(GrassDistanceChangedEvent != null)
                GrassDistanceChangedEvent(CurrentDistanceBias);
            Shader.SetGlobalFloat(InstanceComposition.shaderGlobalLodBiasReciprocal, 1 / value);
        }
    }
}