namespace Assets.Src.Effects.FX
{
    public struct GameObjectPoolSettings
    {
        public int priority;
        public float maxTimeInSeconds;
        public bool isItMandatory;

        public override string ToString()
        {
            return $"{nameof(priority)}: {priority}, time: {maxTimeInSeconds}, mandatory: {isItMandatory}";
        }
    }
}