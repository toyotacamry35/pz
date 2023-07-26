namespace Src.Locomotion
{
    public struct ObstacleInfo
    {
        public bool Detected; 
        public float Distance; // Расстояние до столкновения
        public LocomotionVector HitPoint; // Точка касания
        public LocomotionVector HitNormal; // Нормаль поверхности в точке касания
        public LocomotionVector HitTangent; // Касательная к поверхности в точке касания вдоль направления  
        public bool IsStair; // Это "ступенька"
        public LocomotionVector StairPoint; // Крайняя точка на "ступени"
        public float StairHeight; // (Приблизительная) высота "ступени" от земли
        public LocomotionVector Pivot; // Пивот (нижняя точка) коллайдера персонажа
    }
}