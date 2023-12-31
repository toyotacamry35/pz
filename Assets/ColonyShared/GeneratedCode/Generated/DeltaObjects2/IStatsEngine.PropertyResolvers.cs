// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using SharedCode.Logging;
using System.Collections.Generic;
using System.Linq;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class StatsEngine
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)StatsDef;
                    return true;
                case 11:
                    property = (T)(object)TimeStats;
                    return true;
                case 12:
                    property = (T)(object)TimeStatsBroadcast;
                    return true;
                case 13:
                    property = (T)(object)ValueStats;
                    return true;
                case 14:
                    property = (T)(object)ValueStatsBroadcast;
                    return true;
                case 15:
                    property = (T)(object)ProxyStats;
                    return true;
                case 16:
                    property = (T)(object)ProceduralStats;
                    return true;
                case 17:
                    property = (T)(object)AccumulatedStats;
                    return true;
                case 18:
                    property = (T)(object)ProceduralStatsBroadcast;
                    return true;
                case 19:
                    property = (T)(object)AccumulatedStatsBroadcast;
                    return true;
                case 20:
                    property = (T)(object)TimeWhenIdleStarted;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(StatsDef))
                return 10;
            if (childName == nameof(TimeStats))
                return 11;
            if (childName == nameof(TimeStatsBroadcast))
                return 12;
            if (childName == nameof(ValueStats))
                return 13;
            if (childName == nameof(ValueStatsBroadcast))
                return 14;
            if (childName == nameof(ProxyStats))
                return 15;
            if (childName == nameof(ProceduralStats))
                return 16;
            if (childName == nameof(AccumulatedStats))
                return 17;
            if (childName == nameof(ProceduralStatsBroadcast))
                return 18;
            if (childName == nameof(AccumulatedStatsBroadcast))
                return 19;
            if (childName == nameof(TimeWhenIdleStarted))
                return 20;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}