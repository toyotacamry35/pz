using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Aspects.Impl.Traumas.Template
{
    [Localized]
    public abstract class TraumaDef : SaveableBaseResource
    {
        //        public bool TriggerOnly { get; set; } странная мутная хрень, типа добавлять травму не запуская её спелл. на данный момент ни в каких травмах не используется 
//        public UnityRef<Sprite> Icon { get; set; }
        public int TraumaPoints { get; set; } = 0;
//        public LocalizedString DescriptionLs { get; set; }
        public ResourceRef<PredicateDef> EndPredicate { get; set; }
        
        public bool IgnoresDeath;
    }


    /// <summary>
    /// Травмы активируемые предикатами.
    /// Травмы этого типа НЕ сохраняются в БД, и активируются заново при загрузке персонажа, если удовлетворяется их предикат.
    /// </summary>
    [Localized] 
    public class TraumaGiverDef : TraumaDef
    {
      /*  [JsonProperty(Required = Required.Always)] */ public ResourceRef<PredicateDef> Predicate { get; set; }
        public ResourceRef<SpellDef> DebuffSpellRef { get; set; }

        public override string ToString()
        {
            return $"{nameof(TraumaGiverDef)}[TP={TraumaPoints}, Spell:{DebuffSpellRef.Target?.____GetDebugAddress()}]";
        }
    }
    
    /// <summary>
    /// Травмы активируемые вручную из спеллов.
    /// Травмы этого типа сохраняются в БД и восстанавливаются на персонаже после его загрузки.
    /// Вместо спеллов используют баффы.
    /// </summary>
    [Localized] 
    public class SaveableTraumaDef : TraumaDef
    {
        /// <summary>
        /// Время жизни этого баффа равно времени жизни травмы.
        /// Баф сохраняется в базе, и продолжает работу после загрузки персонажа из базы.
        /// </summary>
        public ResourceRef<BuffDef> Buff { get; set; }

        /// <summary>
        /// Спелл вызываемый только один раз при активации травмы.
        /// Если спелл зафейлился при старте, то травма НЕ активируется
        /// В дальнейшем, время жизни этого спелла НЕ зависит от времени жизни травмы, и наоборот.
        /// Спелл НЕ перезапускается при загрузке из базы. 
        /// </summary>
        public ResourceRef<SpellDef> SpellOnStart { get; set; } 
        public override string ToString()
        {
            return $"{nameof(TraumaGiverDef)}[TP={TraumaPoints}, Buff:{Buff.Target?.____GetDebugAddress()} SpellOnStart:{SpellOnStart.Target?.____GetDebugAddress()}]";
        }
    }

    public static class TraumaDefExtensions
    {
        public static  Sprite Icon(this TraumaDef trauma)
        {
            switch (trauma)
            {
                case SaveableTraumaDef saveableTraumaDef:
                    return saveableTraumaDef.Buff.Target?.Icon?.Target;
                case TraumaGiverDef traumaGiverDef:
                    return traumaGiverDef.DebuffSpellRef.Target?.Icon?.Target;
                case null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trauma));
            }
        }
        
        public static LocalizedString Description(this TraumaDef trauma)
        {
            switch (trauma)
            {
                case SaveableTraumaDef saveableTraumaDef:
                    return saveableTraumaDef.Buff.Target?.DescriptionLs ?? LocalizedString.Empty;
                case TraumaGiverDef traumaGiverDef:
                    return traumaGiverDef.DebuffSpellRef.Target?.DescriptionLs ?? LocalizedString.Empty;
                case null:
                    return LocalizedString.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trauma));
            }
        }
    }
}
