using System;
using System.Threading.Tasks;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using GeneratorAnnotations;

namespace SharedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface IContainer: IItemsContainer, IDeltaObject
    {
    }

    [GenerateDeltaObjectCode]
    public interface IBuildingContainer : IItemsContainer, IDeltaObject
    {
    }

    [GenerateDeltaObjectCode]
    public interface IMachineOutputContainer : IItemsContainer, IDeltaObject
    {
    }

    [GenerateDeltaObjectCode]
    public interface IMachineFuelContainer : IItemsContainer, IDeltaObject
    {
    }
}
