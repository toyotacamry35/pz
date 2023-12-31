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
    public partial class RealmsCollectionEntity
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Realms;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Realms))
                return 10;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmEntity
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Def;
                    return true;
                case 11:
                    property = (T)(object)StartTime;
                    return true;
                case 12:
                    property = (T)(object)Active;
                    return true;
                case 13:
                    property = (T)(object)AttachedAccounts;
                    return true;
                case 14:
                    property = (T)(object)Maps;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Def))
                return 10;
            if (childName == nameof(StartTime))
                return 11;
            if (childName == nameof(Active))
                return 12;
            if (childName == nameof(AttachedAccounts))
                return 13;
            if (childName == nameof(Maps))
                return 14;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}