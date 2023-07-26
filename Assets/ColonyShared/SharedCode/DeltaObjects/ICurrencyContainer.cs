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
    public interface ICurrencyContainer : IItemsContainer, IDeltaObject
    {
    }
}
