using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;

namespace SharedCode.Wizardry
{
    // Its full data of (sub)spell (incl. its state)
    public readonly struct SpellTimeLineData
    {
        public readonly TimeRange TimeRange; // начало и конец относительно начала parent'ового subspell'а!. конец и длительность равны long.MaxValue для бесконечных (sub)spell'ов
        public readonly long Period; // период повтора
        public readonly int  ActivationsLimit; // ограничение на количество повторов
        public readonly bool MustNotFail;
        public readonly SpellDef SpellDef;
        public readonly TimelineHelpers.ExecutionMask ExecutionMask;

        public SpellTimeLineData(SpellDef spellDef, TimeRange timeRange, long period, bool mustNotFail, int activationsLimit, TimelineHelpers.ExecutionMask executionMask)
        {
            TimeRange = timeRange;
            Period = period;
            MustNotFail = mustNotFail;
            SpellDef = spellDef;
            ExecutionMask = executionMask;
            ActivationsLimit = activationsLimit;
        }

        public long StartOffset => TimeRange.Start;
        public long Duration => TimeRange.Duration;
        public bool IsInfinite => TimeRange.Duration == long.MaxValue;

        public override string ToString()
        {
            return $"{SpellDef.____GetDebugShortName()}/{SpellDef.Name}";
        }
        
        // public SpellTimeLineData Clone()
        // {
        //     return new SpellTimeLineData(
        //         activationsLimit: ActivationsLimit,
        //         timeRange: TimeRange,
        //         period: Period,
        //         spellDef: SpellDef,
        //         mustNotFail: MustNotFail,
        //         executionMask: ExecutionMask
        //     );
        // }
        //
        // object ICloneable.Clone() => Clone();
    }


    public class SpellStateValidator
    {
        HashSet<OuterRef<IEntity>> _importantEntities = new HashSet<OuterRef<IEntity>>();
        public SpellStateValidator(ISpell spell)
        {
            foreach (var iEntity in spell.CastData.GetAllImportantEntities())
                _importantEntities.Add(iEntity);
        }

        public bool IsNowInvalid(OuterRef<IEntity> entityThatLeftTheRepo)
        {
            return _importantEntities.Contains(entityThatLeftTheRepo);
        }

        public async Task<bool> IsValid(IEntitiesRepository entitiesRepository)
        {
            return ! await IsInvalid(entitiesRepository);
        }

        public async Task<bool> IsInvalid(IEntitiesRepository entitiesRepository)
        {
            foreach(var entity in _importantEntities)
            {
                using (var ent = await entitiesRepository.Get(entity))
                {
                    if (ent.Get<IEntity>(entity) == null)
                        return true;
                }
            }
            return false;
        }
    }
}
