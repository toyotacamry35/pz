using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using ColonyShared.GeneratedCode.Combat;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using static Src.ManualDefsForSpells.EffectAnimatorDef;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IAnimationModifier
    {
        string DebugName { get; }
    }
    
    public interface IAnimationModifiersFactory
    {
        IAnimationModifier IntModifier(AnimationParameterDef parameter, int value);

        IAnimationModifier BoolModifier(AnimationParameterDef parameter, bool value);
		
        IAnimationModifier BoolWithTriggerModifier(AnimationParameterTupleDef parameter, bool boolAndTriggerValue);

        IAnimationModifier FloatModifier(AnimationParameterDef parameter, float value);

        IAnimationModifier FloatModifier(AnimationParameterDef parameter, float startValue, float endValue, float time);
        
        IAnimationModifier Trigger(AnimationParameterDef parameter);
        
        IAnimationModifier LayerWeightModifier(AnimationLayerDef layer, float value);

        IAnimationModifier LayerWeightModifier(AnimationLayerDef layer, float startValue, float endValue, float time);

        IAnimationModifier State(AnimationStateDef parameter, Mode mode, object playId);        

        IAnimationModifier State(AnimationStateDef state, float length, Mode mode, object playId);
        
        IAnimationModifier State(AnimationStateDef state, float length, Mode mode, TimeRange timeRange, Func<long> clock, object playId);   
    }
    
    
    public interface IAnimationDoer
    {
        /// <summary>
        /// Установить модификатор без привязки к causer
        /// </summary>
        void Set(IAnimationModifier modifier);

        /// <summary>
        /// Установить модификатор 
        /// </summary>
        void Push(object causer, IAnimationModifier mod);

        /// <summary>
        /// Снять модификатор немедленно
        /// </summary>
        void Pop(object causer, IAnimationModifier mod);

        /// <summary>
        /// Отвязать модификатор от causer (отложенное снятие) 
        /// </summary>
        void Detach(object causer, IAnimationModifier mod);

        /// <summary>
        /// Заменить модификатор 
        /// </summary>
        void Replace(object causer,  [CanBeNull] object newCauser, IAnimationModifier mod);
        
        IAnimationModifiersFactory ModifiersFactory { get; }
    }
    
    
    public static class AnimationModifiersFactoryExtensions
    {
        private static readonly System.Random Random = new Random();
        
        public static void Create(this IAnimationModifiersFactory factory, AnimatorModifierDef param, List<IAnimationModifier> res)
        {
            res.Clear();
            switch (param)
            {
                case BoolWithTriggerParameterDef p:
                    res.Add(factory.BoolWithTriggerModifier(p.Parameter, p.Value));
                    break;
                case BoolParameterDef p:
                    res.Add(factory.BoolModifier(p.Parameter, p.Value));
                    break;
                case IntParameterDef p:
                    res.Add(factory.IntModifier(p.Parameter, p.Value));
                    break;
                case FloatParameterDef p:
                    res.Add(factory.FloatModifier(p.Parameter, p.Value));
                    break;
                case RandomFloatParameterDef p:
                    res.Add(factory.FloatModifier(p.Parameter, (float)(Random.NextDouble() * (p.MaxValue - p.MinValue) + p.MinValue)));
                    break;
                case LayerWeightDef p:
                    res.Add(factory.LayerWeightModifier(p.Layer, p.Value));
                    break;
                case SmoothFloatParameterDef p:
                    res.Add(factory.FloatModifier(p.Parameter, p.StartValue, p.EndValue, p.Time));
                    break;
                case SmoothLayerWeightDef p:
                    res.Add(factory.LayerWeightModifier(p.Layer, p.StartValue, p.EndValue, p.Time));
                    break;
                case TriggerParameterDef p:
                    res.Add(factory.Trigger(p.Parameter));
                    break;
                default:
                    throw new NotSupportedException($"param is {param.GetType()}");
            }
        }

        public static async Task Create(this IAnimationModifiersFactory factory, AnimatorModifierDef param, SpellWordCastData cast, OuterRef target, IEntitiesRepository repo, List<IAnimationModifier> res)
        {
            res.Clear();
            switch (param)
            {
                case CalcerFloatParameterDef p:
                    using (var cnt = await repo.Get(target.TypeId, target.Guid))
                        res.Add(factory.FloatModifier(p.Parameter,
                            await p.Calcer.Target.CalcAsync(new CalcerContext(cnt, target, repo, cast))));
                    break;
                case StateDef p:
                {
                    if (p.State == null) throw new ArgumentNullException($"State is null at {p.____GetDebugAddress()}");
                    var playId = AnimationPlayId.CreatePlayId(cast, p);
                    if (Math.Abs(p.Duration - StateDef.DurationAdjusted) < 0.001f)
                        res.Add(factory.State(p.State, p.Length, p.Mode, cast.WordTimeRange,
                            SyncTime.StableClock, playId));
                    else if (Math.Abs(p.Duration - StateDef.DurationOriginal) < 0.0001f)
                        res.Add(factory.State(p.State, p.Length, p.Mode, playId));
                    else if (p.Duration > StateDef.DurationOriginal)
                        res.Add(factory.State(p.State, p.Length, p.Mode,
                            TimeRange.FromDuration(cast.WordTimeRange.Start, SyncTime.FromSeconds(p.Duration)), SyncTime.StableClock,
                            playId));
                    else
                        throw new ArgumentException($"Invalid Duration:{p.Duration}");
                    break;
                }
                case Vector2ParameterDef p:
                {
                    var vec = p.Vector.Target.GetVector(cast);
                    res.Add(factory.FloatModifier(p.ParameterX, vec.x));
                    res.Add(factory.FloatModifier(p.ParameterY, vec.y));
                    break;
                }
                case Vector3ParameterDef p:
                {
                    var vec = p.Vector.Target.GetVector(cast);
                    res.Add(factory.FloatModifier(p.ParameterX, vec.x));
                    res.Add(factory.FloatModifier(p.ParameterY, vec.y));
                    res.Add(factory.FloatModifier(p.ParameterZ, vec.z));
                    break;
                }
                default:
                    factory.Create(param, res);
                    break;
            }
        }
    }
}
