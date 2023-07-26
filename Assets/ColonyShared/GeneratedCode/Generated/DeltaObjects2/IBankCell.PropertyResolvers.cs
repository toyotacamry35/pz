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
    public partial class BankCell
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)OwnerInformation;
                    return true;
                case 11:
                    property = (T)(object)Inventory;
                    return true;
                case 12:
                    property = (T)(object)OpenMechanics;
                    return true;
                case 13:
                    property = (T)(object)ContainerApi;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(OwnerInformation))
                return 10;
            if (childName == nameof(Inventory))
                return 11;
            if (childName == nameof(OpenMechanics))
                return 12;
            if (childName == nameof(ContainerApi))
                return 13;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}