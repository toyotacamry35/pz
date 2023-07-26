using System;
using Assets.Src.Aspects.Impl.Stats;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers.Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Utils;
using Core.Reflection;
using SharedCode.Utils;
using Scripting;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.Wizardry
{
    [ProtoContract]

    public interface ISpellCast
    {

        [ProtoMember(1)] SpellDef Def { get; }
        [ProtoIgnore] long StartAt { get; }
        [ProtoIgnore] long Duration { get; }
        [ProtoIgnore] bool IsInfinite { get; }
        [ProtoMember(3)] ScriptingContext Context { get; set; }
        SpellCastParameter[] GetParameters();
        IEnumerable<OuterRef<IEntity>> GetAllImportantEntities();
        ISpellCast Clone();
    }
    
    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public class SpellCast : ISpellCast
    {
        [BsonIgnore]
        [ProtoMember(1)] public SpellDef Def { get; set; }
        [ProtoMember(2)] public long StartAt { get; set; }
        [ProtoMember(3)] public ScriptingContext Context { get; set; } = new ScriptingContext();
        [ProtoMember(4)] public PropertyAddress Owner { get; set; }
        
        [ProtoIgnore] public virtual long Duration => SyncTime.FromSeconds(Def.Duration);

        [ProtoIgnore] public virtual bool IsInfinite => Def.IsInfinite;
        
        public IEnumerable<OuterRef<IEntity>> GetAllImportantEntities()
        {
            return GetAllImportantEntitiesInternal().Where(x => x.IsValid);
        }

        ISpellCast ISpellCast.Clone() => Clone();

        public virtual SpellCast Clone() => FillCloneInternal(new SpellCast());

        public virtual SpellCastParameter[] GetParameters() => Array.Empty<SpellCastParameter>();
        
        protected virtual IEnumerable<OuterRef<IEntity>> GetAllImportantEntitiesInternal()
        {
            yield break;
        }

        protected SpellCast FillCloneInternal(SpellCast clone)
        {
            clone.Def = Def;
            clone.StartAt = StartAt;
            clone.Context = Context.Clone();
            clone.Owner = Owner.Clone();
            return clone;
        }
        
        public override string ToString()
        {
            var @params = GetParameters();
            return $"Cast:{GetType().Name} Spell:{Def?.____GetDebugAddress()} StartAt:{StartAt.TimeToString()}" + (@params?.Any()==true ? $" Params:[{string.Join(", ", (IEnumerable<SpellCastParameter>)@params)}]" : string.Empty);

        }
    }
    
    [ProtoContract]
    public class SpellCastWithParameters : SpellCast
    {
        [ProtoMember(tag: 3,OverwriteList = true)] public SpellCastParameter[] Parameters;
        
        protected override IEnumerable<OuterRef<IEntity>> GetAllImportantEntitiesInternal()
        {
            return Parameters?.SelectMany(x => x.ImportantEntities) ?? Enumerable.Empty<OuterRef<IEntity>>();
        }

        public override SpellCastParameter[] GetParameters() => Parameters; 

        public override long Duration => this.TryGetParameter<SpellCastParameterDuration>(out var duration) ? duration.DurationTimeUnits : base.Duration;

        public override bool IsInfinite => !this.TryGetParameter<SpellCastParameterDuration>(out _) && base.IsInfinite;
        
        public override SpellCast Clone() => FillCloneInternal(new SpellCastWithParameters());

        protected SpellCastWithParameters FillCloneInternal(SpellCastWithParameters clone)
        {
            base.FillCloneInternal(clone);
            clone.Parameters = SpellCastParameters.Clone(Parameters);
            return clone;
        }
    }

    [ProtoContract]
    [AutoProtoIncludeSubTypes]
//    [ProtoInclude(100, typeof(SpellCastParameterTarget))]
//    [ProtoInclude(110, typeof(SpellCastParameterTarget))]
    public abstract class SpellCastParameter
    {
        [ProtoIgnore] public IEnumerable<OuterRef<IEntity>> ImportantEntities => GetImportantEntities().Where(x => x.IsValid);
        [ProtoIgnore] public virtual Value Value { get => new Value(); set {} }
       
        protected virtual IEnumerable<OuterRef<IEntity>> GetImportantEntities()
        {
            yield break;
        }

        public abstract SpellCastParameter Clone();
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellTargetDef))]
    public class SpellCastParameterTarget : SpellCastParameter
    {
        [ProtoMember(1)] public OuterRef<IEntity> Target;
        [ProtoIgnore] public override Value Value { get => new Value(new OuterRef(Target.Guid, Target.TypeId)); set => Target = new OuterRef<IEntity>(value.OuterRef.Guid, value.OuterRef.TypeId); }

        protected override IEnumerable<OuterRef<IEntity>> GetImportantEntities()
        {
            yield return Target;
        }
        public override SpellCastParameter Clone() => new SpellCastParameterTarget{ Target = Target };
        public override string ToString() => $"Target:{Target.Guid}";
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellDirectionDef))]
    public class SpellCastParameterDirection : SpellCastParameter
    {
        [ProtoMember(1)] public Vector3 Direction;
        [ProtoIgnore] public override Value Value { get => new Value(Direction); set => Direction = value.Vector3; }
        public override SpellCastParameter Clone() => new SpellCastParameterDirection{ Direction = Direction };
        public override string ToString() => $"Direction:{Direction}";
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellDirection2Def))]
    public class SpellCastParameterDirection2 : SpellCastParameter
    {
        [ProtoMember(1)] public Vector2 Direction;
        [ProtoIgnore] public override Value Value { get => new Value(Direction); set => Direction = value.Vector2; }
        public override SpellCastParameter Clone() => new SpellCastParameterDirection2{ Direction = Direction };
        public override string ToString() => $"Direction:{Direction}";
    }
    
    [ProtoContract]
    [TypeToDef(typeof(SpellTargetPointDef))]
    public class SpellCastParameterPosition : SpellCastParameter
    {
        [ProtoMember(1)] public Vector3 Position;
        [ProtoIgnore] public override Value Value { get => new Value(Position); set => Position = value.Vector3; }
        public override SpellCastParameter Clone() => new SpellCastParameterPosition{ Position = Position };
        public override string ToString() => $"Position:{Position}";
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellLocalPointDef))]
    public class SpellCastParameterLocalPosition : SpellCastParameter
    {
        [ProtoMember(1)] public Vector3 Position;
        [ProtoIgnore] public override Value Value { get => new Value(Position); set => Position = value.Vector3; }
        public override SpellCastParameter Clone() => new SpellCastParameterLocalPosition{ Position = Position };
        public override string ToString() => $"LocalPosition:{Position}";
    }
    
    [ProtoContract]
    [TypeToDef(typeof(SpellPoint2Def))]
    public class SpellCastParameterPosition2 : SpellCastParameter
    {
        [ProtoMember(1)] public Vector2 Position;
        [ProtoIgnore] public override Value Value { get => new Value(Position); set => Position = value.Vector2; }
        public override SpellCastParameter Clone() => new SpellCastParameterPosition2{ Position = Position };
        public override string ToString() => $"Position:{Position}";
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellRotationDef))]
    public class SpellCastParameterRotation : SpellCastParameter
    {
        [ProtoMember(1)] public Quaternion Rotation;
        [ProtoIgnore] public override Value Value { get => new Value(Rotation); set => Rotation = value.Quaternion; }
        public override SpellCastParameter Clone() => new SpellCastParameterRotation{ Rotation = Rotation };
        public override string ToString() => $"Rotation:{Rotation}";
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellLocalRotationDef))]
    public class SpellCastParameterLocalRotation : SpellCastParameter
    {
        [ProtoMember(1)] public Quaternion LocalRotation;
        [ProtoIgnore] public override Value Value { get => new Value(LocalRotation); set => LocalRotation = value.Quaternion; }
        public override SpellCastParameter Clone() => new SpellCastParameterLocalRotation{ LocalRotation = LocalRotation };
        public override string ToString() => $"LocalRotation:{LocalRotation}";
    }

    [ProtoContract]
    public class SpellCastParameterInputAction : SpellCastParameter
    {
        [ProtoMember(1)] public InputActionDef InputAction;
        public override SpellCastParameter Clone() => new SpellCastParameterInputAction{ InputAction = InputAction };
        public override string ToString() => $"InputAction:{InputAction}";        
    }

    [ProtoContract]
    public class SpellCastParameterSlotItem : SpellCastParameter
    {
        [ProtoMember(1)] public bool IsInventory;
        [ProtoMember(2)] public int SlotId;
        public override SpellCastParameter Clone() => new SpellCastParameterSlotItem{ IsInventory = IsInventory, SlotId = SlotId };
        public override string ToString() => $"SlotId:{SlotId} IsInventory:{IsInventory}";                
    }
    
    [ProtoContract]
    [TypeToDef(typeof(SpellDurationDef))]
    public class SpellCastParameterDuration : SpellCastParameter
    {
        [ProtoMember(1)] public int _duration;
        
        public long DurationTimeUnits { get => _duration; set => _duration = (int)value; }
        public float DurationSeconds { get => SyncTime.ToSeconds(_duration); set => _duration = (int)SyncTime.FromSeconds(value); }
        [ProtoIgnore] public override Value Value { get => new Value(DurationSeconds); set => DurationSeconds = value.Float; }
        public override SpellCastParameter Clone() => new SpellCastParameterDuration{ _duration = _duration };
        public override string ToString() => $"Duration:{DurationSeconds}";                        
    }

    [ProtoContract]
    [TypeToDef(typeof(PrevSpellIdDef))]
    public class SpellCastParameterPrevSpellId : SpellCastParameter
    {
        [ProtoMember(1)] public SpellId SpellId;
        [ProtoIgnore] public override Value Value { get => SpellId.ToValue(); set => SpellId = value.SpellId(); }
        public override SpellCastParameter Clone() => new SpellCastParameterPrevSpellId{ SpellId = SpellId };
        public override string ToString() => $"SpellId:{SpellId}";                        
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellObjectNameDef))]
    public class SpellCastParameterObjectName : SpellCastParameter
    {
        [ProtoMember(1)] public string Name;
        [ProtoIgnore] public override Value Value { get => new Value(Name); set => Name = value.String; }
        public override SpellCastParameter Clone() => new SpellCastParameterObjectName{ Name = Name };
        public override string ToString() => $"Name:{Name}";                        
    }

    [ProtoContract]
    [TypeToDef(typeof(SpellDamageTypeDef))]
    public class SpellCastParameterDamageType : SpellCastParameter
    {
        [ProtoMember(1)] public DamageTypeDef DamageType;
        [ProtoIgnore] public override Value Value { get => new Value(DamageType); set => DamageType = value.Resource as DamageTypeDef; }
        public override SpellCastParameter Clone() => new SpellCastParameterDamageType{ DamageType = DamageType };
        public override string ToString() => $"DamageType:{DamageType}";                        
    }
    
    [ProtoContract]
    [UsedImplicitly]
    [TypeToDef(typeof(SpellWeaponSizeDef))]
    public class SpellCastParameterWeaponSize : SpellCastParameter
    {
        [ProtoMember(1)] public WeaponSizeDef WeaponSize;
        [ProtoIgnore] public override Value Value { get => new Value(WeaponSize); set => WeaponSize = value.Resource as WeaponSizeDef; }
        public override SpellCastParameter Clone() => new SpellCastParameterWeaponSize{ WeaponSize = WeaponSize };
        public override string ToString() => $"WeaponSize:{WeaponSize}";                        
    }

    [ProtoContract]
    [UsedImplicitly]
    [TypeToDef(typeof(SpellAttackTypeDef))]
    public class SpellCastParameterAttackType : SpellCastParameter
    {
        [ProtoMember(1)] public AttackTypeDef AttackType;
        [ProtoIgnore] public override Value Value { get => new Value(AttackType); set => AttackType = value.Resource as AttackTypeDef; }
        public override SpellCastParameter Clone() => new SpellCastParameterAttackType{ AttackType = AttackType };
        public override string ToString() => $"AttackType:{AttackType}";                        
    }
    
    [ProtoContract]
    public class SpellCastParameterCauser : SpellCastParameter
    {
        [ProtoMember(1)] public SpellPartCastId Causer;
        public override SpellCastParameter Clone() => new SpellCastParameterCauser{ Causer = Causer };
        public override string ToString() => $"Causer:{Causer}";                
    }

    [ProtoContract]
    [UsedImplicitly]
    [TypeToDef(typeof(SpellHitMaterialDef))]
    public class SpellCastParameterHitMaterial : SpellCastParameter
    {
        [ProtoMember(1)] public HitMaterialDef HitMaterial;
        [ProtoIgnore] public override Value Value { get => new Value(HitMaterial); set => HitMaterial = value.Resource as HitMaterialDef; }
        public override SpellCastParameter Clone() => new SpellCastParameterHitMaterial{ HitMaterial = HitMaterial };
        public override string ToString() => $"HitMaterial:{HitMaterial}";                        
    }

    public static class SpellCastParameters
    {
        private static readonly Dictionary<Type, Type> DefTypeToParamType = AppDomain.CurrentDomain
            .GetAssembliesSafeNonStandard()
            .SelectMany(x => x.GetTypesSafe())
            .Where(x => !x.IsInterface && !x.IsAbstract && typeof(SpellCastParameter).IsAssignableFrom(x) && x.GetCustomAttribute<TypeToDefAttribute>() != null)
            .ToDictionary(x => x.GetCustomAttribute<TypeToDefAttribute>().DefType, x => x);
        
        public static bool TryGetParameterTypeByDef(ISpellParameterDef def, out Type type)
        {
            type = null;
            return def != null && DefTypeToParamType.TryGetValue(def.GetType(), out type);
        }

        public static SpellCastParameter Create(ISpellParameterDef def, Value value)
        {
            if (TryGetParameterTypeByDef(def, out var paramType))
            {
                var param = Activator.CreateInstance(paramType) as SpellCastParameter;
                if (param == null) throw  new Exception($"Can't create parameter '{paramType}' (def:{def}')");
                param.Value = value;
                return param;
            }
            throw new NotSupportedException($"Unsupported parameter def '{def}'");
        }

        public static SpellCastParameter[] Clone(SpellCastParameter[] prms)
        {
            if (prms == null) return null;
            var rv = new SpellCastParameter[prms.Length];
            for (int i = 0; i < prms.Length; i++)
                rv[i] = prms[i] != null ? prms[i].Clone() : null;
            return rv;
        }
    }
    
    
    
    
    public static class SpellCastExtensions
    {
        public static T GetParameter<T>(this ISpellCast self) where T : SpellCastParameter
        {
            var @params = self.GetParameters();
            if (@params != null)
                foreach (var parameter in @params)
                    if (parameter is T)
                        return parameter as T;
            if (typeof(T) == typeof(SpellCastParameterTarget))
                return (T)(SpellCastParameter)new SpellCastParameterTarget();
            throw new KeyNotFoundException($"Spell cast of {self.Def} has no parameter {typeof(T).Name}!");
        }
        
        public static SpellCastParameter GetParameter(this ISpellCast self, Type paramType)
        {
            if (!typeof(SpellCastParameter).IsAssignableFrom(paramType)) throw new ArgumentException($"Invalid paramType:{paramType}");
            var @params = self.GetParameters();
            if (@params != null)
                foreach (var parameter in @params)
                    if (paramType.IsInstanceOfType(parameter))
                        return parameter;
            if (paramType == typeof(SpellCastParameterTarget))
                return (SpellCastParameter)new SpellCastParameterTarget();
            throw new KeyNotFoundException($"Spell cast of {self.Def} has no parameter {paramType.Name}!");
        }

        public static bool TryGetParameter<T>(this ISpellCast self, out T val) where T : SpellCastParameter
        {
            var @params = self.GetParameters();
            if (@params != null)
                foreach (var parameter in @params)
                    if (parameter is T)
                    {
                        val = parameter as T;
                        return true;
                    }
            val = default(T);
            return false;
        }
        
        public static bool TryGetParameter(this ISpellCast self, Type paramType, out SpellCastParameter val)
        {
            if (!typeof(SpellCastParameter).IsAssignableFrom(paramType)) throw new ArgumentException($"Invalid paramType:{paramType}");
            var @params = self.GetParameters();
            if (@params != null)
                foreach (var parameter in @params)
                    if (paramType.IsInstanceOfType(parameter))
                    {
                        val = parameter;
                        return true;
                    }
            val = null;
            return false;
        }
    }
    
    
    #region Obsolete
    
    public interface IStatEntity
    {
        IDeltaDictionary<StatResource, float> SimpleStupidStats { get; set; }
        Task SetStat(StatResource res, float setValue);
    }
    
    public interface IWithSlotItem : ISpellCast
    {
        bool IsInventory { get; set; }
        int SlotId { get; set; }
    }
    
    public interface IWithTarget : ISpellCast
    {
        OuterRef<IEntity> Target { get; set; }
    }
    
    public interface IWithDirection : ISpellCast
    {
        Utils.Vector3 Direction { get; set; }
    }
    
//    public interface IWithInputAction : ISpellCast
//    {
//        InputActionDef InputAction { get; set; }
//    }

    [ProtoContract]
    public class SpellCastWithDirection : SpellCastWithParameters, IWithDirection
    {
        [UsedImplicitly] public SpellCastWithDirection(){} 

        public SpellCastWithDirection(IEnumerable<SpellCastParameter> @params)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Append(new SpellCastParameterDirection()).ToArray();
        }
        [ProtoIgnore] public Vector3 Direction { get => this.GetParameter<SpellCastParameterDirection>().Direction; set => this.GetParameter<SpellCastParameterDirection>().Direction = value; }
        
        public override SpellCast Clone() => FillCloneInternal(new SpellCastWithDirection());
    }

    [ProtoContract]
    public class SpellCastWithSlotItem : SpellCastWithParameters, IWithSlotItem
    {
        [UsedImplicitly] public SpellCastWithSlotItem(){}

        public SpellCastWithSlotItem(IEnumerable<SpellCastParameter> @params)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Append(new SpellCastParameterSlotItem()).ToArray();
        }
        
        [ProtoIgnore] public bool IsInventory { get => this.GetParameter<SpellCastParameterSlotItem>().IsInventory; set => this.GetParameter<SpellCastParameterSlotItem>().IsInventory = value; }
        [ProtoIgnore] public int SlotId { get => this.GetParameter<SpellCastParameterSlotItem>().SlotId; set => this.GetParameter<SpellCastParameterSlotItem>().SlotId = value; }
        
        public override SpellCast Clone() => FillCloneInternal(new SpellCastWithSlotItem());
    }
    
    [ProtoContract]
    public class SpellCastWithTarget : SpellCastWithParameters, IWithTarget
    {
        [UsedImplicitly] public SpellCastWithTarget(){}

        public SpellCastWithTarget(IEnumerable<SpellCastParameter> @params)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Append(new SpellCastParameterTarget()).ToArray();
        }
        
        [ProtoIgnore] public OuterRef<IEntity> Target { get => this.GetParameter<SpellCastParameterTarget>().Target; set => this.GetParameter<SpellCastParameterTarget>().Target = value; }
        
        public override SpellCast Clone() => FillCloneInternal(new SpellCastWithTarget());
    }

    [ProtoContract]
    public class SpellCastWithTargetAndDirection : SpellCastWithParameters, IWithTarget, IWithDirection
    {
        [UsedImplicitly] public SpellCastWithTargetAndDirection() {}

        public SpellCastWithTargetAndDirection(IEnumerable<SpellCastParameter> @params)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Concat(new SpellCastParameter[] {new SpellCastParameterTarget(), new SpellCastParameterDirection()}).ToArray();
        }
        [ProtoIgnore] public OuterRef<IEntity> Target { get => this.GetParameter<SpellCastParameterTarget>().Target; set => this.GetParameter<SpellCastParameterTarget>().Target = value; }
        [ProtoIgnore] public Vector3 Direction { get => this.GetParameter<SpellCastParameterDirection>().Direction; set => this.GetParameter<SpellCastParameterDirection>().Direction = value; }
        
        public override SpellCast Clone() => FillCloneInternal(new SpellCastWithTargetAndDirection());
    }
    
    /*

    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public class SpellCastTemplate<TParam1> : SpellCastWithParameters where TParam1 : SpellCastParameter, new()
    {
        public SpellCastTemplate(IEnumerable<SpellCastParameter> @params = null)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Append(new TParam1()).ToArray();
        }
        
        [ProtoIgnore] public TParam1 Param1 => GetParameter<TParam1>();
    }

    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public class SpellCastTemplate<TParam1,TParam2> : SpellCastWithParameters where TParam1 : SpellCastParameter, new() where TParam2 : SpellCastParameter, new()
    {
        public SpellCastTemplate(IEnumerable<SpellCastParameter> @params = null)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Concat(new SpellCastParameter[] {new TParam1(), new TParam2()}).ToArray();
        }
        
        [ProtoIgnore] public TParam1 Param1 => GetParameter<TParam1>();
        [ProtoIgnore] public TParam2 Param2 => GetParameter<TParam2>();
    }

    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public class SpellCastTemplate<TParam1,TParam2,TParam3> : SpellCastWithParameters 
        where TParam1 : SpellCastParameter, new() where TParam2 : SpellCastParameter, new() where TParam3 : SpellCastParameter, new()
    {
        public SpellCastTemplate(IEnumerable<SpellCastParameter> @params = null)
        {
            Parameters = (@params ?? Enumerable.Empty<SpellCastParameter>()).Concat(new SpellCastParameter[] {new TParam1(), new TParam2(), new TParam3()}).ToArray();
        }
        
        [ProtoIgnore] public TParam1 Param1 => GetParameter<TParam1>();
        [ProtoIgnore] public TParam2 Param2 => GetParameter<TParam2>();
        [ProtoIgnore] public TParam3 Param3 => GetParameter<TParam3>();
    }
    
    [ProtoContract]
    public class SpellCastWithDirection : SpellCastTemplate<SpellCastParameterDirection>, IWithDirection
    {
        [UsedImplicitly] public SpellCastWithDirection() {}
        public SpellCastWithDirection(IEnumerable<SpellCastParameter> @params = null) : base(@params) {}
        [ProtoIgnore] public Vector3 Direction { get => Param1.Direction; set => Param1.Direction = value; }
    }

    [ProtoContract]
    public class SpellCastWithTargetPosition : SpellCastTemplate<SpellCastParameterPosition>, IWithDirection
    {
        [UsedImplicitly] public SpellCastWithTargetPosition() {}
        public SpellCastWithTargetPosition(IEnumerable<SpellCastParameter> @params = null) : base(@params) {}
        [ProtoIgnore] public Vector3 Direction { get => Param1.Position; set => Param1.Position = value; }
    }

    [ProtoContract]
    public class SpellCastWithSlotItem : SpellCastTemplate<SpellCastParameterSlotItem>, IWithSlotItem
    {
        [UsedImplicitly] public SpellCastWithSlotItem() {}
        public SpellCastWithSlotItem(IEnumerable<SpellCastParameter> @params = null) : base(@params) {}
        [ProtoIgnore] public bool IsInventory { get => Param1.IsInventory; set => Param1.IsInventory = value; }
        [ProtoIgnore] public int SlotId { get => Param1.SlotId; set => Param1.SlotId = value; }
    }

    [ProtoContract]
    public class SpellCastWithTarget : SpellCastTemplate<SpellCastParameterTarget>, IWithTarget
    {
        [UsedImplicitly] public SpellCastWithTarget() {}
        public SpellCastWithTarget(IEnumerable<SpellCastParameter> @params = null) : base(@params) {}

        [ProtoIgnore] public OuterRef<IEntity> Target { get => Param1.Target; set => Param1.Target = value; }
    }

    [ProtoContract]
    public class SpellCastWithTargetAndDirection : SpellCastTemplate<SpellCastParameterTarget, SpellCastParameterDirection>, IWithTarget, IWithDirection
    {
        [UsedImplicitly] public SpellCastWithTargetAndDirection() {}
        public SpellCastWithTargetAndDirection(IEnumerable<SpellCastParameter> @params = null) : base(@params) {}
        [ProtoIgnore] public OuterRef<IEntity> Target { get => Param1.Target; set => Param1.Target = value; }
        [ProtoIgnore] public Vector3 Direction { get => Param2.Direction; set => Param2.Direction = value; }
    }
*/
//    [ProtoContract]
//    public class SpellCastWithInputAction : SpellCastTemplate<SpellCastParameterInputAction>, IWithInputAction
//    {
//        [ProtoIgnore] public InputActionDef InputAction { get => Param1.InputAction; set => Param1.InputAction = value; }
//    }
//    
//    [ProtoContract]
//    public class SpellCastWithInputActionAndDirection :  SpellCastTemplate<SpellCastParameterInputAction, SpellCastParameterDirection>, IWithInputAction, IWithDirection
//    {
//        [ProtoIgnore] public InputActionDef InputAction { get => Param1.InputAction; set => Param1.InputAction = value; }
//        [ProtoIgnore] public Vector3 Direction { get => Param2.Direction; set => Param2.Direction = value; }
//    }
//    
//    [ProtoContract]
//    public class SpellCastWithInputActionAndTarget : SpellCastTemplate<SpellCastParameterInputAction, SpellCastParameterTarget>, IWithInputAction, IWithTarget
//    {
//        [ProtoIgnore] public InputActionDef InputAction { get => Param1.InputAction; set => Param1.InputAction = value; }
//        [ProtoIgnore] public OuterRef<IEntity> Target { get => Param2.Target; set => Param2.Target = value; }
//    }
//    
//    [ProtoContract]
//    public class SpellCastWithInputActionAndTargetAndDirection : SpellCastTemplate<SpellCastParameterInputAction, SpellCastParameterTarget, SpellCastParameterDirection>, IWithTarget, IWithInputAction, IWithDirection
//    {
//        [ProtoIgnore] public InputActionDef InputAction { get => Param1.InputAction; set => Param1.InputAction = value; }
//        [ProtoIgnore] public OuterRef<IEntity> Target { get => Param2.Target; set => Param2.Target = value; }
//        [ProtoIgnore] public Vector3 Direction { get => Param3.Direction; set => Param3.Direction = value; }
//    }
    
    #endregion
}
