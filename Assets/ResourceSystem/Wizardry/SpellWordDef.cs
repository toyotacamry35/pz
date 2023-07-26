using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using L10n;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace SharedCode.Wizardry
{
    [Localized]
    public abstract class SpellWordDef : BaseResource
    {
        public bool Enabled { get; set; } = true;

    }
    public abstract class SpellEffectDef : SpellWordDef
    {

    }

    public class SpellResourceDef : SpellWordDef
    {

    }

    public class SpellMarkerDef : SpellWordDef
    {

    }

    public class SpellContextValueDef : BaseResource
    {

    }

    public class SpellContextValueDef<T> : SpellContextValueDef
    {

    }

    public abstract class SpellEntityDef : SpellContextValueDef<OuterRef>
    {

    }
    public class SpellEventOfDef : SpellEntityDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; } = new SpellCasterDef();
    }

    public abstract class SpellVector2Def : SpellContextValueDef<Vector2>
    {
    }

    public class SpellDirection2Def : SpellVector2Def, ISpellParameterDef<Vector2>
    {
    }
    
    public class SpellPoint2Def : SpellVector2Def, ISpellParameterDef<Vector2>
    {
    }
        
    public class SpellExplicitVector2Def : SpellVector2Def
    {
        public float x, y;
    }
    
    public abstract class SpellVector3Def : SpellContextValueDef<Vector3>
    {

    }

    public class SpellDirectionDef : SpellVector3Def, ISpellParameterDef<Vector3>
    {

    }

    public class SpellTargetPointDef : SpellVector3Def, ISpellParameterDef<Vector3>
    {

    }

    public class SpellLocalPointDef : SpellVector3Def, ISpellParameterDef<Vector3>
    {

    }

    public class SpellExplicitVector3Def : SpellVector3Def
    {
        public float x, y, z;
    }
    
    public abstract class SpellQuaternionDef : SpellContextValueDef<Quaternion>
    {

    }

    public class SpellRotationDef : SpellQuaternionDef, ISpellParameterDef<Quaternion>
    {

    }

    public class SpellLocalRotationDef : SpellQuaternionDef, ISpellParameterDef<Quaternion>
    {

    }
    
    public class SpellTargetDef : SpellEntityDef, ISpellParameterDef<OuterRef>
    {

    }

    public class SpellSpawnerOfDef : SpellEntityDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; } = new SpellCasterDef();
    }
    public class SpellCasterDef : SpellEntityDef
    {

    }

    public interface ISpellEntityHasAuthorityDef
    {
        SpellEntityDef Entity { get; }
    }

    public class SpellEntityHasAuthorityDef : SpellContextValueDef, ISpellEntityHasAuthorityDef
    {
        public ResourceRef<SpellEntityDef> Entity { get; [UsedImplicitly] set; }
        SpellEntityDef ISpellEntityHasAuthorityDef.Entity => Entity;
    }
    
    public class SpellCasterHasAuthorityDef : SpellContextValueDef, ISpellEntityHasAuthorityDef
    {
        private static readonly SpellEntityDef _caster = new SpellCasterDef();
        public SpellEntityDef Entity => _caster;
    }

    public class SpellTargetHasAuthorityDef : SpellContextValueDef, ISpellEntityHasAuthorityDef
    {
        private static readonly SpellEntityDef _target = new SpellTargetDef();
        public SpellEntityDef Entity => _target;
    }

    public interface ISpellCalcerDef
    {
        CalcerDef Calcer { get; }
    }
    
    public class SpellCalcerDef<T> : SpellContextValueDef, ISpellCalcerDef
    {
        public ResourceRef<CalcerDef<T>> Calcer { get; [UsedImplicitly] set; }
        CalcerDef ISpellCalcerDef.Calcer => Calcer.Target;
    }
    
    public class SpellTargetLinksDef : SpellTargetDef
    {
        public ResourceRef<SpellEntityDef> From { get; set; }
        public ResourceRef<LinkTypeDef> LinkType { get; set; }
    }
    public abstract class SpellIdDef : SpellContextValueDef<long>
    {

    }

    public class PrevSpellIdDef : SpellIdDef, ISpellParameterDef<long>
    {

    }

    public class CurrentSpellIdDef : SpellIdDef
    {

    }
    
    public abstract class SpellStringDef : SpellContextValueDef<string>
    {
    }

    public class SpellObjectNameDef : SpellStringDef, ISpellParameterDef<string>
    {

    }
    
    public class SpellExplicitStringDef : SpellStringDef
    {
        public string Value;
    }
    
    public class SpellDamageTypeDef : SpellContextValueDef<BaseResource>, ISpellParameterDef<BaseResource>
    {

    }

    public class SpellWeaponSizeDef : SpellContextValueDef<BaseResource>, ISpellParameterDef<BaseResource>
    {

    }

    public class SpellAttackTypeDef : SpellContextValueDef<BaseResource>, ISpellParameterDef<BaseResource>
    {

    }

    public class SpellHitMaterialDef : SpellContextValueDef<BaseResource>, ISpellParameterDef<BaseResource>
    {

    }

    public class SpellDurationDef : SpellContextValueDef<float>, ISpellParameterDef<float>
    {

    }
    
    public class SpellWordRef : SpellWordDef
    {
        public ResourceRef<SpellWordDef> Word { get; set; }
    }

    public static class SpellWordExtensions
    {
        public static SpellWordDef ResolveWordRef(this SpellWordDef self)
        {
            return (self as SpellWordRef)?.Word.Target ?? self;
        }
    }
    
    public interface ISpellParameterDef : IResource {}
    
    public interface ISpellParameterDef<T> : ISpellParameterDef {}
}
