using Assets.Src.Aspects.Impl.Factions.Template;
using ReactivePropsNs;

namespace Uins
{
    public class QuestPhaseData
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static int nextFreeId = 0;
        public readonly int Id;

        public PhaseDef PhaseDef;
        public bool IsCurrent;
        public bool IsDone;
        public bool IsDelimiterHidden;

        public readonly IStream<QuestStateReactive> QuestState;

        DisposableComposite D = new DisposableComposite();

        public QuestPhaseData(IStream<QuestStateReactive> questState) {
            Id = ++nextFreeId;
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$$$ new QuestPhaseData[Id={Id}]({questState}=>{questState.StreamState()}").Write();
            QuestState = questState;
            // PZ-15197 // QuestState.Subscribe(D,
            // PZ-15197 //     questStateR => Logger.Error($"$$$$$$$$$$$$ QuestPhaseData[{Id}].QuestState.OnNext:{questStateR}"),
            // PZ-15197 //     () => Logger.Error($"$$$$$$$$$$$$ QuestPhaseData[{Id}].QuestState.OnComplete()\n{new System.Diagnostics.StackTrace(true)}")
            // PZ-15197 // );
        }

        public override string ToString() => $"[QuestPhaseData[Id={Id}], Def={PhaseDef}, QuestState={QuestState}]";
    }
}