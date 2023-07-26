using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.EntityModel.Test
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IToucherTestEntity : IEntity, IToucherTestBaseEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IToucherTestDeltaObject DeltaObjProperty { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task ChangeDeltaObjProperty(int childsIntProperty);

        //********** Dictionary of DeltaObjects **********/
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, IToucherTestDeltaObject> dict { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AddDictionaryChild(int key, int childsIntProperty = default);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetDictionaryChild(int key, int childsIntProperty = default);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task RemoveDictionaryChild(int key);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task ClearDictionaryChild();
        
        //********** Flat Dictionary **********/
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, decimal> flatDict { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task AddFlatDictionary(int key, decimal value);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task RemoveFlatDictionary(int key);

        //********** Flat List **********/
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<decimal> flatList { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task InsertFlatList(int index, decimal value);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetFlatList(int index, decimal value);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task RemoveFlatList(int index);

        //********** List of DeltaObjects **********/
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaList<IToucherTestDeltaObject> listOfChildren { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task InsertListOfChildren(int index, int childsIntProperty = default);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetListOfChildren(int index, int childsIntProperty = default);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task RemoveListOfChildren(int index);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task SetListOfChildren(int index, IToucherTestDeltaObject child);
    }
}