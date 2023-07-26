using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Misc;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    public class WeaponDef : BaseResource
    {
        //public ItemTypeResource WeaponType { get; set; }
        public ContainerUsageType ContainerUsage {get; set;}

        public float ThrowSpeed { get; set; }
        public bool UseGravity { get; set; }
        public bool AddTorque { get; set; }

        public float DamageRadius { get; set; }
        public float ExplosureImpulse { get; set; }
        public float ExplosureTime { get; set; }
        public bool ExplosionByHit { get; set; }

        public UnityRef<GameObject> ThrowObject { get; set; }
        public UnityRef<GameObject> ExplosionObject { get; set; }
        public UnityRef<GameObject> ShotObject { get; set; }

        public int MaxInnerStack { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
    }

    public enum ContainerUsageType
    {
        ItSelf,                 // использует сам себя как расходники
        InnerContainer,         // использует внутренний контейнер (к примеру, стрелы) как расходники
        ContainerInContainer,   // использует внутренний контейнер внутреннего контейнера (двойная вложенность, к примеру, пули в заряднике) как расходники
    }
}