using GeneratedCode.EntityModel.Test;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class ToucherTestEntity
    {
        public Task ChangeDeltaObjPropertyImpl(int childsIntProperty)
        {
            DeltaObjProperty = new ToucherTestDeltaObject() { IntProperty = childsIntProperty };
            return Task.CompletedTask;
        }
        public Task SetIntPropertyImpl(int i)
        {
            IntProperty = i;
            return Task.CompletedTask;
        }

        public Task AddDictionaryChildImpl(int key, int childsIntProperty = default)
        {
            dict.Add(key, new ToucherTestDeltaObject() { IntProperty = childsIntProperty });
            return Task.CompletedTask;
        }
        public Task SetDictionaryChildImpl(int key, int childsIntProperty = default)
        {
            dict[key] = new ToucherTestDeltaObject() { IntProperty = childsIntProperty };
            return Task.CompletedTask;
        }
        public Task RemoveDictionaryChildImpl(int key)
        {
            dict.Remove(key);
            return Task.CompletedTask;
        }
        public Task ClearDictionaryChildImpl()
        {
            dict.Clear();
            return Task.CompletedTask;
        }

        public Task AddFlatDictionaryImpl(int key, decimal value)
        {
            flatDict.Add(key, value);
            return Task.CompletedTask;
        }
        public Task RemoveFlatDictionaryImpl(int key)
        {
            flatDict.Remove(key);
            return Task.CompletedTask;
        }
        // FlatList
        public Task InsertFlatListImpl(int index, decimal value) {
            flatList.Insert(index, value);
            return Task.CompletedTask;
        }
        public Task SetFlatListImpl(int index, decimal value) {
            flatList[index] = value;
            return Task.CompletedTask;
        }
        public Task RemoveFlatListImpl(int index) {
            flatList.RemoveAt(index);
            return Task.CompletedTask;
        }
        // ListOfChildren
        public Task InsertListOfChildrenImpl(int index, int childsIntProperty = default) {
            listOfChildren.Insert(index, new ToucherTestDeltaObject() { IntProperty = childsIntProperty });
            return Task.CompletedTask;
        }
        public Task SetListOfChildrenImpl(int index, int childsIntProperty = default) {
            listOfChildren[index] = new ToucherTestDeltaObject() { IntProperty = childsIntProperty };
            return Task.CompletedTask;
        }
        public Task RemoveListOfChildrenImpl(int index) {
            listOfChildren.RemoveAt(index);
            return Task.CompletedTask;
        }
        public Task SetListOfChildrenImpl(int index, IToucherTestDeltaObject child) {
            listOfChildren[index] = child;
            return Task.CompletedTask;
        }
    }
}