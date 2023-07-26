using System;

namespace Assets.Src.ResourcesSystem.Base
{
    public interface ISaveableResource : IResource
    {
        Guid Id { get; set; }
    }
}