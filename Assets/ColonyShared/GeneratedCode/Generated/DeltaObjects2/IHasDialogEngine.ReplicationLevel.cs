// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class DialogEngineAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineAlways
    {
        public DialogEngineAlways(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1798189559;
    }

    public class DialogEngineClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineClientBroadcast
    {
        public DialogEngineClientBroadcast(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -337589074;
    }

    public class DialogEngineClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineClientFullApi
    {
        public DialogEngineClientFullApi(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -2133005774;
    }

    public class DialogEngineClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineClientFull
    {
        public DialogEngineClientFull(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Aspects.Dialog.DialogDef CurrentNode => __deltaObject__.CurrentNode;
        public System.Threading.Tasks.Task<ResourceSystem.Aspects.Dialog.DialogDef> Next(ResourceSystem.Aspects.Dialog.DialogDef nextDialog)
        {
            return __deltaObject__.Next(nextDialog);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CurrentNode;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1906704065;
    }

    public class DialogEngineServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineServerApi
    {
        public DialogEngineServerApi(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1903466411;
    }

    public class DialogEngineServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IDialogEngineServer
    {
        public DialogEngineServer(SharedCode.Entities.IDialogEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IDialogEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IDialogEngine)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Aspects.Dialog.DialogDef CurrentNode => __deltaObject__.CurrentNode;
        public System.Threading.Tasks.Task<ResourceSystem.Aspects.Dialog.DialogDef> Next(ResourceSystem.Aspects.Dialog.DialogDef nextDialog)
        {
            return __deltaObject__.Next(nextDialog);
        }

        public System.Threading.Tasks.Task<ResourceSystem.Aspects.Dialog.DialogDef> NextWithCheck(ResourceSystem.Aspects.Dialog.DialogDef nextDialog)
        {
            return __deltaObject__.NextWithCheck(nextDialog);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = CurrentNode;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -108816750;
    }
}