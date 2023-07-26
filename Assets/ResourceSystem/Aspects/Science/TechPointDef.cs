using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using EnumerableExtensions;
using ProtoBuf;
using SharedCode.Aspects.Item.Templates;

namespace SharedCode.Aspects.Science
{
    /// <summary>
    /// Затраты в одной валюте
    /// </summary>
    public struct TechPointCountDef : ITechPointRewardSource, IEquatable<TechPointCountDef>
    {
        public ResourceRef<CurrencyResource> TechPoint { get; set; }
        /// <summary>
        /// Отрицательное значение - затраты, положительное - прибыль
        /// </summary>
        public int Count { get; set; }

        CurrencyResource ITechPointRewardSource.TechPoint => TechPoint.Target;

        public override string ToString()
        {
            return $"{TechPoint.Target}={Count}";
        }

        public bool Equals(TechPointCountDef other)
        {
            return TechPoint.Equals(other.TechPoint) && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is TechPointCountDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TechPoint.GetHashCode() * 397) ^ Count;
            }
        }
    }

    [ProtoContract]
    public class TechPointCount
    {
        [ProtoMember(1)]
        public CurrencyResource TechPoint { get; set; }
        [ProtoMember(2)]
        public int Count { get; set; }
    }

    /// <summary>
    /// Затраты в нескольких валютах (цена)
    /// </summary>
    public struct PriceDef : IEquatable<PriceDef>
    {
        /// <summary>
        /// Отрицательное значение Count - затраты, положительное - прибыль
        /// </summary>
        public TechPointCountDef[] TechPointCosts; 

        public override string ToString()
        {
            return TechPointCosts == null ? "" : $"Costs: {TechPointCosts.ItemsToString()}";
        }

        public PriceDef GetPriceWithMultiplier(int multiplier)
        {
            if (TechPointCosts == null || TechPointCosts.Length == 0)
                return default;

            var newTechPointCosts = TechPointCosts
                .Select(price => new TechPointCountDef() {TechPoint = price.TechPoint, Count = multiplier * price.Count})
                .ToArray();

            return new PriceDef(){ TechPointCosts = newTechPointCosts};
        }

        public bool Equals(PriceDef other)
        {
            return Equals(TechPointCosts, other.TechPointCosts);
        }

        public override bool Equals(object obj)
        {
            return obj is PriceDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (TechPointCosts != null ? TechPointCosts.GetHashCode() : 0);
        }
    }

    /// <summary>
    /// Цена за некое действие с ItemResource
    /// </summary>
    public struct PerkActionPriceDef : IPerkActionPrice
    {
        public ResourceRef<CalcerDef> AmountOfResourcesMultiplier { get; set; }
        public ResourceRef<BaseItemResource> Item;
        public PriceDef Price { get; set; }
    }

    /// <summary>
    /// Цена за некое действие с определенным типом перка
    /// </summary>
    public struct PerkTypeActionPriceDef : IPerkActionPrice
    {
        public ResourceRef<CalcerDef> AmountOfResourcesMultiplier { get; set; }
        public ResourceRef<ItemTypeResource> PerkType;
        public PriceDef Price { get; set; }
    }

    public interface IPerkActionPrice
    {
        ResourceRef<CalcerDef> AmountOfResourcesMultiplier { get; set; }
        PriceDef Price { get; set; }
    }
}