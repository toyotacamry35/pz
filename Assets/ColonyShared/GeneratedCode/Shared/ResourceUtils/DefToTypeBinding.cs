using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode;
using SharedCode.MovementSync;

namespace GeneratedCode.DeltaObjects
{
    #region ImpactBinding
    public interface IImpactBinding {} 
    
    public interface IImpactBinding<TargetImpactType> : IImpactBinding 
        where TargetImpactType : SpellImpactDef
    {
        ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, TargetImpactType def);
    }
    public abstract class ImpactBinding : BindingHolder<IImpactBinding, SpellImpactDef>
    {
        public abstract ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, SpellImpactDef def);
    }
    public class ImpactBinding<T> : ImpactBinding where T: SpellImpactDef
    {
        public override ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, SpellImpactDef def)
        {
            var indef = (T)def;
            return ((IImpactBinding<T>)Instance).Apply(cast, repo, indef);
        }
    }
    #endregion

    #region ShapeBinding
    public interface IShapeBinding {}
    public interface IShapeBinding<TargetImpactType> : IShapeBinding
      where TargetImpactType : ShapeDef
    {
        Task<bool> CheckCollision(TargetImpactType def, Transform shapeOwnerTransform, Vector3 point, IEntitiesRepository repo);
        Task<List<VisibilityDataSample>> GetPosibleVisibilityData(TargetImpactType def, SpellPredCastData castData, Guid worldspaceId, Transform shapeOwnerTransform, IEntitiesRepository repo);

    }
    public abstract class ShapeBinding : BindingHolder<IShapeBinding, ShapeDef>
    {
        public abstract Task<bool> CheckCollision(ShapeDef def, Transform shapeOwnerTransform, Vector3 point, IEntitiesRepository repo);
        public abstract Task<List<VisibilityDataSample>> GetPosibleVisibilityData(ShapeDef def, SpellPredCastData castData, Guid worldspaceId, Transform shapeOwnerTransform, IEntitiesRepository repo);
    }
    public class ShapeBinding<T> : ShapeBinding where T : ShapeDef
    {
        public override Task<bool> CheckCollision(ShapeDef def, Transform shapeOwnerTransform, Vector3 point, IEntitiesRepository repo)
        {
            var indef = (T)def;
            return ((IShapeBinding<T>)Instance).CheckCollision(indef, shapeOwnerTransform, point, repo);
        }

        public override Task<List<VisibilityDataSample>> GetPosibleVisibilityData(ShapeDef def, SpellPredCastData castData, Guid worldspaceId, Transform shapeOwnerTransform, IEntitiesRepository repo)
        {
            var indef = (T)def;
            return ((IShapeBinding<T>)Instance).GetPosibleVisibilityData(indef, castData, worldspaceId, shapeOwnerTransform, repo);
        }
    }
    #endregion
    
    #region CalcerBinding
    public interface ICalcerBinding<TargetCalcerType, ReturnType>
        where TargetCalcerType : CalcerDef<ReturnType>
    {
        ValueTask<ReturnType> Calc(TargetCalcerType def, CalcerContext ctx);
        IEnumerable<StatResource> CollectStatNotifiers(TargetCalcerType def);
    }

    public abstract class CalcerBinding
    {
        public object CalcerImplInstance;
        public abstract IEnumerable<StatResource> CollectStatNotifiers(CalcerDef def);

        public abstract ValueTask<Value> Calc(CalcerDef def, CalcerContext ctx);
    }
    
    public abstract class CalcerBinding<ReturnType> : CalcerBinding
    {
        public abstract ValueTask<ReturnType> Calc(CalcerDef<ReturnType> def, CalcerContext ctx);

        public override async ValueTask<Value> Calc(CalcerDef def, CalcerContext ctx) =>
            ValueConverter<ReturnType>.Convert(await Calc((CalcerDef<ReturnType>) def, ctx));
    }

    public class CalcerBinding<TargetCalcerType,ReturnType> : CalcerBinding<ReturnType> where TargetCalcerType : CalcerDef<ReturnType>
    {
        public override ValueTask<ReturnType> Calc(CalcerDef<ReturnType> def, CalcerContext ctx)
        {
            return ((ICalcerBinding<TargetCalcerType,ReturnType>) CalcerImplInstance).Calc((TargetCalcerType)def, ctx);
        }

        public override IEnumerable<StatResource> CollectStatNotifiers(CalcerDef def)
        {
            return ((ICalcerBinding<TargetCalcerType,ReturnType>) CalcerImplInstance).CollectStatNotifiers((TargetCalcerType)def);
        }
    }
    
    public interface ICalcerBindingsCollector
    {
        IEnumerable<Type> Collect();
    }

    #endregion

    #region EffectBinding
    
    public interface IEffectBinding {}
    
    public interface IEffectBinding<TargetEffectType> : IEffectBinding 
        where TargetEffectType : SpellEffectDef
    {
        ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, TargetEffectType def);
        ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, TargetEffectType def);
    }

    public interface IClientOnlyEffectBinding : IEffectBinding {}

    public interface IClientOnlyEffectBinding<TargetEffectType> : IClientOnlyEffectBinding
        where TargetEffectType : SpellEffectDef
    {
        ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, TargetEffectType def);
        ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, TargetEffectType def);
    }
  
    public abstract class EffectBinding : BindingHolder<IEffectBinding, SpellEffectDef>
    {
        public abstract ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def);
        public abstract ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def);
    }
    
    public class EffectBinding<T> : EffectBinding where T : SpellEffectDef
    {
        public override ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def)
        {
            var indef = (T)def;
            return ((IEffectBinding<T>)Instance).Attach(cast, repo, indef);
        }

        public override ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def)
        {
            var indef = (T)def;
            return ((IEffectBinding<T>)Instance).Detach(cast, repo, indef);
        }
    }

    public class ClientOnlyEffectBinding<T> : EffectBinding where T : SpellEffectDef
    {
        public override ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def)
        {
            if (!cast.OnClient()) throw new Exception($"Attempt to start client only effect not on client | Effect{def} Cast:{cast} Repo:{repo}");
            var indef = (T)def;
            return ((IClientOnlyEffectBinding<T>)Instance).AttachOnClient(cast, repo, indef);
        }

        public override ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, SpellEffectDef def)
        {
            if (!cast.OnClient()) throw new Exception($"Attempt to finish client only effect not on client | Effect{def} Cast:{cast} Repo:{repo}");
            var indef = (T)def;
            return ((IClientOnlyEffectBinding<T>)Instance).DetachOnClient(cast, repo, indef);
        }
    }
    
    #endregion
    
    
    #region PredicateBinding

    public interface IPredicateBinding
    {}
    
    public interface IPredicateBinding<TargetPredicateType> : IPredicateBinding
        where TargetPredicateType : SpellPredicateDef
    {
        ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, TargetPredicateType def);
    }
    public interface IPredictablePredicateBinding {}
    public interface IPredictablePredicateBinding<TargetPredicateType> : IPredicateBinding<TargetPredicateType>
        where TargetPredicateType : SpellPredicateDef
    {}
    
    public abstract class PredicateBinding : BindingHolder<IPredicateBinding, SpellPredicateDef>
    {
        public abstract ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateDef def);
    }
    
    public class PredicateBinding<T> : PredicateBinding where T : SpellPredicateDef
    {
        public override ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateDef def)
        {
            var indef = (T)def;
            return ((IPredicateBinding<T>)Instance).True(cast, repo, indef);
        }
    }
    #endregion 
}
