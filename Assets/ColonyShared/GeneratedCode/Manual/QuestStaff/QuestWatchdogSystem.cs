using Assets.Src.Aspects.Impl.Factions.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Manual.QuestStaff
{
    public static class QuestWatchdogSystem
    {
        private static Dictionary<QuestDef, LinkedList<QuestDiagnosticInfo>> questDiagnosticInfos = new Dictionary<QuestDef, LinkedList<QuestDiagnosticInfo>>(32);
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Register(QuestDef quest, Action<QuestDiagnosticInfo> OnComplete = null, Action<QuestDiagnosticInfo> OnFastComplete = null)
        {
            if (!questDiagnosticInfos.ContainsKey(quest))
                questDiagnosticInfos.Add(quest, new LinkedList<QuestDiagnosticInfo>());
            else if (!questDiagnosticInfos[quest].Last?.Value?.Completed ?? true)
            {
                Logger.IfWarn()?.Message($"Attempt to register a quest[{quest}] when the same one is not yet completed").Write();
                return;
            }

            var qdi = new QuestDiagnosticInfo(quest);
            if (OnComplete != null)
                qdi.OnComplete += OnComplete;
            if (OnFastComplete != null)
                qdi.OnFastComplete += OnFastComplete;

            qdi.StartedAt = DateTime.UtcNow;
            questDiagnosticInfos[quest].AddLast(qdi);
            return;

        }
        public static void QuestFinished(QuestDef quest)
        {
            if(!questDiagnosticInfos.ContainsKey(quest))
            {
                Logger.IfError()?.Message("Attempt to finished not registered quest").Write();
                return;
            }
            else if(questDiagnosticInfos[quest].Last.Value.Completed)
            {
                Logger.IfError()?.Message("Attempt to finished already completed quest").Write();
                return;
            }
            questDiagnosticInfos[quest].Last.Value.EndedAt = DateTime.UtcNow;
        }
        
        public static void Dump()
        {
           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n----------[QuestWatchdogSystem]----------");
            
            foreach (var item in questDiagnosticInfos)
            {
                sb.Append("Quest: ").AppendLine(item.Key.ToString());
                foreach (var qdi in item.Value)
                    sb.Append("\t" + qdi.ToString().Replace("\r\n", "\r\n\t")).AppendLine();
            }
            sb.AppendLine("----------[QuestWatchdogSystem]----------");
            Logger.IfDebug()?.Message(sb.ToString()).Write();
        }
    }
}
