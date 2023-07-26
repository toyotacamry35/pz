namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class MutationState
    {
        public MutatingFactionDef Faction;
        public MutatingFactionDef NewFaction;
        public MutationStageDef Stage;
        public MutationStageDef NewStage;
        public float Mutation;

        public MutationState() { }

        public MutationState(MutatingFactionDef faction, MutationStageDef stage, MutatingFactionDef newFaction, MutationStageDef newStage, float mutation)
        {
            Faction = faction;
            Stage = stage;
            NewFaction = newFaction;
            NewStage = newStage;
            Mutation = mutation;
        }

        public MutationState(MutationState state)
        {
            Faction = state.Faction;
            Stage = state.Stage;
            NewFaction = state.NewFaction;
            NewStage = state.NewStage;
            Mutation = state.Mutation;
        }

        public static bool operator ==(MutationState obj1, MutationState obj2)
        {
            return obj1?.Equals(obj2) ?? false;
        }

        public static bool operator !=(MutationState obj1, MutationState obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            var state = obj as MutationState;
            return state != null &&
                   Faction == state?.Faction &&
                   NewFaction == state?.NewFaction &&
                   Stage == state?.Stage &&
                   NewStage == state?.NewStage &&
                   Mutation == state?.Mutation;
        }

        public override string ToString()
        {
            return $"Mutation = {Mutation}; Now: ({Faction?.____GetDebugShortName() ?? ""} {Stage?.____GetDebugShortName() ?? ""}); After Death: ({NewFaction?.____GetDebugShortName() ?? ""} {NewStage?.____GetDebugShortName() ?? ""})";
        }
    }
}
