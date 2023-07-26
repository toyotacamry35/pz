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
    public partial class AccountCharacter
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)CharacterName;
                    return true;
                case 11:
                    property = (T)(object)MapId;
                    return true;
                case 12:
                    property = (T)(object)CurrentSessionRewards;
                    return true;
                case 13:
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
            if (childName == nameof(CharacterName))
                return 10;
            if (childName == nameof(MapId))
                return 11;
            if (childName == nameof(CurrentSessionRewards))
                return 12;
            if (childName == nameof(Id))
                return 13;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}