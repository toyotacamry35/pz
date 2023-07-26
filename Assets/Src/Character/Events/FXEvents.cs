using Assets.Src.ResourcesSystem.Base;
using Assets.Tools;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Entities.Reactions;
using SharedCode.Utils;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using SharedCode.Serializers;

namespace Assets.Src.Character.Events
{
    public class FXEvents
    {
        private readonly TriggerFXRule[] _rules;

        public FXEvents(FXEventsDef def)
        {
            if (def.FXEvents != null)
                _rules = def.FXEvents.Select(x => new TriggerFXRule(x)).ToArray();
        }

        public void Update(VisualEvent evt, Animator animator)
        {
            if (_rules == null)
                return;
            
            AsyncUtils.RunAsyncTask(async () =>
            {
                int rulesToUpdateCount = 0;
                var rulesToUpdate = PooledArray<(TriggerFXRule rule, VisualEvent evt)>.Create(_rules.Length);
                foreach (var rule in _rules)
                    if (rule.CheckEventType(evt.eventType) && !HasRuleWithEventType(rulesToUpdate.Array, rulesToUpdateCount, evt.eventType))
                        if (await rule.CheckPredicate(evt))
                            rulesToUpdate.Array[rulesToUpdateCount++] = (rule, evt);
                if (rulesToUpdateCount > 0)
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () =>
                        {
                            for (int i = 0; i < rulesToUpdateCount; ++i)
                            {
                                var tuple = rulesToUpdate.Array[i];
                                tuple.rule.UpdateEvent(tuple.evt, animator);
                            }
                            rulesToUpdate.Dispose();
                        });
            });
        }

        private bool HasRuleWithEventType((TriggerFXRule, VisualEvent)[] list, int count, FXEventType eType)
        {
            if( list != null)
                for (int i=0; i<count; ++i)
                    if (list[i].Item1.CheckEventType(eType))
                        return true;
            return false;
        }
    }

    public interface IVisualEvent
    {
        FXEventType eventType { get; }
        OuterRef<IEntity> casterEntityRef { get; }
        IEntitiesRepository casterRepository { get; }
        GameObject casterGameObject { get; }
        OuterRef<IEntity> targetEntityRef { get; }
        IEntitiesRepository targetRepository { get; }
        GameObject targetGameObject { get; }
        Vector3 position { get; } // Дефолтные позиция и поворот в мировых координатах. Для конкретных хендлеров могут быть переопределены параметрами.
        Quaternion rotation { get; } //
        ArgTuple[] parameters { get; }
    }
    
    public class VisualEvent : IVisualEvent
    {
        public FXEventType eventType { get; set; }
        public OuterRef<IEntity> casterEntityRef { get; set; }
        public IEntitiesRepository casterRepository { get; set; }
        public GameObject casterGameObject { get; set; }
        public OuterRef<IEntity> targetEntityRef { get; set; }
        public IEntitiesRepository targetRepository { get; set; }
        public GameObject targetGameObject { get; set; }
        public Vector3 position { get; set; } // Дефолтные позиция и поворот в мировых координатах. Для конкретных хендлеров могут быть переопределены параметрами.
        public Quaternion rotation { get; set; } //
        public ArgTuple[] parameters { get; set; }

        public override string ToString()
        {
            return eventType.ToString();
        }

        public string ToStringLong()
        {
            return StringBuildersPool.Get
                .Append("[")
                .Append("Event:").Append(eventType.____GetDebugAddress())
                .Append(" CasterRef:").Append(casterEntityRef.ToString())
                .Append(" CasterObj:").Append(casterGameObject?.transform.FullName())
                .Append(" TargetRef:").Append(targetEntityRef.ToString())
                .Append(" TargetObj:").Append(targetGameObject?.transform.FullName())
                .Append(" Position:").Append(position.ToString())
                .Append(" Rotation:").Append(rotation.ToString())
                .Append(parameters != null ? $" Parameters:[{string.Join(" ,", parameters.Select(x => $"{x.Def.____GetDebugAddress()}:{x.Value.Value}"))}]" : string.Empty)
                .Append("]")
                .ToStringAndReturn();
        }
    }
}
