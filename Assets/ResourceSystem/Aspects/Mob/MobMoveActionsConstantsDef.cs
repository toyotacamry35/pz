using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.NetworkedMovement
{
    public class MobMoveActionsConstantsDef : BaseResource
    {
        /// <summary>
        /// Максимальный радиус поиска навмеша рядом с точкой назначения при поиске пути
        /// </summary>
        public float DestinationPointAdjustmentRadius { get; private set; } = 3; // м

        /// <summary>
        /// Минимальное и максимальное время задержки перед телепортированием при SimulationLevel.Faraway  
        /// </summary>
        public float FollowPathTeleportDelayMin { get; private set; } = 10; // сек
        public float FollowPathTeleportDelayMax { get; private set; } = 50; // сек

        /// <summary>
        /// Параметры определяющие частоту пересчёта пути в зависимости от расстояния до цели 
        /// </summary>
        public RepathParameters FollowPathRepathNear { get; private set; } = new RepathParameters {
            Distance = 2,
            TargetShift = 0.2f,
            SelfDeviation = 0.5f,
            Delay = 0.1f
        };

        public RepathParameters FollowPathRepathFar { get; private set; } = new RepathParameters {
            Distance = 20,
            TargetShift = 3f,
            SelfDeviation = 3f,
            Delay = 1f
        };
        
        /// <summary>
        /// Дополнительный случайный разброс задержки пересчёта пути
        /// </summary>
        public float FollowPathRepathDelayRandomisation { get; private set; } = 0.1f; // сек

        /// <summary>
        /// Время, через которое хождение по пути сфейлится в случае невозможности построить путь до цели.  
        /// </summary>
        public float FollowPathTargetNotReachableTimeout { get; private set; } = 3.0f; // сек
        
        public class RepathParameters : BaseResource
        {
            public float Distance; // Расстояние до цели (м)
            public float TargetShift; // Порог смещения цели при котором путь должен быть перестроен (м)
            public float SelfDeviation; // Порог отклонения моба от текущего пути при котором путь должен быть перестроен (м)
            public float Delay; // Задержка перед пересчётом пути (сек)
        }
    }
}