using System;

namespace ResourcesSystem.Loader
{
    public class KnownToGameResourcesAttribute : Attribute
    {
        public bool Serializable { get; set; } = true;
    }
}
