using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class SpawnLegionaryAndRememberIt : BehaviourNode
    {
        SpawnLegionaryAndRememberItDef _def;
        TargetSelector _target;
        TargetSelector _memory;
        TargetSelector _legion;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (SpawnLegionaryAndRememberItDef)def;
            _target = (TargetSelector)await _def.Target.ExprOptional(HostStrategy);
            _memory = (TargetSelector)await _def.MemoryOf.ExprOptional(HostStrategy);
            _legion = (TargetSelector)await _def.Legion.ExprOptional(HostStrategy);
        }
        TimedMemory _timedMemory;
        int _waitFor = 10;
        public override async ValueTask<ScriptResultType> OnStart()
        {
            return ScriptResultType.Failed;
            //_waitFor = 10;
            //_spawnedLegionary = null;
            //_timedMemory = null;
            //Legionary targetLegionary = null;
            //if (_target != null)
            //    targetLegionary = _target.SelectTarget(HostStrategy.CurrentLegionary).Legionary;
            //else
            //    targetLegionary = HostStrategy.CurrentLegionary;
            //if (targetLegionary == null)
            //    return ScriptResultType.Failed;
            //TimedMemory memory = null;
            //if (_memory != null)
            //    memory = _memory.SelectTarget(HostStrategy.CurrentLegionary)?.Legionary.Knowledge.Memory;
            //else
            //    memory = HostStrategy.CurrentLegionary.Knowledge.Memory;
            //if (memory == null)
            //    return ScriptResultType.Failed;
            //var tran = ((GOLegionary)targetLegionary).ControlledGO.transform;
            //Legion assignedLegion = HostStrategy.CurrentLegionary.AssignedLegion;
            //if (_legion != null)
            //    assignedLegion = (Legion)_legion.SelectTarget(HostStrategy.CurrentLegionary);

            //var go = GameObjectCreator.CreateGameObject(_def.Prefab.Target, tran.position + tran.TransformVector(_def.Offset), tran.rotation, null, x => { x.GetComponent<SpatialLegionary>().GetAssignedToLegion = assignedLegion; return Task.CompletedTask; });
            //var spawnedLegionary = go.GetComponent<SpatialLegionary>();
            //HostStrategy.ShouldTickWithDelay(0.07f);
            //if (_def.AsStat.Target != null)
            //{
            //    _spawnedLegionary = spawnedLegionary;
            //    _timedMemory = memory;
            //    return ScriptResultType.Running;
            //}
            //else
            //    return ScriptResultType.Succeeded;

        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            /*if (_spawnedLegionary.Legionary != null)
            {
                _timedMemory.SetStatMod(_spawnedLegionary.Legionary, SyncTime.FromSeconds(_def.ForTime), _def.AsStat, 1f, new StatModKey() { Assigner = HostStrategy.CurrentLegionary });
                return ScriptResultType.Succeeded;
            }
            if (_waitFor-- <= 0)//in case something goes wrong mob should not stuck here forewer
                return ScriptResultType.Failed;*/
            return ScriptResultType.Failed;
        }
    }
}
