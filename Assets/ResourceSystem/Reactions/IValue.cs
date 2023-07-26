
namespace ResourceSystem.Reactions
{
    public interface IValue
    {
        ColonyShared.SharedCode.Value Value { get; }
    }

    public interface IValue<T> : IValue, IVar<T>
    {
        new T Value { get; }
    }
}