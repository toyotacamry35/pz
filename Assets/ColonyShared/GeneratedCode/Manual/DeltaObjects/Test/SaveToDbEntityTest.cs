using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Test;

namespace GeneratedCode.DeltaObjects
{
    public partial class SaveToDbEntityTest
    {
        public Task SetTestPropertyValueImpl(int value)
        {
            TestProperty.Test.Test = value;
            return Task.CompletedTask;
        }
    }
}