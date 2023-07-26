using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class MutatingFactionsDef : SaveableBaseResource
    {
        public ResourceRef<MutatingFactionDef>[] Factions { get; set; }

        public ResourceRef<MutatingFactionDef> DefaultFaction { get; set; }

        public float DefaultMutation { get; set; }
    }
}
