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
    public partial class PositionedFenceElement
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)State;
                    return true;
                case 11:
                    property = (T)(object)Owner;
                    return true;
                case 12:
                    property = (T)(object)RecipeDef;
                    return true;
                case 13:
                    property = (T)(object)Position;
                    return true;
                case 14:
                    property = (T)(object)Rotation;
                    return true;
                case 15:
                    property = (T)(object)Depth;
                    return true;
                case 16:
                    property = (T)(object)BuildToken;
                    return true;
                case 17:
                    property = (T)(object)BuildTimestamp;
                    return true;
                case 18:
                    property = (T)(object)BuildTime;
                    return true;
                case 19:
                    property = (T)(object)Health;
                    return true;
                case 20:
                    property = (T)(object)Interaction;
                    return true;
                case 21:
                    property = (T)(object)Visual;
                    return true;
                case 22:
                    property = (T)(object)Id;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(State))
                return 10;
            if (childName == nameof(Owner))
                return 11;
            if (childName == nameof(RecipeDef))
                return 12;
            if (childName == nameof(Position))
                return 13;
            if (childName == nameof(Rotation))
                return 14;
            if (childName == nameof(Depth))
                return 15;
            if (childName == nameof(BuildToken))
                return 16;
            if (childName == nameof(BuildTimestamp))
                return 17;
            if (childName == nameof(BuildTime))
                return 18;
            if (childName == nameof(Health))
                return 19;
            if (childName == nameof(Interaction))
                return 20;
            if (childName == nameof(Visual))
                return 21;
            if (childName == nameof(Id))
                return 22;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}