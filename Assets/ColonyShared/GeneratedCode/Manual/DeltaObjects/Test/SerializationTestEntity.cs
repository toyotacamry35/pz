using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Test;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public partial class SerializationTestEntity : IHookOnDestroy
    {
        private Action _onDestroy;

        public async Task FillTestPropertyImpl(Guid id)
        {
            var entity = await EntitiesRepository.Create<ISerializationTestEntity2>(id);
            var deltaObject = new TestDeltaObject2
            {
                EntityTestServer = entity
            };
            TestProperty = deltaObject;
        }

        public Task FillTestProperty2FromTestPropertyImpl()
        {
            TestProperty2 = TestProperty;
            return Task.CompletedTask;
        }

        public Task SetTestProperty2WithNewDletaObjectImpl()
        {
            TestProperty2=new TestDeltaObject2();
            return Task.CompletedTask;
        }

        public Task RemoveTestPropertyImpl()
        {
            TestProperty = null;
            return Task.CompletedTask;
        }
        
        public async Task FillTestProperty2Impl()
        {
            var deltaObject = new TestDeltaObject2
            {
                EntityTestServer = TestProperty.EntityTestServer
            };
            TestProperty2 = deltaObject;
        }

        public Task RemoveTestProperty2Impl()
        {
            TestProperty2 = null;
            return Task.CompletedTask;
        }
        
        public Task SetValueImpl(int value)
        {
             Value = value;
             return Task.CompletedTask;
        }

       public Task AddToListImpl(ITestDeltaObject2 element)
       {
           List1.Add(element);
           return Task.CompletedTask;
       }

       public Task SetListImpl(IDeltaList<ITestDeltaObject2> list)
       {
           List1 = list;
           return Task.CompletedTask;
       }

       public Task RemoveFromDictionaryImpl(int key)
       {
           Dictionary1.Remove(key);
           return Task.CompletedTask;
       }

       public Task RemoveFromListImpl(ITestDeltaObject2 element)
       {
           List1.Remove(element);
           return Task.CompletedTask;
       }

       public Task AddToDictionaryImpl(int key, ITestDeltaObject2 element)
       {
           Dictionary1.Add(key, element);
           return Task.CompletedTask;
       }

       public Task<int> SetOnDestroyImpl(int newValue)
       {
           _onDestroy = () => Value = 10;
           return Task.FromResult(Value);
       }

       public Task OnDestroy()
       {
           _onDestroy?.Invoke();
           return Task.CompletedTask;
       }
    }
    
    public partial class TestDeltaObject2
    {
        public Task AddToListImpl(ITestDeltaObject3 element)
        {
            List1.Add(element);
            return Task.CompletedTask;
        }

        public Task AddToDictionaryImpl(int key,ITestDeltaObject3 element)
        {
            Dictionary1.Add(key, element);
            return Task.CompletedTask;
        }
        
        public Task RemoveFromDictionaryImpl(int key)
        {
            Dictionary1.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveFromListImpl(ITestDeltaObject3 element)
        {
            List1.Remove(element);
            return Task.CompletedTask;
        }

        public Task AddToListImpl()
        {
            List1.Add(new TestDeltaObject3());
            return Task.CompletedTask;
        }

        public Task AddToDictionaryImpl(int key)
        {
            Dictionary1.Add(key, new TestDeltaObject3());
            return Task.CompletedTask;
        }
    }
    
    public partial class SerializationTestEntity2
    {
        public Task SetValueImpl(int value)
        {
            Value = value;
            return Task.CompletedTask;
        }
    }
}