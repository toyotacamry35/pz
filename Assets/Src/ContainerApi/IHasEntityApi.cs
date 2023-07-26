using System;

namespace Assets.Src.ContainerApis
{
    public interface IHasEntityApi : IDisposable
    {
        EntityApi Api { get; }
    }
}