namespace Assets.Src.Effects.FX
{
    public static class GameObjectPoolSettingsLevels
    {
        public static GameObjectPoolSettings NonMandatory_0_1_1000 { get; } = new GameObjectPoolSettings {isItMandatory = false, maxTimeInSeconds = 0.1f, priority = 1000};

        public static GameObjectPoolSettings NonMandatory_0_1_200 { get; } = new GameObjectPoolSettings {isItMandatory = false, maxTimeInSeconds = 0.1f, priority = 200};

        public static GameObjectPoolSettings NonMandatory_0_1_100 { get; } = new GameObjectPoolSettings {isItMandatory = false, maxTimeInSeconds = 0.1f, priority = 100};
    }
}