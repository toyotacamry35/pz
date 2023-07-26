using UnityEngine;

namespace AwesomeTechnologies.Utility
{
    public class RandomNumberGenerator
    {
        private readonly int _randomSeed;
        private readonly float[] _randomFloats = new float[10000];

        public RandomNumberGenerator(int randomSeed)
        {
            _randomSeed = randomSeed;
            SetupRandomNumbers();
        }

        private void SetupRandomNumbers()
        {
            Random.InitState(_randomSeed);
            for (int i = 0; i <= 9999; i++)
                _randomFloats[i] = Random.Range(0f, 1f);
        }

        public float RandomRange(int seedCounter, float min, float max)
        {
            int index = seedCounter;

            while (index > 9999)
                index = index - 10000;

            return Mathf.Lerp(min, max, _randomFloats[index]);
        }
    }
}