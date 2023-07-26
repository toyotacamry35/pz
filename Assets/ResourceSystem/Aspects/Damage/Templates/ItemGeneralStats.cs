using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    
    public struct StatModifier
    {        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
         private float _value;
        public float Value { get {return _value;} set {_value = value; updateHashCode();} }
         private ResourceRef<StatResource> _stat;
         public ResourceRef<StatResource> Stat { get { return _stat; } 
            set 
            {
                if(value == null) 
                {                
                    Logger.IfError()?.Message("StatModifier Stat is null").Write();
                    throw new Exception("StatModifierData Stat is null");
                }
                _stat = value; 
                updateHashCode();
            } 
        }

        private int _hashCode;

        public StatModifier(StatResource stat, float value)
        {
            _stat = stat;
            _value = value;
            _hashCode = 0;
            updateHashCode();            
        }

        private void updateHashCode()
        {
            _hashCode = -537877839;
            _hashCode = _hashCode * -1521134295 + Value.GetHashCode();
            _hashCode = _hashCode * -1521134295 + EqualityComparer<StatResource>.Default.GetHashCode(Stat);            
        }

        public override bool Equals(object other)
        {
            if (!(other is StatModifier))
                return false;

            var asStatModifier = (StatModifier)other;
            return Stat == asStatModifier.Stat;
        }

        public static bool Equals(StatModifier[] one, StatModifier[] other)
        {
            if (one == null && other == null) //Считаем пустые одинаковыми
                return true;

            if (one != null && other != null)
                return one.SequenceEqual(other);

            return false;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Stat.Target.DebugName} = {Value}";
        }

        public static string ToString(IEnumerable<StatModifier> modifiers)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var modifier in modifiers)
            {
                sb.Append(modifier + "\n");
            }

            return sb.ToString();
        }

        public static string ToString(Dictionary<StatResource, float> modifiers)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var modifier in modifiers)
            {
                sb.Append(modifier.Key.DebugName + " = " + modifier.Value + "\n");
            }

            return sb.ToString();
        }
    }

    public class ItemGeneralStats : BaseResource
    {
        public StatModifier[] Stats { get; set; }
    }

    [ProtoContract]
    public struct StatModifierProto
    {
        [ProtoMember(1)]
        public float Value { get; set; }
        [ProtoMember(2)]
        public StatResource Stat { get; set; }
    }
}