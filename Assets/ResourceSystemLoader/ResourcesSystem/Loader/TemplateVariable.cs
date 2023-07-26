using System;
using Assets.Src.ResourcesSystem.Base;

namespace ResourcesSystem.Loader
{
    [KnownToGameResources]
    public class TemplateVariable
    {
        [NotInSchema]
        public ResourceIDFull VariableId { get; set; }
        public Type Type { get; set; }
        public System.Object Value { get; set; }
    }

}
