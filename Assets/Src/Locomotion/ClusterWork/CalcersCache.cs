using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Utils.Threads;
using Src.Aspects.Impl.Stats;
using SharedPredicate = Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.PredicateDef;
    
namespace Src.Locomotion
{
    public partial class CalcersCache : IDisposable, ILocomotionUpdateable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

//        public delegate Task<float> CustomCalcer(CalcerContext e);
//        public delegate Task<bool> CustomPredicate(CalcerContext e);

        private readonly float _defaultPeriod;
        private readonly AsyncTaskRunner _taskRunner;
        private readonly OuterRef<IEntity> _entity;
        private readonly IEntitiesRepository _repository;
        private readonly List<CalcerHolder> _calcers = new List<CalcerHolder>(8);
        private bool _ready;

        public CalcersCache(
            OuterRef<IEntity> entity,
            IEntitiesRepository repository,
            AsyncTaskRunner taskRunner,
            float defaultPeriod)
        {
            _defaultPeriod = defaultPeriod;
            _taskRunner = taskRunner;
            _entity = entity;
            _repository = repository;
        }

        public void Dispose()
        {
            foreach (var holder in _calcers)
                holder.Dispose();
            _calcers.Clear();
        }

        public void Update(float deltaTime)
        {
            foreach (var calcer in _calcers)
                calcer.Update(deltaTime);
        }

        public bool IsReady => _ready || (_ready = _calcers.All(x => x.IsReady));

        //       public CalcerProxy Add(CustomCalcer calcer, float period = -1) => new CalcerProxy(AddInternal(new CustomCalcerWrapper(calcer), period), this);

        public CalcerProxy Add(CalcerDef<float> calcer, float period = -1)
        {
            return calcer != null ? new CalcerProxy(AddInternal(new SharedCalcerWrapper(calcer, null), period), this) : CalcerProxy.Null;
        }

        public CalcerProxy Add(CalcerDef<float> calcer, string argName, Func<float> argFn, float period = -1)
        {
            return calcer != null ? new CalcerProxy(AddInternal(new SharedCalcerWrapper(calcer, new[] {(argName, argFn)}), period), this) : CalcerProxy.Null;
        }

        public CalcerProxy Add(CalcerDef<float> calcer, string argName1, Func<float> argFn1, string argName2, Func<float> argFn2, float period = -1)
        {
            return calcer != null ? new CalcerProxy(AddInternal(new SharedCalcerWrapper(calcer, new[] {(argName1, argFn1), (argName2, argFn2)}), period), this) : CalcerProxy.Null;
        }

        //       public PredicateProxy Add(CustomPredicate predicate, float period = -1) => new PredicateProxy(AddInternal(new CustomPredicateWrapper(predicate), period), this);

        public PredicateProxy Add(SharedPredicate predicate, float period = -1)
        {
            return predicate != null ? new PredicateProxy(AddInternal(new SharedPredicateWrapper(predicate, null), period), this) : PredicateProxy.Null;
        }

        public PredicateProxy Add(SharedPredicate predicate, string argName, Func<float> argFn, float period = -1)
        {
            return predicate != null ? new PredicateProxy(AddInternal(new SharedPredicateWrapper(predicate, new[] {(argName, argFn)}), period), this) : PredicateProxy.Null;
        }

        public PredicateProxy Add(SharedPredicate predicate, string argName1, Func<float> argFn1, string argName2, Func<float> argFn2, float period = -1)
        {
            return predicate != null ? new PredicateProxy(AddInternal(new SharedPredicateWrapper(predicate, new[] {(argName1, argFn1), (argName2, argFn2)}), period), this) : PredicateProxy.Null;
        }

        private int AddInternal(ICalcerWrapper calcer, float delay)
        {
            if (calcer == null) throw new ArgumentNullException(nameof(calcer));
            var idx = _calcers.Count;
            var holder = new CalcerHolder(calcer, _taskRunner, _repository, _entity, delay >= 0 ? delay : _defaultPeriod);
            _calcers.Add(holder);
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Add calcer | {@}", new {Calcer = calcer, idx, holder.Period, Entity = _entity.Guid}).Write();
            holder.Activate();
            return idx;
        }

        /// <summary>
        /// CalcerProxy
        /// </summary>
        public struct CalcerProxy
        {
            public static readonly CalcerProxy Null = new CalcerProxy(-1, null);
            private readonly int _idx;
            private readonly CalcersCache _cache;

            public CalcerProxy(int idx, CalcersCache cache)
            {
                _idx = idx;
                _cache = cache;
            }
            
            public float Value => _idx >= 0 ? _cache._calcers[_idx].Value : 0;

            public static implicit operator float(CalcerProxy p) => p.Value;
        }
        
        /// <summary>
        /// PredicateProxy
        /// </summary>
        public struct PredicateProxy
        {
            public static readonly PredicateProxy Null = new PredicateProxy(-1, null);
            
            private readonly int _idx;
            private readonly CalcersCache _cache;

            public PredicateProxy(int idx, CalcersCache cache)
            {
                _idx = idx;
                _cache = cache;
            }
            
            public bool Value => _idx >= 0 && _cache._calcers[_idx].Value > 0;

            public static implicit operator bool(PredicateProxy p) => p.Value;
        }


        private interface ICalcerWrapper
        {
            IEnumerable<StatResource> CollectNotifiers();
            ValueTask<float> Calc(CalcerContext e);
            (bool Changed, CalcerContext.Arg[] Args) CalcArgs(CalcerContext.Arg[] prevArgs);
        }
    
        // private class CustomCalcerWrapper : ICalcerWrapper
        // {
        //     private readonly CustomCalcer _calcer;
        //     public CustomCalcerWrapper(CustomCalcer calcer) { _calcer = calcer; }
        //     public async Task<float> Calc(CalcerContext e) => await _calcer(e);
        //     public void CalcArgs(ref CalcerContext.Arg[] args) {}
        // }
        //
        // private class CustomPredicateWrapper : ICalcerWrapper
        // {
        //     private readonly CustomPredicate _predicate;
        //     public CustomPredicateWrapper(CustomPredicate predicate) { _predicate = predicate; }
        //     public async Task<float> Calc(CalcerContext e) => await _predicate(e) ? 1 : 0;
        //     public void CalcArgs(ref CalcerContext.Arg[] args) {}
        // }

        private class SharedCalcerWrapper : CalcerWithArgsWrapper, ICalcerWrapper
        {
            private readonly CalcerDef<float> _calcer;
            public SharedCalcerWrapper(CalcerDef<float> calcer, (string,Func<float>)[] args) : base(args) { _calcer = calcer ?? throw new ArgumentNullException(nameof(calcer)); }
            public IEnumerable<StatResource> CollectNotifiers() => _calcer.CollectStatNotifiers();
            public ValueTask<float> Calc(CalcerContext e) => _calcer.CalcAsync(e);
            public override string ToString() => _calcer.__DebugName;
        }
            
        private class SharedPredicateWrapper : CalcerWithArgsWrapper, ICalcerWrapper
        {
            private readonly SharedPredicate _predicate;
            public SharedPredicateWrapper(SharedPredicate predicate, (string, Func<float>)[] args) : base(args) { _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate)); }
            public IEnumerable<StatResource> CollectNotifiers() => _predicate.CollectStatNotifiers();
            public async ValueTask<float> Calc(CalcerContext e) => await _predicate.CalcAsync(e) ? 1 : 0;
            public override string ToString() => _predicate.__DebugName;
        }
        
        private abstract class CalcerWithArgsWrapper
        {
            private readonly (string Name, Func<float> Func)[] _args;
            
            protected CalcerWithArgsWrapper((string,Func<float>)[] args) 
            { 
                _args = args;
            }

            public (bool, CalcerContext.Arg[]) CalcArgs(CalcerContext.Arg[] oldArgs)
            {
                if (_args == null || _args.Length == 0)
                    return (false, null);
                    
                CalcerContext.Arg[] newArgs = null;
                for (int i = 0; i < _args.Length; ++i)
                {
                    var newValue = _args[i].Func();
                    if (oldArgs == null || !SharedHelpers.Approximately(newValue, oldArgs[i].Value.Float))
                    {
                        if (newArgs == null) newArgs = new CalcerContext.Arg[_args.Length];
                        newArgs[i] = new CalcerContext.Arg(_args[i].Name, new Value(newValue));
                    }
                }
                return (newArgs != null, newArgs ?? oldArgs);
            }
        }
    }
}
