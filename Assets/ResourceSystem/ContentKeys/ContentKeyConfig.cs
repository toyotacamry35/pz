using Assets.Src.ResourcesSystem.Base;
using SharedCode.Config;

namespace ResourceSystem.ContentKeys
{
    public class ContentKeyConfig : CustomConfig
    {
        public ResourceRef<ContentKeyDef>[] Keys { get; set; } = { };
    }
}
