using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;
 
namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{
    public class ComputableStateMachineDef : BaseResource
    {
        public List<ComputableStateData> StatesTable;
    }

    public struct ComputableStateData
    {
        public ResourceRef<ComputableStatesDef> States;
        public ResourceRef<ComputableStateMachinePredicateDef> Predicate;
    }

    // This proxy wrapper-class is useful, 'cos to send via net it needed 3 numbers. To send a list directly, we need send an array
    public class ComputableStatesDef : BaseResource
    {
        public List<ResourceRef<ComputableStateDef>> States;
    }

    public class ComputableStateDef : BaseResource
    {
    }

    public interface IPrefabStateDef
    {
        UnityRef<GameObject> Prefab { get; set; }
    }

    public class PrefabStateDef : ComputableStateDef, IPrefabStateDef
    {
        public UnityRef<GameObject> Prefab { get; set; }
    }

    public class LootTableStateDef : ComputableStateDef
    {
        public ResourceRef<LootTableBaseDef> LootTable { get; set; }
    }
}
