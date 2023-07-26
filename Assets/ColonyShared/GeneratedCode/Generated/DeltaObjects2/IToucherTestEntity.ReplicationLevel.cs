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
    public class ToucherTestEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityAlways
    {
        public ToucherTestEntityAlways(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 359730857;
    }

    public class ToucherTestEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityClientBroadcast
    {
        public ToucherTestEntityClientBroadcast(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast DeltaObjProperty => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast)__deltaObject__.DeltaObjProperty?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast> __dict__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast> dict
        {
            get
            {
                if (__dict__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__dict__Wrapper__).GetBaseDeltaObject() != __deltaObject__.dict)
                    __dict__Wrapper__ = __deltaObject__.dict == null ? null : new DeltaDictionaryWrapper<int, GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast>(__deltaObject__.dict);
                return __dict__Wrapper__;
            }
        }

        public IDeltaDictionary<int, decimal> flatDict
        {
            get
            {
                return __deltaObject__.flatDict;
            }
        }

        public IDeltaList<decimal> flatList => __deltaObject__.flatList;
        IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast> __listOfChildren__Wrapper__;
        public IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast> listOfChildren
        {
            get
            {
                if (__listOfChildren__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__listOfChildren__Wrapper__).GetBaseDeltaObject() != __deltaObject__.listOfChildren)
                    __listOfChildren__Wrapper__ = __deltaObject__.listOfChildren == null ? null : new DeltaListWrapper<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast>(__deltaObject__.listOfChildren);
                return __listOfChildren__Wrapper__;
            }
        }

        public int IntProperty => __deltaObject__.IntProperty;
        public System.Threading.Tasks.Task ChangeDeltaObjProperty(int childsIntProperty)
        {
            return __deltaObject__.ChangeDeltaObjProperty(childsIntProperty);
        }

        public System.Threading.Tasks.Task AddDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.AddDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.SetDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveDictionaryChild(int key)
        {
            return __deltaObject__.RemoveDictionaryChild(key);
        }

        public System.Threading.Tasks.Task ClearDictionaryChild()
        {
            return __deltaObject__.ClearDictionaryChild();
        }

        public System.Threading.Tasks.Task AddFlatDictionary(int key, decimal value)
        {
            return __deltaObject__.AddFlatDictionary(key, value);
        }

        public System.Threading.Tasks.Task RemoveFlatDictionary(int key)
        {
            return __deltaObject__.RemoveFlatDictionary(key);
        }

        public System.Threading.Tasks.Task InsertFlatList(int index, decimal value)
        {
            return __deltaObject__.InsertFlatList(index, value);
        }

        public System.Threading.Tasks.Task SetFlatList(int index, decimal value)
        {
            return __deltaObject__.SetFlatList(index, value);
        }

        public System.Threading.Tasks.Task RemoveFlatList(int index)
        {
            return __deltaObject__.RemoveFlatList(index);
        }

        public System.Threading.Tasks.Task InsertListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.InsertListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.SetListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveListOfChildren(int index)
        {
            return __deltaObject__.RemoveListOfChildren(index);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientBroadcast child)
        {
            return __deltaObject__.SetListOfChildren(index, child.To<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject>());
        }

        public System.Threading.Tasks.Task SetIntProperty(int i)
        {
            return __deltaObject__.SetIntProperty(i);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = DeltaObjProperty;
                    break;
                case 11:
                    currProperty = dict;
                    break;
                case 12:
                    currProperty = flatDict;
                    break;
                case 13:
                    currProperty = flatList;
                    break;
                case 14:
                    currProperty = listOfChildren;
                    break;
                case 15:
                    currProperty = IntProperty;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -191752356;
    }

    public class ToucherTestEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityClientFullApi
    {
        public ToucherTestEntityClientFullApi(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1708599344;
    }

    public class ToucherTestEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityClientFull
    {
        public ToucherTestEntityClientFull(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull DeltaObjProperty => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull)__deltaObject__.DeltaObjProperty?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull> __dict__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull> dict
        {
            get
            {
                if (__dict__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__dict__Wrapper__).GetBaseDeltaObject() != __deltaObject__.dict)
                    __dict__Wrapper__ = __deltaObject__.dict == null ? null : new DeltaDictionaryWrapper<int, GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull>(__deltaObject__.dict);
                return __dict__Wrapper__;
            }
        }

        public IDeltaDictionary<int, decimal> flatDict
        {
            get
            {
                return __deltaObject__.flatDict;
            }
        }

        public IDeltaList<decimal> flatList => __deltaObject__.flatList;
        IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull> __listOfChildren__Wrapper__;
        public IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull> listOfChildren
        {
            get
            {
                if (__listOfChildren__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__listOfChildren__Wrapper__).GetBaseDeltaObject() != __deltaObject__.listOfChildren)
                    __listOfChildren__Wrapper__ = __deltaObject__.listOfChildren == null ? null : new DeltaListWrapper<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull>(__deltaObject__.listOfChildren);
                return __listOfChildren__Wrapper__;
            }
        }

        public int IntProperty => __deltaObject__.IntProperty;
        public System.Threading.Tasks.Task ChangeDeltaObjProperty(int childsIntProperty)
        {
            return __deltaObject__.ChangeDeltaObjProperty(childsIntProperty);
        }

        public System.Threading.Tasks.Task AddDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.AddDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.SetDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveDictionaryChild(int key)
        {
            return __deltaObject__.RemoveDictionaryChild(key);
        }

        public System.Threading.Tasks.Task ClearDictionaryChild()
        {
            return __deltaObject__.ClearDictionaryChild();
        }

        public System.Threading.Tasks.Task AddFlatDictionary(int key, decimal value)
        {
            return __deltaObject__.AddFlatDictionary(key, value);
        }

        public System.Threading.Tasks.Task RemoveFlatDictionary(int key)
        {
            return __deltaObject__.RemoveFlatDictionary(key);
        }

        public System.Threading.Tasks.Task InsertFlatList(int index, decimal value)
        {
            return __deltaObject__.InsertFlatList(index, value);
        }

        public System.Threading.Tasks.Task SetFlatList(int index, decimal value)
        {
            return __deltaObject__.SetFlatList(index, value);
        }

        public System.Threading.Tasks.Task RemoveFlatList(int index)
        {
            return __deltaObject__.RemoveFlatList(index);
        }

        public System.Threading.Tasks.Task InsertListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.InsertListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.SetListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveListOfChildren(int index)
        {
            return __deltaObject__.RemoveListOfChildren(index);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectClientFull child)
        {
            return __deltaObject__.SetListOfChildren(index, child.To<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject>());
        }

        public System.Threading.Tasks.Task SetIntProperty(int i)
        {
            return __deltaObject__.SetIntProperty(i);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = DeltaObjProperty;
                    break;
                case 11:
                    currProperty = dict;
                    break;
                case 12:
                    currProperty = flatDict;
                    break;
                case 13:
                    currProperty = flatList;
                    break;
                case 14:
                    currProperty = listOfChildren;
                    break;
                case 15:
                    currProperty = IntProperty;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -164559541;
    }

    public class ToucherTestEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityServerApi
    {
        public ToucherTestEntityServerApi(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 442747700;
    }

    public class ToucherTestEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestEntityServer
    {
        public ToucherTestEntityServer(GeneratedCode.EntityModel.Test.IToucherTestEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.EntityModel.Test.IToucherTestEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.EntityModel.Test.IToucherTestEntity)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer DeltaObjProperty => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer)__deltaObject__.DeltaObjProperty?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer> __dict__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer> dict
        {
            get
            {
                if (__dict__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__dict__Wrapper__).GetBaseDeltaObject() != __deltaObject__.dict)
                    __dict__Wrapper__ = __deltaObject__.dict == null ? null : new DeltaDictionaryWrapper<int, GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer>(__deltaObject__.dict);
                return __dict__Wrapper__;
            }
        }

        public IDeltaDictionary<int, decimal> flatDict
        {
            get
            {
                return __deltaObject__.flatDict;
            }
        }

        public IDeltaList<decimal> flatList => __deltaObject__.flatList;
        IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer> __listOfChildren__Wrapper__;
        public IDeltaListWrapper<GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer> listOfChildren
        {
            get
            {
                if (__listOfChildren__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__listOfChildren__Wrapper__).GetBaseDeltaObject() != __deltaObject__.listOfChildren)
                    __listOfChildren__Wrapper__ = __deltaObject__.listOfChildren == null ? null : new DeltaListWrapper<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer>(__deltaObject__.listOfChildren);
                return __listOfChildren__Wrapper__;
            }
        }

        public int IntProperty => __deltaObject__.IntProperty;
        public System.Threading.Tasks.Task ChangeDeltaObjProperty(int childsIntProperty)
        {
            return __deltaObject__.ChangeDeltaObjProperty(childsIntProperty);
        }

        public System.Threading.Tasks.Task AddDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.AddDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetDictionaryChild(int key, int childsIntProperty)
        {
            return __deltaObject__.SetDictionaryChild(key, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveDictionaryChild(int key)
        {
            return __deltaObject__.RemoveDictionaryChild(key);
        }

        public System.Threading.Tasks.Task ClearDictionaryChild()
        {
            return __deltaObject__.ClearDictionaryChild();
        }

        public System.Threading.Tasks.Task AddFlatDictionary(int key, decimal value)
        {
            return __deltaObject__.AddFlatDictionary(key, value);
        }

        public System.Threading.Tasks.Task RemoveFlatDictionary(int key)
        {
            return __deltaObject__.RemoveFlatDictionary(key);
        }

        public System.Threading.Tasks.Task InsertFlatList(int index, decimal value)
        {
            return __deltaObject__.InsertFlatList(index, value);
        }

        public System.Threading.Tasks.Task SetFlatList(int index, decimal value)
        {
            return __deltaObject__.SetFlatList(index, value);
        }

        public System.Threading.Tasks.Task RemoveFlatList(int index)
        {
            return __deltaObject__.RemoveFlatList(index);
        }

        public System.Threading.Tasks.Task InsertListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.InsertListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, int childsIntProperty)
        {
            return __deltaObject__.SetListOfChildren(index, childsIntProperty);
        }

        public System.Threading.Tasks.Task RemoveListOfChildren(int index)
        {
            return __deltaObject__.RemoveListOfChildren(index);
        }

        public System.Threading.Tasks.Task SetListOfChildren(int index, GeneratedCode.DeltaObjects.ReplicationInterfaces.IToucherTestDeltaObjectServer child)
        {
            return __deltaObject__.SetListOfChildren(index, child.To<GeneratedCode.EntityModel.Test.IToucherTestDeltaObject>());
        }

        public System.Threading.Tasks.Task SetIntProperty(int i)
        {
            return __deltaObject__.SetIntProperty(i);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = DeltaObjProperty;
                    break;
                case 11:
                    currProperty = dict;
                    break;
                case 12:
                    currProperty = flatDict;
                    break;
                case 13:
                    currProperty = flatList;
                    break;
                case 14:
                    currProperty = listOfChildren;
                    break;
                case 15:
                    currProperty = IntProperty;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 362036424;
    }
}