namespace ResourceSystem.Reactions
{
    public interface IArg
    {
        ArgDef Def { get; }
    }

    public interface IArg<T> : IArg, IVar<T>
    {
        new ArgDef<T> Def { get; }
    }
}
