using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Science
{
    [Localized]
    public class KnowledgeSectionDef : SaveableBaseResource, IHasKnowledgeContent
    {
        public ResourceRef<KnowledgeSectionDef> Section;
        public RecordContent Content { get; set; }
        public int SortingOrder { get; set; }
        public string LinkId { get; set; }
    }

    [Localized]
    public class KnowledgeRecordDef : KnowledgeSectionDef
    {
        public bool IsKeyKnowledge { get; set; }
    }

    public interface IHasKnowledgeContent
    {
        RecordContent Content { get; }
    }

    [KnownToGameResources, Localized]
    public struct RecordContent
    {
        public LocalizedString TitleLs { get; set; }

        public LocalizedString TextLs { get; set; }

        public UnityRef<Sprite> Image { get; set; }
    }
}