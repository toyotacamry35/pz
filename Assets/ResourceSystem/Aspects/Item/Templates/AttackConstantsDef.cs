using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Entities;

namespace SharedCode.Aspects.Item.Templates
{
    public class AttackConstantsDef : BaseResource
    {
        public float DefaultDestructionPower { get; set; } = 1000;
        public float DefaultDestructionPowerWeapons { get; set; } = 1000;
        /// <summary>
        /// Максимальное кол-во "атак" которые персонаж может осуществлять одновременно
        /// </summary>
        public int SimultaneousAttacksLimit { get; set; } = 5;
        
        /// <summary>
        /// Максимальное кол-во целей затрагиваемых одной "атакой"
        /// </summary>
        public int AttackTargetsLimit { get; set; } = 10; 

        /// <summary>
        /// Максимальный радиус "атаки"
        /// </summary>
        public int AttackDistanceLimit { get; set; } = 20;
        
        /// <summary>
        /// Временной допуск при проверке попадания цели во временное окно атаки (ms)
        /// </summary>
        public long AttackTimeTolerance { get; set; } = 1000;         

//        /// <summary>
//        /// Максимальная длительность атаки (ms)
//        /// </summary>
//        public long AttackDurationLimit { get; set; } = 10000;

        /// <summary>
        /// Время после детекта первой цели, в течении которого ждём ещё цели чтобы отослать их все скопом (ms).  
        /// </summary>
        public long TargetsBatchTimeout { get; set; } = 100;
        
        /// <summary>
        /// Временное окно при проверке позиции  (ms).  
        /// </summary>
        public long ValidationTimeInterval { get; set; } = 100;         

        /// <summary>
        /// Количество проверок позиции в интервале ValidationTimeInterval.
        /// Если 0 то проверка не осуществляется и попадание засчитывается всегда.
        /// </summary>
        public int ValidationTimeIterations { get; set; } = 3;

        public float HitDetectionTimeStep { get; set; } = 0.001f;
        
        public float HitDetectionColliderFactor { get; set; } = 0.9f;

        public long AttackColliderLookAhead { get; set; } = 0;
        
        public ResourceRef<CapsuleDef> DefaultBounds = new CapsuleDef{ Radius = 0.3f, Height = 0.6f };
        
        public ResourceRef<CapsuleDef> DefaultBuildingBounds = new CapsuleDef{ Radius = 1f, Height = 2f };
    }
}