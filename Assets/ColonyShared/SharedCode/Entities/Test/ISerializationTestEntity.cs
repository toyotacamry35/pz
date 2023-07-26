using System;
using System.Threading.Tasks;
using GeneratedCode.Repositories;
using GeneratorAnnotations;
using Nest;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;

namespace Assets.ColonyShared.SharedCode.Entities.Test
{
    [GenerateDeltaObjectCode]
    public interface ISerializationTestEntity : IEntity
    {   
        [ReplicationLevel(ReplicationLevel.Server)] 
        int Value { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetValue(int value);

        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject2 TestProperty { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ITestDeltaObject2 TestPropertyClientFull { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task FillTestProperty(Guid id);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveTestProperty();

        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject2 TestProperty2 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task FillTestProperty2();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetTestProperty2WithNewDletaObject();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task FillTestProperty2FromTestProperty();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveTestProperty2();
        
        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject3 TestProperty3 { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddToList(ITestDeltaObject2 element);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddToDictionary(int key, ITestDeltaObject2 element);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetList(IDeltaList<ITestDeltaObject2> list);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveFromList(ITestDeltaObject2 element);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveFromDictionary(int key);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<ITestDeltaObject2> List1 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<int, ITestDeltaObject2> Dictionary1 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<int> SimpleList1 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<int, int> SimpleDictionary1 { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<int> SetOnDestroy(int newValue);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        EntityRef<ISerializationTestEntity2> LinkedEntityServer { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        EntityRef<ISerializationTestEntity2> LinkedEntityClientFull { get; set; }
    }
    
    [GenerateDeltaObjectCode]
    public interface ISerializationTestEntity2 : IEntity, IHasOwner
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject2 TestProperty { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetValue(int value);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        int Value { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        EntityRef<ISerializationTestEntity3> LinkedEntityServer { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        EntityRef<ISerializationTestEntity3> LinkedEntityClientFull { get; set; }
    }
    
    [GenerateDeltaObjectCode]
    public interface ISerializationTestEntity3 : IEntity, IHasOwner
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject2 TestProperty { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface ITestDeltaObject2 : IDeltaObject
    {
         [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject3 Test { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ITestDeltaObject3 TestClientFull { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject3 Test2 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        EntityRef<ISerializationTestEntity2> EntityTestServer { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        EntityRef<ISerializationTestEntity2> EntityTestClientFull { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<int, EntityRef<ISerializationTestEntity2>> EntityDictionaryTest { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<EntityRef<ISerializationTestEntity2>> EntityListTest { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddToList(ITestDeltaObject3 element);
        
        [ReplicationLevel(ReplicationLevel.Server)]
         Task AddToList();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveFromList(ITestDeltaObject3 element);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddToDictionary(int key, ITestDeltaObject3 element);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task AddToDictionary(int key);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task RemoveFromDictionary(int key);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<ITestDeltaObject3> List1 { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<int, ITestDeltaObject3> Dictionary1 { get; set; }
        
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int ClientBroadcastValue { get; set; }

        // [ReplicationLevel(ReplicationLevel.Server)]
        // ITestDeltaObject4 CycleRefTest { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface ITestDeltaObject3 : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        int Test { get; set; }
    }
    
    // [GenerateDeltaObjectCode]
    // public interface ITestDeltaObject4 : IDeltaObject
    // {
    //     [ReplicationLevel(ReplicationLevel.Server)]
    //     ITestDeltaObject2 Test { get; set; }
    // }

    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface ISaveToDbEntityTest : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        ITestDeltaObject2 TestProperty { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetTestPropertyValue(int value);
    }
}