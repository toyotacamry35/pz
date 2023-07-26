using System;

namespace Assets.Src.ResourcesSystem.Base
{
    public struct SaveableResource
    {
        public Guid Id { get; set; }
        public string Root { get; set; }
    }

    public class SaveableResourceIndex : BaseResource
    {
        public SaveableResource[] Resources { get; set; } = Array.Empty<SaveableResource>();
    }
}
